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
    using System.Net.Http.Headers;
    using System.Text;
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
        public const string ProductionUrl = "https://api.storekit.itunes.apple.com";

        /// <summary>
        /// Contains the base URL for the Sandbox API.
        /// </summary>
        public const string SandboxUrl = "https://api.storekit-sandbox.itunes.apple.com";

        /// <summary>
        /// Contains the base URL for Local Testing.
        /// </summary>
        public const string LocalTestingUrl = "https://local-testing-base-url";

        /// <summary>
        /// Contains the name of the named <see cref="HttpClient"/> used by this client.
        /// </summary>
        public const string HttpClientName = "Talegen.Apple.Storekit";

        /// <summary>
        /// Contains the User-Agent string for the API client.
        /// </summary>
        private const string USER_AGENT = "app-store-server-library-dotnet/{0}";

        /// <summary>
        /// Contains the JSON media type.
        /// </summary>
        private const string JSON = "application/json";

        /// <summary>
        /// Contains the shared JSON serializer options.
        /// </summary>
        internal static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        /// <summary>
        /// Contains the bearer token authenticator.
        /// </summary>
        private readonly IBearerTokenAuthenticator bearerTokenAuthenticator;

        /// <summary>
        /// Contains the HTTP client factory.
        /// </summary>
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Contains the Apple API settings.
        /// </summary>
        private readonly AppleApiSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppStoreServerApiClient" /> class.
        /// </summary>
        /// <param name="httpClientFactory">Contains the HTTP client factory.</param>
        /// <param name="bearerTokenAuthenticator">Contains the bearer token authenticator.</param>
        /// <param name="settingsOptions">Contains the Apple API settings.</param>
        public AppStoreServerApiClient(IHttpClientFactory httpClientFactory, IBearerTokenAuthenticator bearerTokenAuthenticator, IOptions<AppleApiSettings> settingsOptions)
        {
            ArgumentNullException.ThrowIfNull(httpClientFactory);
            ArgumentNullException.ThrowIfNull(bearerTokenAuthenticator);
            ArgumentNullException.ThrowIfNull(settingsOptions);

            this.httpClientFactory = httpClientFactory;
            this.bearerTokenAuthenticator = bearerTokenAuthenticator;
            this.settings = settingsOptions.Value ?? throw new ArgumentException("Settings value must not be null.", nameof(settingsOptions));

            // validate the configured environment eagerly so misconfiguration surfaces at construction.
            GetBaseUri(this.settings.Environment);
        }

        /// <inheritdoc />
        public async Task MakeRequest(string path, HttpMethod method, Dictionary<string, string>? queryParameters = null, object? requestBody = null, EnvironmentType? environment = null, CancellationToken cancellationToken = default)
        {
            using var response = await this.SendAsync(path, method, queryParameters, requestBody, environment, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<TReturn> MakeRequest<TReturn>(string path, HttpMethod method, Dictionary<string, string>? queryParameters = null, object? requestBody = null, EnvironmentType? environment = null, CancellationToken cancellationToken = default)
        {
            using var response = await this.SendAsync(path, method, queryParameters, requestBody, environment, cancellationToken).ConfigureAwait(false);

            try
            {
                if (response.Content.Headers.ContentLength == 0)
                {
                    return default!;
                }

                using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                return (await JsonSerializer.DeserializeAsync<TReturn>(stream, JsonOptions, cancellationToken).ConfigureAwait(false))!;
            }
            catch (JsonException jex)
            {
                throw new AppleApiException(response.StatusCode, jex);
            }
        }

        /// <summary>
        /// This method is used to build the absolute request URI.
        /// </summary>
        /// <param name="baseUri">Contains the base URI.</param>
        /// <param name="path">Contains the API path.</param>
        /// <param name="queryParameters">Contains optional query parameters.</param>
        /// <returns>Returns the absolute URI.</returns>
        internal static Uri BuildUri(Uri baseUri, string path, IEnumerable<KeyValuePair<string, string>>? queryParameters)
        {
            var builder = new StringBuilder(path);
            var parameters = queryParameters?.Where(p => !string.IsNullOrEmpty(p.Key)).ToList();

            if (parameters != null && parameters.Count > 0)
            {
                builder.Append('?');
                builder.Append(string.Join("&", parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value ?? string.Empty)}")));
            }

            return new Uri(baseUri, builder.ToString());
        }

        /// <summary>
        /// This method returns the base URI for the specified environment.
        /// </summary>
        /// <param name="environment">Contains the apple environment.</param>
        /// <returns>Returns the base URI.</returns>
        internal static Uri GetBaseUri(EnvironmentType environment)
        {
            return environment switch
            {
                EnvironmentType.Production => new Uri(ProductionUrl),
                EnvironmentType.Sandbox => new Uri(SandboxUrl),
                EnvironmentType.LocalTesting => new Uri(LocalTestingUrl),
                _ => throw new ArgumentOutOfRangeException(nameof(environment), environment, "Invalid environment type."),
            };
        }

        /// <summary>
        /// This method sends the request and converts non-success responses into <see cref="AppleApiException"/>.
        /// </summary>
        private async Task<HttpResponseMessage> SendAsync(string path, HttpMethod method, Dictionary<string, string>? queryParameters, object? requestBody, EnvironmentType? environment, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            ArgumentNullException.ThrowIfNull(method);

            if (method != HttpMethod.Get && method != HttpMethod.Post && method != HttpMethod.Put && method != HttpMethod.Delete)
            {
                throw new ArgumentOutOfRangeException(nameof(method), method, "Invalid HTTP method.");
            }

            // the environment override applies to this request only; the configured environment is never mutated.
            Uri apiUri = BuildUri(GetBaseUri(environment ?? this.settings.Environment), path, queryParameters);

            using var request = new HttpRequestMessage(method, apiUri);
            request.Headers.UserAgent.ParseAdd(string.Format(USER_AGENT, typeof(AppStoreServerApiClient).Assembly.GetName().Version));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.bearerTokenAuthenticator.GenerateToken());
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON));

            if (requestBody != null)
            {
                request.Content = new StringContent(JsonSerializer.Serialize(requestBody, requestBody.GetType(), JsonOptions), Encoding.UTF8, JSON);
            }
            else if (method == HttpMethod.Post || method == HttpMethod.Put)
            {
                request.Content = new StringContent(string.Empty, Encoding.UTF8, JSON);
            }

            HttpResponseMessage response;
            var client = this.httpClientFactory.CreateClient(HttpClientName);

            try
            {
                response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (TaskCanceledException tex)
            {
                // HttpClient timeout
                throw new AppleApiException(HttpStatusCode.RequestTimeout, tex);
            }
            catch (HttpRequestException rex)
            {
                throw new AppleApiException(rex.StatusCode ?? HttpStatusCode.ServiceUnavailable, rex);
            }

            if (!response.IsSuccessStatusCode)
            {
                using (response)
                {
                    AppleApiError? apiError = await TryReadErrorAsync(response, cancellationToken).ConfigureAwait(false);

                    if (apiError != null)
                    {
                        throw new AppleApiException(response.StatusCode, apiError, apiError.ErrorMessage ?? response.ReasonPhrase ?? response.StatusCode.ToString());
                    }

                    throw new AppleApiException(response.StatusCode);
                }
            }

            return response;
        }

        /// <summary>
        /// This method attempts to read an Apple error body from a failed response.
        /// </summary>
        private static async Task<AppleApiError?> TryReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            try
            {
                string content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(content))
                {
                    return null;
                }

                var error = JsonSerializer.Deserialize<AppleApiError>(content, JsonOptions);
                return error != null && error.ErrorCode != 0 ? error : null;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
