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
namespace Talegen.Apple.Storekit.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Talegen.Apple.Storekit.Client;
    using Talegen.Apple.Storekit.Models.Api;

    /// <summary>
    /// This class contains extension methods for the <see cref="IAppStoreServerApiClient"/> interface.
    /// </summary>
    public static class TransactionExtensions
    {
        
        /// <summary>
        /// This method is used to get the transaction history details from the Apple App Store API.
        /// </summary>
        /// <param name="client">Contains the client to extend.</param>
        /// <param name="transactionId">The identifier of a transaction that belongs to the customer, and which may be an original transaction identifier</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns></returns>
        /// <remarks>
        /// This response contains results for all in-app purchase product types: auto-renewable subscriptions, non-renewing subscriptions, non-consumables, and consumables. 
        /// The result includes transactions in any state, including transactions that are refunded or revoked, and transactions that the app has or hasn't marked as finished.
        /// The history response returns a maximum of 20 transactions per response. If your customer has more than 20 in-app transactions, the hasMore value is true. 
        /// Each response includes a revision value. 
        /// Call Get Transaction History again with the revision token in the query to receive the next set of transactions; 
        /// use the same query parameters for each subsequent call that includes revision.
        /// When the App Store has no more transactions to send, the hasMore value is false.
        /// If a customer upgrades a subscription or the App Store revokes an in-app purchase while you're receiving transaction history, the App Store updates the relevant 
        /// transaction.
        /// If the response is sorted in ASCENDING order, you receive the transaction again, with updated data.
        /// </remarks>
        public static async Task<TransactionHistoryResponse> GetTransactionHistoryAsync(IAppStoreServerApiClient client, string transactionId, TransactionHistoryRequest? requestModel = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(nameof(client));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(transactionId));
            string endpoint = $"/inApps/v2/history/{transactionId}";
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();

            if (requestModel != null)
            {
                queryParameters = requestModel.ToQueryDictionary();
            }

            var payloadResult = await client.MakeRequest<TransactionHistoryResponse>(endpoint, HttpMethod.Get, queryParameters, cancellationToken: cancellationToken);

            if (payloadResult != null)
            {
                // decode the payloads
                foreach (var signedTransaction in payloadResult.SignedTransactions)
                {
                    payloadResult.DecodedTransactions.Add(signedTransaction.DecodeJwtPayload<JwsTransactionDecodedPayload>());
                }
            }
            
            return payloadResult;
        }

        /// <summary>
        /// This method is used to get the transaction details from the Apple App Store API.
        /// </summary>
        /// <param name="client">Contains the client to extend.</param>
        /// <param name="transactionId">Contains the transaction id to retrieve.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns the <see cref="JwsTransactionDecodedPayload"/> returned.</returns>
        public static async Task<JwsTransactionDecodedPayload> GetTransactionAsync(IAppStoreServerApiClient client, string transactionId, CancellationToken cancellationToken = default)
        { 
            ArgumentNullException.ThrowIfNull(nameof(client));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(transactionId));

            string endpoint = $"/inApps/v1/transactions/{transactionId}";
            var payloadResult = await client.MakeRequest<JwsTransaction>(endpoint, HttpMethod.Get, cancellationToken: cancellationToken);
            return payloadResult != null ? payloadResult.SignedTransactionInfo.DecodeJwtPayload<JwsTransactionDecodedPayload>() : null;
        }
    }
}
