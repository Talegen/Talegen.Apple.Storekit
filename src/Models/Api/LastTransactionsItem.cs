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
namespace Talegen.Apple.Storekit.Models.Api
{
    using System.Text.Json.Serialization;
    using Talegen.Apple.Storekit.Extensions;

    /// <summary>
    /// This class represents the last transactions item model.
    /// </summary>
    public class LastTransactionsItem
    {
        /// <summary>
        /// Gets or sets the original transaction identifier of the auto-renewable subscription.
        /// </summary>
        [JsonPropertyName("originalTransactionId")]
        public string OriginalTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the status of the auto-renewable subscription. 
        /// </summary>
        [JsonPropertyName("status")]
        public ApplePayloadMetadataStatusType Status { get; set; }

        /// <summary>
        /// Gets or sets the subscription renewal information signed by the App Store, in JSON Web Signature (JWS) format.
        /// </summary>
        [JsonPropertyName("signedRenewalInfo")]
        public string SignedRenewalInfo { get; set; }

        /// <summary>
        /// Gets the renewal information decoded payload.
        /// </summary>
        [JsonIgnore]
        public JwsRenewalInfoDecodedPayload RenewalInfoDecodedPayload => !string.IsNullOrWhiteSpace(this.SignedRenewalInfo) ? this.SignedRenewalInfo.DecodeJwtPayload<JwsRenewalInfoDecodedPayload>() : null;

        /// <summary>
        /// Gets or sets the transaction information signed by the App Store, in JWS format.
        /// </summary>
        [JsonPropertyName("signedTransactionInfo")]
        public string SignedTransactionInfo { get; set; }

        /// <summary>
        /// Gets the transaction information decoded payload.
        /// </summary>
        [JsonIgnore]
        public JwsTransactionDecodedPayload TransactionInfoDecodedPayload => !string.IsNullOrWhiteSpace(this.SignedTransactionInfo) ? this.SignedTransactionInfo.DecodeJwtPayload<JwsTransactionDecodedPayload>() : null;
    }
}
