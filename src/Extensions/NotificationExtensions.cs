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
    using System.Threading;
    using System.Threading.Tasks;
    using Talegen.Apple.Storekit.Client;
    using Talegen.Apple.Storekit.Models;
    using Talegen.Apple.Storekit.Models.Api;

    /// <summary>
    /// This class contains App Store Server Notification extension methods for the <see cref="IAppStoreServerApiClient"/> interface.
    /// </summary>
    public static class NotificationExtensions
    {
        /// <summary>
        /// This method asks the App Store to send a <c>TEST</c> notification to your configured notification URL.
        /// </summary>
        /// <param name="client">Contains the client to extend.</param>
        /// <param name="environment">Contains the environment override to use for the request.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns the token used to check the notification's delivery status.</returns>
        public static async Task<SendTestNotificationResponse> RequestTestNotificationAsync(this IAppStoreServerApiClient client, EnvironmentType? environment = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(client);
            return await client.MakeRequest<SendTestNotificationResponse>("/inApps/v1/notifications/test", HttpMethod.Post, environment: environment, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// This method checks whether a test notification reached your server.
        /// </summary>
        /// <param name="client">Contains the client to extend.</param>
        /// <param name="testNotificationToken">Contains the token returned by <see cref="RequestTestNotificationAsync"/>.</param>
        /// <param name="environment">Contains the environment override to use for the request.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns the delivery status.</returns>
        public static async Task<CheckTestNotificationResponse> GetTestNotificationStatusAsync(this IAppStoreServerApiClient client, string testNotificationToken, EnvironmentType? environment = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentException.ThrowIfNullOrWhiteSpace(testNotificationToken);

            string endpoint = $"/inApps/v1/notifications/test/{Uri.EscapeDataString(testNotificationToken)}";
            return await client.MakeRequest<CheckTestNotificationResponse>(endpoint, HttpMethod.Get, environment: environment, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
