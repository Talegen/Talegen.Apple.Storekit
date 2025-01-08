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

    /// <summary>
    /// Contains the Apple Payload Metadata Status Type enumeration.
    /// </summary>
    public enum ApplePayloadMetadataStatusType 
    {
        /// <summary>
        /// The auto-renewable subscription is active.
        /// </summary>
        AutoRenewableSubscriptionActive = 1,

        /// <summary>
        /// The auto-renewable subscription has expired.
        /// </summary>
        AutoRenewableSubscriptionExpired = 2,

        /// <summary>
        /// The auto-renewable subscription is in the retry period.
        /// </summary>
        AutoRenewableSubscriptionInRetryPeriod = 3,

        /// <summary>
        /// The auto-renewable subscription is in the grace period.
        /// </summary>
        AutoRenewableSubscriptionInGracePeriod = 4,

        /// <summary>
        /// The auto-renewable subscription has been revoked.
        /// </summary>
        AutoRenewableSubscriptionRevoked = 5
    }

    /// <summary>
    /// This class contains the Apple Payload Metadata.
    /// </summary>
    public class ApplePayloadMetadata
    {
        /// <summary>
        /// Gets or sets the unique identifier of the app that the notification applies to. 
        /// This property is available for apps that users download from the App Store. It isn’t present in the sandbox environment.
        /// </summary>
        [JsonPropertyName("appAppleId")]
        public int AppAppleId { get; set; }

        /// <summary>
        /// Gets or sets the bundle identifier of the app.
        /// </summary>
        [JsonPropertyName("bundleId")]
        public string BundleId { get; set; }

        /// <summary>
        /// Gets or sets the version of the build that identifies an iteration of the bundle.
        /// </summary>
        [JsonPropertyName("bundleVersion")]
        public string BundleVersion { get; set; }

        /// <summary>
        /// Gets or sets the reason the customer requested the refund. 
        /// This field appears only for CONSUMPTION_REQUEST notifications, which the server sends when a customer 
        /// initiates a refund request for a consumable in-app purchase or auto-renewable subscription.
        /// </summary>
        [JsonPropertyName("consumptionRequestReason")]
        public string ConsumptionRequestReason { get; set; }

        /// <summary>
        /// Gets or sets the environment for which the notification applies to, either sandbox or production.
        /// </summary>
        [JsonPropertyName("environment")]
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets the subscription renewal information signed by the App Store, in JSON Web Signature (JWS) format.
        /// This field appears only for notifications that apply to auto-renewable subscriptions.
        /// </summary>
        [JsonPropertyName("signedRenewalInfo")]
        public string SignedRenewalInfo { get; set; }

        /// <summary>
        /// Gets or sets the transaction information signed by the App Store, in JSON Web Signature (JWS) format.
        /// </summary>
        [JsonPropertyName("signedTransactionInfo")]
        public string SignedTransactionInfo { get; set; }

        /// <summary>
        /// Gets or sets the status of an auto-renewable subscription as of the signedDate in the responseBodyV2DecodedPayload. 
        /// This field appears only for notifications sent for auto-renewable subscriptions.
        /// </summary>
        [JsonPropertyName("status")]
        public ApplePayloadMetadataStatusType Status { get; set; }
    }
}
