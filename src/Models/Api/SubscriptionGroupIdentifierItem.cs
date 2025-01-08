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
    /// This class represents the subscription group identifier item model.
    /// </summary>
    public class SubscriptionGroupIdentifierItem
    {
        /// <summary>
        /// Gets or sets the subscription group identifier of the auto-renewable subscriptions in the lastTransactions array.
        /// </summary>
        [JsonPropertyName("subscriptionGroupIdentifier")]
        public string SubscriptionGroupIdentifier { get; set; }

        /// <summary>
        /// Gets or sets a list of the most recent App Store-signed transaction information and App Store-signed renewal information 
        /// for all auto-renewable subscriptions in the subscription group.
        /// </summary>
        [JsonPropertyName("lastTransactions")]
        public List<LastTransactionsItem> LastTransactions { get; set; }
    }
}
