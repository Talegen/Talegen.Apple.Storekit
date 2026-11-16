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
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Talegen.Apple.Storekit.Client;
    using Talegen.Apple.Storekit.Models.Settings;

    /// <summary>
    /// This class contains dependency injection registration extensions.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Adds the Apple App Store Server API client to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="config">An action used to configure the <see cref="AppleApiSettings" />.</param>
        /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
        /// <remarks>
        /// Registers <see cref="IAppStoreServerApiClient"/> (scoped) and <see cref="IBearerTokenAuthenticator"/> (singleton, so
        /// signed tokens are cached and reused until shortly before they expire), plus a named HTTP client
        /// (<see cref="AppStoreServerApiClient.HttpClientName"/>). Customize that client (timeouts, resilience handlers, proxies) with
        /// <c>services.AddHttpClient(AppStoreServerApiClient.HttpClientName).ConfigureHttpClient(...)</c>.
        /// </remarks>
        public static IServiceCollection AddAppleApiClient(this IServiceCollection services, Action<AppleApiSettings> config)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(config);

            services.AddOptions();
            services.Configure(config);
            services.TryAddSingleton<IBearerTokenAuthenticator, BearerTokenAuthenticator>();
            services.TryAddScoped<IAppStoreServerApiClient, AppStoreServerApiClient>();
            services.AddHttpClient(AppStoreServerApiClient.HttpClientName);
            return services;
        }

        /// <summary>
        /// Adds an <see cref="ISignedDataVerifier"/> used to verify App Store Server Notifications and signed transactions.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="config">An action used to configure the <see cref="SignedDataVerifierOptions" />. At least one Apple root certificate is required.</param>
        /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
        public static IServiceCollection AddAppleSignedDataVerifier(this IServiceCollection services, Action<SignedDataVerifierOptions> config)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(config);

            services.AddOptions();
            services.Configure(config);
            services.TryAddSingleton<ISignedDataVerifier, SignedDataVerifier>();
            return services;
        }
    }
}
