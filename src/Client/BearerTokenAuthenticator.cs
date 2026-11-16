/*
 * Talegen Apple Storekit Library
 * (c) Copyright Talegen, LLC.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/
namespace Talegen.Apple.Storekit.Client
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Talegen.Apple.Storekit.Models.Settings;

    /// <summary>
    /// This class implements the <see cref="IBearerTokenAuthenticator"/> interface and is used to generate a new bearer token for the client.
    /// </summary>
    /// <remarks>
    /// Tokens are signed with ES256 using the App Store Connect "In-App Purchase" key and are cached until shortly
    /// before they expire, so a token is not re-signed for every request.
    /// </remarks>
    public class BearerTokenAuthenticator : IBearerTokenAuthenticator
    {
        /// <summary>
        /// Contains the audience required by the App Store Server API.
        /// </summary>
        private const string AppStoreConnectAudience = "appstoreconnect-v1";

        /// <summary>
        /// Contains the default token lifetime. Apple rejects tokens with a lifetime over 60 minutes.
        /// </summary>
        private static readonly TimeSpan DefaultLifetime = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Contains the margin before expiry at which a cached token is regenerated.
        /// </summary>
        private static readonly TimeSpan RefreshMargin = TimeSpan.FromSeconds(60);

        private readonly string keyId;
        private readonly string issuerId;
        private readonly string bundleId;
        private readonly byte[] privateKeyBytes;
        private readonly DateTimeOffset? expiry;
        private readonly TimeProvider timeProvider;
        private readonly object syncRoot = new object();

        private string? cachedToken;
        private DateTimeOffset cachedTokenExpiry;

        /// <summary>
        /// Initializes a new instance of the <see cref="BearerTokenAuthenticator"/> class.
        /// </summary>
        /// <param name="optionsAccessor">Contains an options accessor for <see cref="AppleApiSettings"/></param>
        public BearerTokenAuthenticator(IOptions<AppleApiSettings> optionsAccessor)
            : this((optionsAccessor ?? throw new ArgumentNullException(nameof(optionsAccessor))).Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BearerTokenAuthenticator"/> class.
        /// </summary>
        /// <param name="settings">Contains the Apple API settings.</param>
        public BearerTokenAuthenticator(AppleApiSettings settings)
            : this(
                  (settings ?? throw new ArgumentNullException(nameof(settings))).KeyId,
                  settings.IssuerId,
                  settings.BundleId,
                  settings.PrivateKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BearerTokenAuthenticator"/> class.
        /// </summary>
        /// <param name="keyId">Contains the Kid</param>
        /// <param name="issuerId">Contains the issuer Id</param>
        /// <param name="bundleId">Contains the app bundle id</param>
        /// <param name="privateKey">Contains the API issued private key (PEM text of the .p8 file, or its base64 body).</param>
        /// <param name="expiry">Contains an optional fixed expiry. When not specified, each token expires 5 minutes after it is issued.</param>
        public BearerTokenAuthenticator(string keyId, string issuerId, string bundleId, string privateKey, DateTimeOffset? expiry = null)
            : this(keyId, issuerId, bundleId, privateKey, expiry, TimeProvider.System)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BearerTokenAuthenticator"/> class.
        /// </summary>
        /// <param name="keyId">Contains the Kid</param>
        /// <param name="issuerId">Contains the issuer Id</param>
        /// <param name="bundleId">Contains the app bundle id</param>
        /// <param name="privateKey">Contains the API issued private key.</param>
        /// <param name="expiry">Contains an optional fixed expiry.</param>
        /// <param name="timeProvider">Contains the time provider.</param>
        internal BearerTokenAuthenticator(string keyId, string issuerId, string bundleId, string privateKey, DateTimeOffset? expiry, TimeProvider timeProvider)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(keyId);
            ArgumentException.ThrowIfNullOrWhiteSpace(issuerId);
            ArgumentException.ThrowIfNullOrWhiteSpace(bundleId);
            ArgumentException.ThrowIfNullOrWhiteSpace(privateKey);
            ArgumentNullException.ThrowIfNull(timeProvider);

            this.keyId = keyId;
            this.issuerId = issuerId;
            this.bundleId = bundleId;
            this.privateKeyBytes = DecodePrivateKey(privateKey);
            this.expiry = expiry;
            this.timeProvider = timeProvider;

            // fail fast on an unusable key rather than on the first API call.
            using var probe = ECDsa.Create();
            probe.ImportPkcs8PrivateKey(this.privateKeyBytes, out _);
        }

        /// <summary>
        /// This method is used to generate a new bearer token for the client.
        /// </summary>
        /// <returns>Returns the JWT encoded token.</returns>
        public string GenerateToken()
        {
            var now = this.timeProvider.GetUtcNow();

            lock (this.syncRoot)
            {
                if (this.cachedToken != null && now < this.cachedTokenExpiry - RefreshMargin)
                {
                    return this.cachedToken;
                }

                var localExpiry = this.expiry ?? now.Add(DefaultLifetime);
                this.cachedToken = this.CreateToken(now, localExpiry);
                this.cachedTokenExpiry = localExpiry;
                return this.cachedToken;
            }
        }

        /// <summary>
        /// This method converts PEM or raw base64 private key text into PKCS#8 bytes.
        /// </summary>
        /// <param name="privateKey">Contains the private key text.</param>
        /// <returns>Returns the key bytes.</returns>
        internal static byte[] DecodePrivateKey(string privateKey)
        {
            string body = Regex.Replace(privateKey, "-----(BEGIN|END) [A-Z ]*PRIVATE KEY-----", string.Empty);
            body = Regex.Replace(body, @"\s+", string.Empty);

            try
            {
                return Convert.FromBase64String(body);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("The private key is not valid PEM or base64 text.", nameof(privateKey), ex);
            }
        }

        /// <summary>
        /// This method creates and signs a token.
        /// </summary>
        private string CreateToken(DateTimeOffset issuedAt, DateTimeOffset expiresAt)
        {
            using var ecdsa = ECDsa.Create();
            ecdsa.ImportPkcs8PrivateKey(this.privateKeyBytes, out _);

            var securityKey = new ECDsaSecurityKey(ecdsa) { KeyId = this.keyId };

            // disable provider caching: the key is disposed after signing and caching would otherwise grow unbounded.
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };

            var header = new JwtHeader(credentials);
            header[JwtHeaderParameterNames.Kid] = this.keyId;

            var payload = new JwtPayload
            {
                { "iss", this.issuerId },
                { "iat", issuedAt.ToUnixTimeSeconds() },
                { "exp", expiresAt.ToUnixTimeSeconds() },
                { "aud", AppStoreConnectAudience },
                { "bid", this.bundleId }
            };

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(header, payload));
        }
    }
}
