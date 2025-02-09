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
    /// The externalPurchaseToken object is part of the responseBodyV2DecodedPayload. It’s present in the payload when the notificationType 
    /// is EXTERNAL_PURCHASE_TOKEN. This notification type applies to apps that use the External Purchase API to offer alternative payment options.
    /// 
    /// This notification indicates that Apple didn't receive a report for the token within the time period specified in the Commission, transaction reports, 
    /// and payments section of the article Using alternative payment options on the App Store in the European Union. To report tokens with or without associated 
    /// transactions, call the Send External Purchase Report endpoint of the External Purchase Server API from your server.
    /// 
    /// The externalPurchaseToken object in the notification payload is the Base64URL-decoded JSON of the external purchase token your app or website 
    /// receive when your customer initiates an external purchase.For more information on the external purchase token, see Receiving and decoding external purchase tokens.
    /// </summary>
    public class ApplePayloadExternalPurchaseToken
    {
        /// <summary>
        /// Gets or sets the unique identifier of the token. 
        /// Use this value to report tokens and their associated transactions in the Send External Purchase Report endpoint.
        /// </summary>
        [JsonPropertyName("externalPurchaseId")]
        public string ExternalPurchaseId { get; set; }

        /// <summary>
        /// Gets or sets the UNIX time, in milliseconds, when the system created the token.
        /// </summary>
        [JsonPropertyName("tokenCreationDate")]
        public long? TokenCreationDateTime { get; set; }

        /// <summary>
        /// Gets or sets the app Apple ID for which the system generated the token.
        /// </summary>
        [JsonPropertyName("appAppleId")]
        public int AppAppleId { get; set; }

        /// <summary>
        /// Gets or sets the bundle identifier of the app.
        /// </summary>
        [JsonPropertyName("bundleId")]
        public string BundleId { get; set; }
    }
}
