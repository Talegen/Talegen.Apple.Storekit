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
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Talegen.Apple.Storekit.Models.Settings;

    /// <summary>
    /// This class implements the <see cref="IBearerTokenAuthenticator"/> interface and is used to generate a new bearer token for the client.
    /// </summary>
    public class BearerTokenAuthenticator : IBearerTokenAuthenticator
    {
        private const string AppStoreConnectAudience = "appstoreconnect-v1";

        private readonly string keyId;
        private readonly string issuerId;
        private readonly string bundleId;
        private readonly string privateKey;
        private readonly DateTimeOffset? expiry;

        /// <summary>
        /// Initializes a new instance of the <see cref="BearerTokenAuthenticator"/> class.
        /// </summary>
        /// <param name="optionsAccessor">Contains an options accessor for <see cref="AppleApiSettings"/></param>
        public BearerTokenAuthenticator(IOptions<AppleApiSettings> optionsAccessor)
            : this(optionsAccessor.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BearerTokenAuthenticator"/> class.
        /// </summary>
        /// <param name="settings">Contains the Apple API settings.</param>
        public BearerTokenAuthenticator(AppleApiSettings settings)
            : this(settings.KeyId, settings.IssuerId, settings.BundleId, settings.PrivateKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BearerTokenAuthenticator"/> class.
        /// </summary>
        /// <param name="keyId">Contains the Kid</param>
        /// <param name="issuerId">Contains the issuer Id</param>
        /// <param name="bundleId">Contains the app bundle id</param>
        /// <param name="privateKey">Contains the API issued private key.</param>
        /// <param name="expiry">Contains an optional expiry. Default is 5 minutes from UTC Now.</param>
        public BearerTokenAuthenticator(string keyId, string issuerId, string bundleId, string privateKey, DateTimeOffset? expiry = null)
        {
            ArgumentNullException.ThrowIfNull(nameof(keyId));
            ArgumentNullException.ThrowIfNull(nameof(issuerId));
            ArgumentNullException.ThrowIfNull(nameof(bundleId));
            ArgumentNullException.ThrowIfNull(nameof(privateKey));

            this.keyId = keyId;
            this.issuerId = issuerId;
            this.bundleId = bundleId;
            this.privateKey = privateKey;
            this.expiry = expiry;
        }

        /// <summary>
        /// This method is used to generate a new bearer token for the client.
        /// </summary>
        /// <returns>Returns the JWT encoded token.</returns>
        public string GenerateToken()
        {
            var ecdsa = ECDsa.Create(); // don't use 'using' as this will be disposed by SingingCredentials. Sucks.

            // clean-up the key text and convert to bytes
            var privateKeyBytes = Convert.FromBase64String(
                this.privateKey
                .Replace("-----BEGIN PRIVATE KEY-----", string.Empty)
                .Replace("-----END PRIVATE KEY-----", string.Empty)
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty));
            ecdsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);

            var securityKey = new ECDsaSecurityKey(ecdsa);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha256);
            var header = new JwtHeader(credentials);
            header[JwtHeaderParameterNames.Kid] = this.keyId;

            var now = DateTimeOffset.UtcNow;
            var localExpiry = this.expiry ?? now.AddMinutes(5);

            var payload = new JwtPayload
            {
                { "iss", this.issuerId },
                { "iat", now.ToUnixTimeSeconds() },
                { "exp", localExpiry.ToUnixTimeSeconds() },
                { "aud", AppStoreConnectAudience },
                { "bid", this.bundleId }
            };

            var token = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.WriteToken(token);

            return jwt;
        }
    }
}
