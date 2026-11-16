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
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Talegen.Apple.Storekit.Client;
    using Talegen.Apple.Storekit.Models;
    using Talegen.Apple.Storekit.Models.Api;

    /// <summary>
    /// This class contains Subscription extension methods for the <see cref="IAppStoreServerApiClient"/> interface.
    /// </summary>
    public static class SubscriptionExtensions
    {
        /// <summary>
        /// This method is used to get the statuses for all of a customer's auto-renewable subscriptions in your app.
        /// </summary>
        /// <param name="client">Contains the client to extend.</param>
        /// <param name="transactionId">Contains a transaction identifier that belongs to the customer.</param>
        /// <param name="status">Contains an optional filter limiting results to subscriptions with this status (e.g. active).</param>
        /// <param name="environment">Contains the environment override to use for the request.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns the subscription status.</returns>
        public static async Task<SubscriptionStatusResponse> GetSubscriptionStatusAsync(this IAppStoreServerApiClient client, string transactionId, ApplePayloadMetadataStatusType? status = null, EnvironmentType? environment = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentException.ThrowIfNullOrWhiteSpace(transactionId);

            string endpoint = $"/inApps/v1/subscriptions/{Uri.EscapeDataString(transactionId)}";
            var queryParameters = new Dictionary<string, string>();

            if (status != null)
            {
                queryParameters.Add("status", ((int)status.Value).ToString(CultureInfo.InvariantCulture));
            }

            return await client.MakeRequest<SubscriptionStatusResponse>(endpoint, HttpMethod.Get, queryParameters, environment: environment, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// This overload filtered by the wrong enumeration; the <c>status</c> filter is a subscription status, not an auto-renew status.
        /// </summary>
        /// <param name="client">Contains the client to extend.</param>
        /// <param name="transactionId">Contains the transaction identifier.</param>
        /// <param name="status">Contains the status filter; its numeric value is passed through unchanged.</param>
        /// <param name="environment">Contains the environment override to use for the request.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns the subscription status.</returns>
        [Obsolete("The status filter is a subscription status. Use the overload that takes ApplePayloadMetadataStatusType.")]
        public static Task<SubscriptionStatusResponse> GetSubscriptionStatusAsync(this IAppStoreServerApiClient client, string transactionId, AppleAutoRenewStatuses? status, EnvironmentType? environment = null, CancellationToken cancellationToken = default)
        {
            return client.GetSubscriptionStatusAsync(transactionId, status.HasValue ? (ApplePayloadMetadataStatusType)(int)status.Value : (ApplePayloadMetadataStatusType?)null, environment, cancellationToken);
        }

        /// <summary>
        /// This method extends the renewal date of a customer's active auto-renewable subscription.
        /// </summary>
        /// <param name="client">Contains the client to extend.</param>
        /// <param name="originalTransactionId">Contains the original transaction identifier of the subscription.</param>
        /// <param name="request">Contains the extension request. Reuse the same <see cref="ExtendRenewalDateRequest.RequestIdentifier"/> when retrying.</param>
        /// <param name="environment">Contains the environment override to use for the request.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns the extension result.</returns>
        public static async Task<ExtendRenewalDateResponse> ExtendSubscriptionRenewalDateAsync(this IAppStoreServerApiClient client, string originalTransactionId, ExtendRenewalDateRequest request, EnvironmentType? environment = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentException.ThrowIfNullOrWhiteSpace(originalTransactionId);
            ArgumentNullException.ThrowIfNull(request);

            if (request.ExtendByDays < 1 || request.ExtendByDays > 90)
            {
                throw new ArgumentOutOfRangeException(nameof(request), request.ExtendByDays, "ExtendByDays must be between 1 and 90.");
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(request.RequestIdentifier, nameof(request.RequestIdentifier));

            string endpoint = $"/inApps/v1/subscriptions/extend/{Uri.EscapeDataString(originalTransactionId)}";
            return await client.MakeRequest<ExtendRenewalDateResponse>(endpoint, HttpMethod.Put, requestBody: request, environment: environment, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
