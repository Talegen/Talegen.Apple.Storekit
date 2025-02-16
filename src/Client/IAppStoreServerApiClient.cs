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
namespace Talegen.Apple.Storekit.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Talegen.Apple.Storekit.Models;

    /// <summary>
    /// This interface is used to define the Apple App Store API client.
    /// </summary>
    public interface IAppStoreServerApiClient
    {
        /// <summary>
        /// This method is used to make a request to the Apple App Store API.
        /// </summary>
        /// <param name="path">Contains the API path.</param>
        /// <param name="method">Contains the HTTP method.</param>
        /// <param name="queryParameters">Contains additional query parameters.</param>
        /// <param name="requestBody">Contains a request body for POST/PUT calls.</param>
        /// <param name="environment">Contains the environment to use for the request.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns teh data requested.</returns>
        Task MakeRequest(string path, HttpMethod method, Dictionary<string, string>? queryParameters = null, object? requestBody = null, EnvironmentType? environment = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// This method is used to make a request to the Apple App Store API.
        /// </summary>
        /// <typeparam name="T">Contains the data type to return.</typeparam>
        /// <param name="path">Contains the API path.</param>
        /// <param name="method">Contains the HTTP method.</param>
        /// <param name="queryParameters">Contains additional query parameters.</param>
        /// <param name="requestBody">Contains a request body for POST/PUT calls.</param>
        /// <param name="environment">Contains the environment to use for the request.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns teh data requested.</returns>
        Task<TReturn> MakeRequest<TReturn>(string path, HttpMethod method, Dictionary<string, string>? queryParameters = null, object? requestBody = null, EnvironmentType? environment = null, CancellationToken cancellationToken = default);
    }
}
