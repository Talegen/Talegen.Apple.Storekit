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
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Talegen.Apple.Storekit.Client;
    using Talegen.Apple.Storekit.Models;
    using Talegen.Apple.Storekit.Models.Api;

    /// <summary>
    /// This class contains transaction extension methods for the <see cref="IAppStoreServerApiClient"/> interface.
    /// </summary>
    public static class TransactionExtensions
    {
        /// <summary>
        /// This method is used to get one page (up to 20 transactions) of a customer's transaction history.
        /// </summary>
        /// <param name="client">Contains the client to extend.</param>
        /// <param name="transactionId">The identifier of a transaction that belongs to the customer, and which may be an original transaction identifier</param>
        /// <param name="requestModel">Contains optional filters and the <c>revision</c> token for subsequent pages.</param>
        /// <param name="environment">Contains the environment override to use for the request.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns the history page, with <see cref="TransactionHistoryResponse.DecodedTransactions"/> populated.</returns>
        /// <remarks>
        /// This response contains results for all in-app purchase product types: auto-renewable subscriptions, non-renewing subscriptions, non-consumables, and consumables.
        /// The result includes transactions in any state, including transactions that are refunded or revoked, and transactions that the app has or hasn't marked as finished.
        /// The history response returns a maximum of 20 transactions per response. If your customer has more than 20 in-app transactions, the hasMore value is true.
        /// Call again with the returned revision in <see cref="TransactionHistoryRequest.Revision"/> and the same filters to get the next page,
        /// or use <see cref="GetAllTransactionHistoryAsync"/> to page automatically.
        /// </remarks>
        public static async Task<TransactionHistoryResponse> GetTransactionHistoryAsync(this IAppStoreServerApiClient client, string transactionId, TransactionHistoryRequest? requestModel = null, EnvironmentType? environment = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentException.ThrowIfNullOrWhiteSpace(transactionId);

            string endpoint = $"/inApps/v2/history/{Uri.EscapeDataString(transactionId)}";
            var queryParameters = requestModel?.ToQueryParameters() ?? new Dictionary<string, string>();

            var payloadResult = await client.MakeRequest<TransactionHistoryResponse>(endpoint, HttpMethod.Get, queryParameters, environment: environment, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (payloadResult != null)
            {
                payloadResult.DecodedTransactions = DecodeAll(payloadResult.SignedTransactions);
            }

            return payloadResult!;
        }

        /// <summary>
        /// This method enumerates a customer's entire transaction history, requesting additional pages as needed.
        /// </summary>
        /// <param name="client">Contains the client to extend.</param>
        /// <param name="transactionId">The identifier of a transaction that belongs to the customer.</param>
        /// <param name="requestModel">Contains optional filters. The <c>Revision</c> property is managed by this method.</param>
        /// <param name="environment">Contains the environment override to use for the request.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns an asynchronous sequence of decoded transactions.</returns>
        public static async IAsyncEnumerable<JwsTransactionDecodedPayload> GetAllTransactionHistoryAsync(this IAppStoreServerApiClient client, string transactionId, TransactionHistoryRequest? requestModel = null, EnvironmentType? environment = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentException.ThrowIfNullOrWhiteSpace(transactionId);

            // copy the filters so the caller's instance is not mutated while paging.
            var request = new TransactionHistoryRequest
            {
                StartDate = requestModel?.StartDate,
                EndDate = requestModel?.EndDate,
                ProductId = requestModel?.ProductId,
                ProductType = requestModel?.ProductType,
                InAppOwnershipType = requestModel?.InAppOwnershipType,
                Sort = requestModel?.Sort,
                Revoked = requestModel?.Revoked,
                SubscriptionGroupIdentifier = requestModel?.SubscriptionGroupIdentifier,
            };

            while (true)
            {
                var page = await client.GetTransactionHistoryAsync(transactionId, request, environment, cancellationToken).ConfigureAwait(false);

                if (page == null)
                {
                    yield break;
                }

                foreach (var transaction in page.DecodedTransactions)
                {
                    yield return transaction;
                }

                if (!page.HasMore || string.IsNullOrEmpty(page.Revision) || page.Revision == request.Revision)
                {
                    yield break;
                }

                request.Revision = page.Revision;
            }
        }

        /// <summary>
        /// This method is used to get the transaction details from the Apple App Store API.
        /// </summary>
        /// <param name="client">Contains the client to extend.</param>
        /// <param name="transactionId">Contains the transaction id to retrieve.</param>
        /// <param name="environment">Contains the environment override to use for the request.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns the <see cref="JwsTransactionDecodedPayload"/> returned.</returns>
        public static async Task<JwsTransactionDecodedPayload> GetTransactionAsync(this IAppStoreServerApiClient client, string transactionId, EnvironmentType? environment = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentException.ThrowIfNullOrWhiteSpace(transactionId);

            string endpoint = $"/inApps/v1/transactions/{Uri.EscapeDataString(transactionId)}";
            var payloadResult = await client.MakeRequest<JwsTransaction>(endpoint, HttpMethod.Get, environment: environment, cancellationToken: cancellationToken).ConfigureAwait(false);
            return payloadResult != null && !string.IsNullOrWhiteSpace(payloadResult.SignedTransactionInfo)
                ? payloadResult.SignedTransactionInfo.DecodeJwtPayload<JwsTransactionDecodedPayload>()
                : null!;
        }

        /// <summary>
        /// This method gets one page of refunded in-app purchases for a customer.
        /// </summary>
        /// <param name="client">Contains the client to extend.</param>
        /// <param name="transactionId">The identifier of a transaction that belongs to the customer.</param>
        /// <param name="revision">Contains the revision token from a previous response, or <c>null</c> for the first page.</param>
        /// <param name="environment">Contains the environment override to use for the request.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns the refund history page, with decoded transactions.</returns>
        public static async Task<RefundHistoryResponse> GetRefundHistoryAsync(this IAppStoreServerApiClient client, string transactionId, string? revision = null, EnvironmentType? environment = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentException.ThrowIfNullOrWhiteSpace(transactionId);

            string endpoint = $"/inApps/v2/refund/lookup/{Uri.EscapeDataString(transactionId)}";
            var queryParameters = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(revision))
            {
                queryParameters["revision"] = revision;
            }

            var result = await client.MakeRequest<RefundHistoryResponse>(endpoint, HttpMethod.Get, queryParameters, environment: environment, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (result != null)
            {
                result.DecodedTransactions = DecodeAll(result.SignedTransactions);
            }

            return result!;
        }

        /// <summary>
        /// This method looks up the in-app purchases in an order, using the order ID from the customer's App Store receipt email.
        /// </summary>
        /// <param name="client">Contains the client to extend.</param>
        /// <param name="orderId">Contains the order ID (e.g. <c>MK5TTTVWJH</c>).</param>
        /// <param name="environment">Contains the environment override to use for the request.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns the lookup result, with decoded transactions.</returns>
        public static async Task<OrderLookupResponse> LookUpOrderIdAsync(this IAppStoreServerApiClient client, string orderId, EnvironmentType? environment = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentException.ThrowIfNullOrWhiteSpace(orderId);

            string endpoint = $"/inApps/v1/lookup/{Uri.EscapeDataString(orderId)}";
            var result = await client.MakeRequest<OrderLookupResponse>(endpoint, HttpMethod.Get, environment: environment, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (result != null)
            {
                result.DecodedTransactions = DecodeAll(result.SignedTransactions);
            }

            return result!;
        }

        /// <summary>
        /// Decodes a list of signed transactions.
        /// </summary>
        private static List<JwsTransactionDecodedPayload> DecodeAll(IEnumerable<string>? signedTransactions)
        {
            var decoded = new List<JwsTransactionDecodedPayload>();

            if (signedTransactions != null)
            {
                foreach (var signedTransaction in signedTransactions)
                {
                    decoded.Add(signedTransaction.DecodeJwtPayload<JwsTransactionDecodedPayload>());
                }
            }

            return decoded;
        }
    }
}
