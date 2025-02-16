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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Talegen.Apple.Storekit.Models;
    using Talegen.Apple.Storekit.Models.Settings;

    /// <summary>
    /// This class contains the App Store Server API client implementation.
    /// </summary>
    public class AppStoreServerApiClient : IAppStoreServerApiClient
    {
        /// <summary>
        /// Contains the base URL for the Production API.
        /// </summary>
        private const string PRODUCTION_URL = "https://api.storekit.itunes.apple.com";

        /// <summary>
        /// Contains the base URL for the Sandbox API.
        /// </summary>
        private const string SANDBOX_URL = "https://api.storekit-sandbox.itunes.apple.com";

        /// <summary>
        /// Contains the base URL for Local Testing.
        /// </summary>
        private const string LOCAL_TESTING_URL = "https://local-testing-base-url";

        /// <summary>
        /// Contains the User-Agent string for the API client.
        /// </summary>
        private const string USER_AGENT = "app-store-server-library-dotnet/{0}";

        /// <summary>
        /// Contains the JSON content type.
        /// </summary>
        private const string JSON = "application/json; charset=utf-8";

        /// <summary>
        /// Contains the bearer token authenticator.
        /// </summary>
        private readonly IBearerTokenAuthenticator bearerTokenAuthenticator;

        /// <summary>
        /// Contains the base URI for the API.
        /// </summary>
        private Uri baseUri;

        /// <summary>
        /// Contains the HTTP client factory.
        /// </summary>
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Contains the JSON serializer options.
        /// </summary>
        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        /// <summary>
        /// Contains the Apple API settings.
        /// </summary>
        private readonly AppleApiSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppStoreServerApiClient" /> class.
        /// </summary>
        /// <param name="httpClientFactory">Contains the HTTP client factory.</param>
        /// <param name="bearerTokenAuthenticator">Contains the bearer token authenticator.</param>
        /// <param name="environment">Contains the environment type. Default is Production.</param>
        public AppStoreServerApiClient(IHttpClientFactory httpClientFactory, IBearerTokenAuthenticator bearerTokenAuthenticator, IOptions<AppleApiSettings> settingsOptions)
        {
            ArgumentNullException.ThrowIfNull(nameof(httpClientFactory));
            ArgumentNullException.ThrowIfNull(nameof(bearerTokenAuthenticator));
            ArgumentNullException.ThrowIfNull(nameof(settingsOptions));

            this.httpClientFactory = httpClientFactory;
            this.bearerTokenAuthenticator = bearerTokenAuthenticator;
            this.settings = settingsOptions.Value;

            this.SetEnvironment(this.settings.Environment);
        }

        /// <summary>
        /// This method is used to make a request to the Apple App Store API.
        /// </summary>
        /// <param name="path">Contains the API path.</param>
        /// <param name="method">Contains the HTTP method.</param>
        /// <param name="queryParameters">Contains additional query parameters.</param>
        /// <param name="requestBody">Contains a request body for POST/PUT calls.</param>
        /// <param name="environment">Contains the environment type.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns teh data requested.</returns>

        public async Task MakeRequest(string path, HttpMethod method, Dictionary<string, string>? queryParameters = null, object? requestBody = null, EnvironmentType? environment = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(nameof(path));
            ArgumentNullException.ThrowIfNull(nameof(method));

            if (environment != null)
            {
                this.SetEnvironment(environment.Value);
            }

            string queryString = queryParameters != null ? "?" + string.Join("&", queryParameters.Select(p => $"{p.Key}={UrlEncoder.Default.Encode(p.Value)}")) : string.Empty;
            Uri apiUri = new Uri(this.baseUri, path + queryString);

            HttpResponseMessage? responseMessage = null;
            using var client = this.CreateApiHttpClient();

            try
            {
                if (method == HttpMethod.Post)
                {
                    HttpContent httpContent;
                    if (requestBody != null)
                    {
                        httpContent = new StringContent(JsonSerializer.Serialize(requestBody, this.jsonSerializerOptions), Encoding.UTF8, JSON);
                    }
                    else
                    {
                        httpContent = new StringContent(string.Empty);
                    }
                    responseMessage = await client.PostAsync(apiUri, httpContent, cancellationToken);
                    responseMessage.EnsureSuccessStatusCode();
                }
                else if (method == HttpMethod.Put)
                {
                    HttpContent httpContent;
                    if (requestBody != null)
                    {
                        httpContent = new StringContent(JsonSerializer.Serialize(requestBody, this.jsonSerializerOptions), Encoding.UTF8, JSON);
                    }
                    else
                    {
                        httpContent = new StringContent(string.Empty);
                    }
                    responseMessage = await client.PutAsync(apiUri, httpContent, cancellationToken);
                    responseMessage.EnsureSuccessStatusCode();
                }
                else if (method == HttpMethod.Delete)
                {
                    responseMessage = await client.DeleteAsync(apiUri, cancellationToken);
                    responseMessage.EnsureSuccessStatusCode();
                }
            }
            catch (HttpRequestException rex)
            {
                AppleApiError appleApiError = null;

                if (responseMessage != null)
                {
                    // check to see if API error model was returned in content
                    appleApiError = await responseMessage.Content.ReadFromJsonAsync<AppleApiError>(this.jsonSerializerOptions, cancellationToken);
                }

                if (appleApiError != null)
                {
                    throw new AppleApiException(rex.StatusCode != null ? rex.StatusCode.Value : HttpStatusCode.InternalServerError, appleApiError, appleApiError.ErrorMessage ?? rex.Message, rex);
                }
                else
                {
                    throw new AppleApiException(rex.StatusCode != null ? rex.StatusCode.Value : HttpStatusCode.InternalServerError, rex);
                }
            }
            catch (Exception ex)
            {
                throw new AppleApiException(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// This method is used to make a request to the Apple App Store API.
        /// </summary>
        /// <typeparam name="T">Contains the data type to return.</typeparam>
        /// <param name="path">Contains the API path.</param>
        /// <param name="method">Contains the HTTP method.</param>
        /// <param name="queryParameters">Contains additional query parameters.</param>
        /// <param name="requestBody">Contains a request body for POST/PUT calls.</param>
        /// <param name="environment">Contains the environment type.</param>
        /// <param name="cancellationToken">Contains an optional cancellation token.</param>
        /// <returns>Returns teh data requested.</returns>
        public async Task<TReturn> MakeRequest<TReturn>(string path, HttpMethod method, Dictionary<string, string>? queryParameters = null, object? requestBody = null, EnvironmentType? environment = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(nameof(path));
            ArgumentNullException.ThrowIfNull(nameof(method));

            if (environment != null)
            {
                this.SetEnvironment(environment.Value);
            }
            
            string queryString = queryParameters != null ? "?" + string.Join("&", queryParameters.Select(p => $"{p.Key}={UrlEncoder.Default.Encode(p.Value)}")) : string.Empty;
            Uri apiUri = new Uri(this.baseUri, path + queryString);

            TReturn? result;
            HttpResponseMessage? responseMessage = null;
            using var client = this.CreateApiHttpClient();

            try
            {
                if (method == HttpMethod.Get)
                {
                    result = await client.GetFromJsonAsync<TReturn>(apiUri, this.jsonSerializerOptions, cancellationToken);
                }
                else if (method == HttpMethod.Post)
                {
                    HttpContent httpContent;
                    if (requestBody != null)
                    {
                        httpContent = new StringContent(JsonSerializer.Serialize(requestBody, this.jsonSerializerOptions), Encoding.UTF8, JSON);
                    }
                    else
                    {
                        httpContent = new StringContent(string.Empty);
                    }

                    responseMessage = await client.PostAsync(apiUri, httpContent, cancellationToken);
                    responseMessage.EnsureSuccessStatusCode();
                    result = await responseMessage.Content.ReadFromJsonAsync<TReturn>(this.jsonSerializerOptions, cancellationToken);
                }
                else if (method == HttpMethod.Put)
                {
                    HttpContent httpContent;
                    if (requestBody != null)
                    {
                        httpContent = new StringContent(JsonSerializer.Serialize(requestBody, this.jsonSerializerOptions), Encoding.UTF8, JSON);
                    }
                    else
                    {
                        httpContent = new StringContent(string.Empty);
                    }
                    responseMessage = await client.PutAsync(apiUri, httpContent, cancellationToken);
                    responseMessage.EnsureSuccessStatusCode();
                    result = await responseMessage.Content.ReadFromJsonAsync<TReturn>(this.jsonSerializerOptions, cancellationToken);
                }
                else if (method == HttpMethod.Delete)
                { 
                    result = await client.DeleteFromJsonAsync<TReturn>(apiUri, this.jsonSerializerOptions);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(method), method, "Invalid HTTP method.");
                }
            }
            catch (HttpRequestException rex)
            {
                AppleApiError appleApiError = null; 

                if (responseMessage != null)
                {
                    // check to see if API error model was returned in content
                    appleApiError = await responseMessage.Content.ReadFromJsonAsync<AppleApiError>(this.jsonSerializerOptions, cancellationToken);
                }

                if (appleApiError != null)
                {
                    throw new AppleApiException(rex.StatusCode != null ? rex.StatusCode.Value : HttpStatusCode.InternalServerError, appleApiError, appleApiError.ErrorMessage ?? rex.Message, rex);
                }
                else
                {
                    throw new AppleApiException(rex.StatusCode != null ? rex.StatusCode.Value : HttpStatusCode.InternalServerError, rex);
                }
            }
            catch (Exception ex)
            {
                throw new AppleApiException(HttpStatusCode.InternalServerError, ex);
            }

            return result;
        }

        /// <summary>
        /// This method is used to create a new HTTP client that utilizes a bearer token for authentication.
        /// </summary>
        /// <returns></returns>
        private HttpClient CreateApiHttpClient()
        {
            var client = this.httpClientFactory?.CreateClient() ?? new HttpClient();

            if (client.DefaultRequestHeaders.Contains("User-Agent"))
            {
                client.DefaultRequestHeaders.Remove("User-Agent");
            }

            // add agent header
            client.DefaultRequestHeaders.UserAgent.ParseAdd(string.Format(USER_AGENT, typeof(AppStoreServerApiClient).Assembly.GetName().Version));
            
            // if no authorization header exists, then generate a new token
            if (client.DefaultRequestHeaders.Authorization == null)
            {
                // Here you would generate a JWT token for authentication, which involves your private key
                string jwtToken = this.bearerTokenAuthenticator.GenerateToken();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
            }

            return client;
        }

        /// <summary>
        /// this method is used to set the environment.
        /// </summary>
        /// <param name="environment">Contains the apple environment.</param>

        private void SetEnvironment(EnvironmentType environment)
        {
            switch (environment)
            {
                case EnvironmentType.Production:
                    this.baseUri = new Uri(PRODUCTION_URL);
                    break;
                case EnvironmentType.Sandbox:
                    this.baseUri = new Uri(SANDBOX_URL);
                    break;
                case EnvironmentType.LocalTesting:
                    this.baseUri = new Uri(LOCAL_TESTING_URL);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(this.settings.Environment), this.settings.Environment, "Invalid environment type.");
            }
        }
    }
}
