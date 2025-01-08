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
    /// This class represents the response from the Apple App Store API for the transaction history.
    /// </summary>
    public class TransactionHistoryResponse
    {
        /// <summary>
        /// Gets or sets the app’s identifier in the App Store.
        /// </summary>
        [JsonPropertyName("appAppleId")]
        public string AppAppleId { get; set; }

        /// <summary>
        /// Gets or sets the app’s bundle identifier.
        /// </summary>
        [JsonPropertyName("bundleId")]
        public string BundleId { get; set; }

        /// <summary>
        /// Gets or sets the server environment in which you’re making the request, whether sandbox or production.
        /// </summary>
        [JsonPropertyName("environment")]
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets a Boolean value that indicates whether the App Store has more transactions than it returns 
        /// in this response.
        /// If the value is true, use the revision token to request the next set of transactions.
        /// </summary>
        [JsonPropertyName("hasMore")]
        public bool HasMore { get; set; }

        /// <summary>
        /// Gets or sets a token you use in a query to request the next set of transactions from the endpoint.
        /// </summary>
        [JsonPropertyName("revision")]
        public string? Revision { get; set; }

        /// <summary>
        /// Gets or sets an array of in-app purchase transactions for the customer, signed by Apple, in JSON Web Signature (JWS) format.
        /// </summary>
        [JsonPropertyName("signedTransactions")]
        public List<string> SignedTransactions { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the decoded transactions.
        /// </summary>
        [JsonIgnore]
        public List<JwsTransactionDecodedPayload> DecodedTransactions { get; set; } = new List<JwsTransactionDecodedPayload>();
    }
}
