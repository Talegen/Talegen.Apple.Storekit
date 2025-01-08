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
    /// The summary object appears in the responseBodyV2DecodedPayload when the notificationType is RENEWAL_EXTENSION and the subtype is SUMMARY. 
    /// This notification occurs when the App Store completes your request to extend the subscription renewal date for eligible subscribers. 
    /// </summary>
    public class ApplePayloadSummary
    {
        /// <summary>
        /// Gets or sets the UUID that represents a specific request to extend a subscription renewal date. 
        /// This value matches the value you initially specify in the requestIdentifier when you call Extend 
        /// Subscription Renewal Dates for All Active Subscribers in the App Store Server API.
        /// </summary>
        [JsonPropertyName("requestIdentifier")]
        public Guid RequestIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the environment that the notification applies to.
        /// </summary>
        [JsonPropertyName("environment")]
        public string Environment { get; set; }

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
        /// Gets or sets the product identifier of the auto-renewable subscription that the subscription-renewal-date extension applies to.
        /// </summary>
        [JsonPropertyName("productId")]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets a list of country codes that limits the App Store’s attempt to apply the subscription-renewal-date extension. 
        /// If this list isn’t present, the subscription-renewal-date extension applies to all storefronts.
        /// </summary>
        [JsonPropertyName("storefrontCountryCodes")]
        public List<string>? StorefrontCountryCodes { get; set; }

        /// <summary>
        /// Gets or sets the final count of subscriptions that fail to receive a subscription-renewal-date extension.
        /// </summary>
        [JsonPropertyName("failedCount")]
        public int FailedCount { get; set; }

        /// <summary>
        /// Gets or sets the final count of subscriptions that successfully receive a subscription-renewal-date extension.
        /// </summary>
        [JsonPropertyName("succeededCount")]
        public int SucceededCount { get; set; }
    }
}
