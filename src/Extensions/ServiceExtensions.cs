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
    using Talegen.Apple.Storekit.Client;
    using Talegen.Apple.Storekit.Models.Settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceExtensions"/> class.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Adds Redis distributed caching services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="setupAction">An <see cref="Action{RedisCacheOptions}" /> to configure the provided <see cref="RedisCacheOptions" />.</param>
        /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
        public static IServiceCollection AddAppleApiClient(this IServiceCollection services, Action<AppleApiSettings> config)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            services.AddOptions();
            services.Configure(config);
            services.AddScoped<IBearerTokenAuthenticator, BearerTokenAuthenticator>();
            services.AddScoped<IAppStoreServerApiClient, AppStoreServerApiClient>();
            return services;
        }
    }
}
