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
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Talegen.Apple.Storekit.Models;
    using Talegen.Apple.Storekit.Models.Api;

    /// <summary>
    /// Contains the reasons signed data verification can fail.
    /// </summary>
    public enum VerificationStatus
    {
        /// <summary>
        /// The value is not a well-formed JWS or its header is not supported.
        /// </summary>
        InvalidFormat,

        /// <summary>
        /// The x5c certificate chain is missing, invalid, or does not chain to a trusted Apple root.
        /// </summary>
        InvalidCertificate,

        /// <summary>
        /// The JWS signature does not match the leaf certificate.
        /// </summary>
        InvalidSignature,

        /// <summary>
        /// The bundle identifier or app Apple ID does not match the expected app.
        /// </summary>
        InvalidAppIdentifier,

        /// <summary>
        /// The environment in the payload does not match the expected environment.
        /// </summary>
        InvalidEnvironment,
    }

    /// <summary>
    /// The exception thrown when signed data from Apple fails verification.
    /// </summary>
    public class SignedDataVerificationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignedDataVerificationException"/> class.
        /// </summary>
        /// <param name="status">Contains the failure status.</param>
        /// <param name="message">Contains the message.</param>
        /// <param name="innerException">Contains an optional inner exception.</param>
        public SignedDataVerificationException(VerificationStatus status, string message, Exception? innerException = null)
            : base(message, innerException)
        {
            this.Status = status;
        }

        /// <summary>
        /// Gets the failure status.
        /// </summary>
        public VerificationStatus Status { get; }
    }

    /// <summary>
    /// Contains the options for <see cref="SignedDataVerifier"/>.
    /// </summary>
    public class SignedDataVerifierOptions
    {
        /// <summary>
        /// Gets the trusted Apple root certificates. Download them from https://www.apple.com/certificateauthority/
        /// (at minimum "Apple Root CA - G3").
        /// </summary>
        public List<X509Certificate2> RootCertificates { get; } = new List<X509Certificate2>();

        /// <summary>
        /// Gets or sets the expected app bundle identifier. When set, payloads for other apps are rejected.
        /// </summary>
        public string? BundleId { get; set; }

        /// <summary>
        /// Gets or sets the expected app Apple ID. When set, production payloads for other apps are rejected.
        /// </summary>
        public long? AppAppleId { get; set; }

        /// <summary>
        /// Gets or sets the expected environment. When set, payloads from other environments are rejected.
        /// </summary>
        public EnvironmentType? Environment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether certificate revocation is checked online (OCSP/CRL).
        /// When enabled, the chain is validated at the current time; otherwise it is validated at the payload's signed date.
        /// </summary>
        public bool EnableOnlineChecks { get; set; }
    }

    /// <summary>
    /// This interface defines a verifier for data signed by the App Store.
    /// </summary>
    public interface ISignedDataVerifier
    {
        /// <summary>
        /// Verifies a JWS signed by the App Store and decodes its payload.
        /// </summary>
        /// <typeparam name="T">Contains the payload type.</typeparam>
        /// <param name="signedData">Contains the JWS compact serialization.</param>
        /// <returns>Returns the decoded payload.</returns>
        T VerifyAndDecode<T>(string signedData);

        /// <summary>
        /// Verifies and decodes an App Store Server Notification V2 <c>signedPayload</c>, including app and environment checks.
        /// </summary>
        /// <param name="signedPayload">Contains the notification <c>signedPayload</c>.</param>
        /// <returns>Returns the decoded notification.</returns>
        AppleResponseBodyV2DecodedPayload VerifyAndDecodeNotification(string signedPayload);

        /// <summary>
        /// Verifies and decodes a signed transaction, including app and environment checks.
        /// </summary>
        /// <param name="signedTransaction">Contains the signed transaction.</param>
        /// <returns>Returns the decoded transaction.</returns>
        JwsTransactionDecodedPayload VerifyAndDecodeTransaction(string signedTransaction);

        /// <summary>
        /// Verifies and decodes signed subscription renewal information, including the environment check.
        /// </summary>
        /// <param name="signedRenewalInfo">Contains the signed renewal information.</param>
        /// <returns>Returns the decoded renewal information.</returns>
        JwsRenewalInfoDecodedPayload VerifyAndDecodeRenewalInfo(string signedRenewalInfo);
    }

    /// <summary>
    /// Verifies data signed by the App Store: notifications, transactions and renewal information.
    /// </summary>
    /// <remarks>
    /// Verification checks that the JWS header uses ES256, that its x5c chain (leaf, intermediate, root) builds to one of
    /// the configured trusted Apple roots, that the leaf and intermediate certificates carry Apple's marker OIDs, and that the
    /// signature matches the leaf public key. Typed helpers additionally check the bundle ID, app Apple ID and environment.
    /// </remarks>
    public class SignedDataVerifier : ISignedDataVerifier
    {
        /// <summary>
        /// Contains the OID Apple places on App Store receipt-signing leaf certificates.
        /// </summary>
        internal const string LeafCertificateOid = "1.2.840.113635.100.6.11.1";

        /// <summary>
        /// Contains the OID Apple places on the WWDR intermediate certificate.
        /// </summary>
        internal const string IntermediateCertificateOid = "1.2.840.113635.100.6.2.1";

        private readonly SignedDataVerifierOptions options;
        private readonly TimeProvider timeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignedDataVerifier"/> class.
        /// </summary>
        /// <param name="options">Contains the verifier options.</param>
        public SignedDataVerifier(IOptions<SignedDataVerifierOptions> options)
            : this((options ?? throw new ArgumentNullException(nameof(options))).Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignedDataVerifier"/> class.
        /// </summary>
        /// <param name="options">Contains the verifier options.</param>
        public SignedDataVerifier(SignedDataVerifierOptions options)
            : this(options, TimeProvider.System)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignedDataVerifier"/> class.
        /// </summary>
        /// <param name="options">Contains the verifier options.</param>
        /// <param name="timeProvider">Contains the time provider.</param>
        internal SignedDataVerifier(SignedDataVerifierOptions options, TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(timeProvider);

            if (options.RootCertificates.Count == 0)
            {
                throw new ArgumentException("At least one trusted Apple root certificate is required.", nameof(options));
            }

            this.options = options;
            this.timeProvider = timeProvider;
        }

        /// <inheritdoc />
        public T VerifyAndDecode<T>(string signedData)
        {
            return JsonSerializer.Deserialize<T>(this.VerifySignature(signedData), AppStoreServerApiClient.JsonOptions)!;
        }

        /// <inheritdoc />
        public AppleResponseBodyV2DecodedPayload VerifyAndDecodeNotification(string signedPayload)
        {
            var notification = this.VerifyAndDecode<AppleResponseBodyV2DecodedPayload>(signedPayload);

            string? bundleId = notification.Data?.BundleId ?? notification.Summary?.BundleId ?? notification.ExternalPurchaseToken?.BundleId;
            long? appAppleId = notification.Data?.AppAppleId ?? notification.Summary?.AppAppleId ?? notification.ExternalPurchaseToken?.AppAppleId;
            string? environment = notification.Data?.Environment ?? notification.Summary?.Environment;

            if (environment == null && notification.ExternalPurchaseToken != null)
            {
                // external purchase token notifications carry no environment field; Apple marks sandbox tokens with an "SANDBOX_" id prefix.
                environment = notification.ExternalPurchaseToken.ExternalPurchaseId?.StartsWith("SANDBOX", StringComparison.OrdinalIgnoreCase) == true ? "Sandbox" : "Production";
            }

            this.VerifyAppAndEnvironment(bundleId, appAppleId, environment, checkBundleId: true, checkAppAppleId: true);
            return notification;
        }

        /// <inheritdoc />
        public JwsTransactionDecodedPayload VerifyAndDecodeTransaction(string signedTransaction)
        {
            var transaction = this.VerifyAndDecode<JwsTransactionDecodedPayload>(signedTransaction);
            this.VerifyAppAndEnvironment(transaction.BundleId, null, transaction.Environment, checkBundleId: true, checkAppAppleId: false);
            return transaction;
        }

        /// <inheritdoc />
        public JwsRenewalInfoDecodedPayload VerifyAndDecodeRenewalInfo(string signedRenewalInfo)
        {
            var renewalInfo = this.VerifyAndDecode<JwsRenewalInfoDecodedPayload>(signedRenewalInfo);
            this.VerifyAppAndEnvironment(null, null, renewalInfo.Environment, checkBundleId: false, checkAppAppleId: false);
            return renewalInfo;
        }

        /// <summary>
        /// Verifies the JWS and returns the raw payload bytes.
        /// </summary>
        private byte[] VerifySignature(string signedData)
        {
            if (string.IsNullOrWhiteSpace(signedData))
            {
                throw new SignedDataVerificationException(VerificationStatus.InvalidFormat, "The signed data is empty.");
            }

            string[] parts = signedData.Split('.');

            if (parts.Length != 3)
            {
                throw new SignedDataVerificationException(VerificationStatus.InvalidFormat, "The signed data is not a JWS compact serialization.");
            }

            JwsHeader header;
            byte[] payload;
            byte[] signature;

            try
            {
                header = JsonSerializer.Deserialize<JwsHeader>(Base64UrlEncoder.DecodeBytes(parts[0]))
                    ?? throw new JsonException("Empty header.");
                payload = Base64UrlEncoder.DecodeBytes(parts[1]);
                signature = Base64UrlEncoder.DecodeBytes(parts[2]);
            }
            catch (Exception ex) when (ex is FormatException || ex is JsonException || ex is ArgumentException)
            {
                throw new SignedDataVerificationException(VerificationStatus.InvalidFormat, "The signed data could not be decoded.", ex);
            }

            if (header.Algorithm != "ES256")
            {
                throw new SignedDataVerificationException(VerificationStatus.InvalidFormat, $"Unsupported JWS algorithm \"{header.Algorithm}\".");
            }

            if (header.CertificateChain == null || header.CertificateChain.Count != 3)
            {
                throw new SignedDataVerificationException(VerificationStatus.InvalidCertificate, "The x5c header must contain exactly three certificates.");
            }

            List<X509Certificate2> certificates;

            try
            {
                certificates = header.CertificateChain.Select(c => new X509Certificate2(Convert.FromBase64String(c))).ToList();
            }
            catch (Exception ex) when (ex is FormatException || ex is CryptographicException)
            {
                throw new SignedDataVerificationException(VerificationStatus.InvalidCertificate, "The x5c header contains an invalid certificate.", ex);
            }

            try
            {
                var leaf = certificates[0];
                var intermediate = certificates[1];

                this.VerifyChain(leaf, intermediate, payload);

                using var publicKey = leaf.GetECDsaPublicKey()
                    ?? throw new SignedDataVerificationException(VerificationStatus.InvalidCertificate, "The leaf certificate does not contain an EC public key.");

                byte[] signingInput = Encoding.ASCII.GetBytes(parts[0] + "." + parts[1]);

                if (!publicKey.VerifyData(signingInput, signature, HashAlgorithmName.SHA256, DSASignatureFormat.IeeeP1363FixedFieldConcatenation))
                {
                    throw new SignedDataVerificationException(VerificationStatus.InvalidSignature, "The JWS signature is invalid.");
                }

                return payload;
            }
            finally
            {
                foreach (var certificate in certificates)
                {
                    certificate.Dispose();
                }
            }
        }

        /// <summary>
        /// Verifies that the leaf and intermediate chain to a trusted root and carry the Apple marker OIDs.
        /// </summary>
        private void VerifyChain(X509Certificate2 leaf, X509Certificate2 intermediate, byte[] payload)
        {
            if (leaf.Extensions[LeafCertificateOid] == null)
            {
                throw new SignedDataVerificationException(VerificationStatus.InvalidCertificate, "The leaf certificate is not an App Store signing certificate.");
            }

            if (intermediate.Extensions[IntermediateCertificateOid] == null)
            {
                throw new SignedDataVerificationException(VerificationStatus.InvalidCertificate, "The intermediate certificate is not an Apple WWDR certificate.");
            }

            using var chain = new X509Chain();
            chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
            chain.ChainPolicy.CustomTrustStore.AddRange(this.options.RootCertificates.ToArray());
            chain.ChainPolicy.ExtraStore.Add(intermediate);
            chain.ChainPolicy.RevocationMode = this.options.EnableOnlineChecks ? X509RevocationMode.Online : X509RevocationMode.NoCheck;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
            chain.ChainPolicy.VerificationTime = this.GetVerificationTime(payload).UtcDateTime;
            chain.ChainPolicy.VerificationTimeIgnored = false;

            if (!chain.Build(leaf))
            {
                string reasons = string.Join(", ", chain.ChainStatus.Select(s => s.Status));
                throw new SignedDataVerificationException(VerificationStatus.InvalidCertificate, $"The certificate chain is not trusted ({reasons}).");
            }

            // the chain must actually pass through the intermediate supplied in the header.
            if (chain.ChainElements.Count != 3 || chain.ChainElements[1].Certificate.Thumbprint != intermediate.Thumbprint)
            {
                throw new SignedDataVerificationException(VerificationStatus.InvalidCertificate, "The certificate chain does not match the x5c header.");
            }
        }

        /// <summary>
        /// Returns the time at which the certificate chain is validated.
        /// </summary>
        private DateTimeOffset GetVerificationTime(byte[] payload)
        {
            var now = this.timeProvider.GetUtcNow();

            if (this.options.EnableOnlineChecks)
            {
                return now;
            }

            try
            {
                var dates = JsonSerializer.Deserialize<SignedDateProbe>(payload);
                return dates?.SignedDate is long signedDate ? DateTimeOffset.FromUnixTimeMilliseconds(signedDate) : now;
            }
            catch (JsonException)
            {
                return now;
            }
        }

        /// <summary>
        /// Verifies the bundle id, app Apple id and environment against the configured expectations.
        /// Only fields the payload type carries are checked (renewal info has no bundle id; transactions have no app Apple id).
        /// </summary>
        private void VerifyAppAndEnvironment(string? bundleId, long? appAppleId, string? environment, bool checkBundleId, bool checkAppAppleId)
        {
            if (checkBundleId && this.options.BundleId != null && !string.Equals(this.options.BundleId, bundleId, StringComparison.Ordinal))
            {
                throw new SignedDataVerificationException(VerificationStatus.InvalidAppIdentifier, $"The bundle identifier \"{bundleId}\" does not match the expected app.");
            }

            if (this.options.Environment != null && !EnvironmentMatches(this.options.Environment.Value, environment))
            {
                throw new SignedDataVerificationException(VerificationStatus.InvalidEnvironment, $"The environment \"{environment}\" does not match the expected environment {this.options.Environment}.");
            }

            if (checkAppAppleId && this.options.AppAppleId != null && this.options.Environment == EnvironmentType.Production && appAppleId != this.options.AppAppleId)
            {
                throw new SignedDataVerificationException(VerificationStatus.InvalidAppIdentifier, $"The app Apple ID \"{appAppleId}\" does not match the expected app.");
            }
        }

        /// <summary>
        /// Returns a value indicating whether an Apple environment string matches the expected environment.
        /// </summary>
        private static bool EnvironmentMatches(EnvironmentType expected, string? actual)
        {
            return expected switch
            {
                EnvironmentType.Production => string.Equals(actual, "Production", StringComparison.OrdinalIgnoreCase),
                EnvironmentType.Sandbox => string.Equals(actual, "Sandbox", StringComparison.OrdinalIgnoreCase),
                EnvironmentType.LocalTesting => string.Equals(actual, "LocalTesting", StringComparison.OrdinalIgnoreCase) || string.Equals(actual, "Xcode", StringComparison.OrdinalIgnoreCase),
                _ => false,
            };
        }

        /// <summary>
        /// Represents the JWS header fields used for verification.
        /// </summary>
        private sealed class JwsHeader
        {
            [JsonPropertyName("alg")]
            public string? Algorithm { get; set; }

            [JsonPropertyName("x5c")]
            public List<string>? CertificateChain { get; set; }
        }

        /// <summary>
        /// Reads only the signed date from a payload.
        /// </summary>
        private sealed class SignedDateProbe
        {
            [JsonPropertyName("signedDate")]
            public long? SignedDate { get; set; }
        }
    }
}
