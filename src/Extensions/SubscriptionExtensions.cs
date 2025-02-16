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
        /// This method is used to get the subscription status from the Apple App Store API.
        /// </summary>
        /// <param name="client">Contains the client to extend.</param>
        /// <param name="transactionId">Contains the transaction identifier.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns the subscription status.</returns>
        public static async Task<SubscriptionStatusResponse> GetSubscriptionStatusAsync(this IAppStoreServerApiClient client, string transactionId, AppleAutoRenewStatuses? status = null, EnvironmentType? environment = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(nameof(client));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(transactionId));
            string endpoint = $"/inApps/v1/subscriptions/{transactionId}";
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            
            if (status != null)
            {
                queryParameters.Add("status", ((int)status).ToString());
            }

            return await client.MakeRequest<SubscriptionStatusResponse>(endpoint, HttpMethod.Get, queryParameters, environment: environment, cancellationToken: cancellationToken);
        }
    }
}
