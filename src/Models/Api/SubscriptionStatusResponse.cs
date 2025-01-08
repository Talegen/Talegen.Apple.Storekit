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
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// This class represents the subscription status response model.
    /// </summary>
    public class SubscriptionStatusResponse
    {
        /// <summary>
        /// Gets or sets the application apple Id.
        /// </summary>
        [JsonPropertyName("appAppleId")]
        public string AppAppleId { get; set; }

        /// <summary>
        /// Gets or sets the bundle identifier.
        /// </summary>
        [JsonPropertyName("bundleId")]
        public string BundleId { get; set; }

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        [JsonPropertyName("environment")]
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets the subscription data.
        /// </summary>
        [JsonPropertyName("data")]
        public List<SubscriptionGroupIdentifierItem> Data { get; set; }
    }
}
