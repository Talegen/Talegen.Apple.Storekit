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
    /// <summary>
    /// This class contains the transaction history request model.
    /// </summary>
    public class TransactionHistoryRequest
    {
        /// <summary>
        /// Gets or sets the revision token.
        /// </summary>
        /// <remarks>
        /// A token you provide to get the next set of up to 20 transactions. All responses include a revision token. 
        /// Use the revision token from the previous HistoryResponse
        /// Note: The revision token is required in all requests except the initial request.
        /// For requests that use the revision token, include the same query parameters from the initial request.
        /// </remarks>
        public string? Revision { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <remarks>
        /// An optional start date of the timespan for the transaction history records you’re requesting. 
        /// The startDate needs to precede the endDate if you specify both dates. The results include a 
        /// transaction if its purchaseDate is equal to or greater than the startDate.
        /// This is converted to UNIX timestamp.
        /// </remarks>
        public long? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <remarks>
        /// An optional end date of the timespan for the transaction history records you’re requesting. 
        /// Choose an endDate that’s later than the startDate if you specify both dates. Using an endDate 
        /// in the future is valid. The results include a transaction if its purchaseDate is less than the endDate.
        /// This is converted to UNIX timestamp.
        /// </remarks>
        public long? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        /// <remarks>
        /// An optional filter that indicates the product identifier to include in the transaction history. 
        /// Your query may specify more than one productID.
        /// </remarks>
        public string? ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product type.
        /// </summary>
        /// <remarks>
        /// An optional filter that indicates the product type to include in the transaction history. Your query may specify more than one productType.
        /// Possible Values: AUTO_RENEWABLE, NON_RENEWABLE, CONSUMABLE, NON_CONSUMABLE
        /// </remarks>
        public string? ProductType { get; set; }

        /// <summary>
        /// Gets or sets an optional filter that limits the transaction history by the in-app ownership type.
        /// </summary>
        public string? InAppOwnershipType { get; set; }

        /// <summary>
        /// Gets or sets an optional sort order for the transaction history records. The response sorts the transaction records by their 
        /// recently modified date. The default value is ASCENDING, so you receive the oldest records first.
        /// </summary>
        /// <remarks>
        /// Possible Values: ASCENDING, DESCENDING
        /// </remarks>
        public string? Sort { get; set; }

        /// <summary>
        /// Gets or sets an optional Boolean value that indicates whether the response includes only revoked transactions when the value is true,
        /// or contains only non-revoked transactions when the value is false. By default, the request doesn't include this parameter.
        /// </summary>
        public bool? Revoked { get; set; }

        /// <summary>
        /// Gets or sets an optional filter that indicates the subscription group identifier to include in the transaction history.
        /// </summary>
        public string? SubscriptionGroupIdentifier { get; set; }
     }
}
