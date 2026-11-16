namespace Talegen.Apple.Storekit.Tests.Client
{
    using System.Net;
    using Microsoft.Extensions.Options;
    using Talegen.Apple.Storekit.Client;
    using Talegen.Apple.Storekit.Models;
    using Talegen.Apple.Storekit.Models.Api;
    using Talegen.Apple.Storekit.Models.Settings;
    using Talegen.Apple.Storekit.Tests.TestSupport;

    public class AppStoreServerApiClientTests
    {
        [Theory]
        [InlineData(EnvironmentType.Production, "https://api.storekit.itunes.apple.com/inApps/v1/test")]
        [InlineData(EnvironmentType.Sandbox, "https://api.storekit-sandbox.itunes.apple.com/inApps/v1/test")]
        [InlineData(EnvironmentType.LocalTesting, "https://local-testing-base-url/inApps/v1/test")]
        public async Task MakeRequest_UsesConfiguredEnvironment(EnvironmentType environment, string expectedUrl)
        {
            var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, "{}");
            var client = ClientFactory.Create(handler, environment);

            await client.MakeRequest<SendTestNotificationResponse>("/inApps/v1/test", HttpMethod.Get);

            Assert.Equal(expectedUrl, handler.Requests[0].Request.RequestUri!.ToString());
        }

        [Fact]
        public async Task MakeRequest_EnvironmentOverride_AppliesToSingleRequestOnly()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, "{}").Enqueue(HttpStatusCode.OK, "{}");
            var client = ClientFactory.Create(handler, EnvironmentType.Production);

            await client.MakeRequest<SendTestNotificationResponse>("/x", HttpMethod.Get, environment: EnvironmentType.Sandbox);
            await client.MakeRequest<SendTestNotificationResponse>("/x", HttpMethod.Get);

            Assert.Equal("api.storekit-sandbox.itunes.apple.com", handler.Requests[0].Request.RequestUri!.Host);
            Assert.Equal("api.storekit.itunes.apple.com", handler.Requests[1].Request.RequestUri!.Host);
        }

        [Fact]
        public async Task MakeRequest_SetsAuthorizationAndUserAgent()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, "{}");
            var factory = new FakeHttpClientFactory(handler);
            var client = new AppStoreServerApiClient(factory, new StaticTokenAuthenticator(), Options.Create(new AppleApiSettings()));

            await client.MakeRequest<SendTestNotificationResponse>("/x", HttpMethod.Get);

            var request = handler.Requests[0].Request;
            Assert.Equal("Bearer", request.Headers.Authorization!.Scheme);
            Assert.Equal(StaticTokenAuthenticator.Token, request.Headers.Authorization.Parameter);
            Assert.StartsWith("app-store-server-library-dotnet/", request.Headers.UserAgent.ToString());
            Assert.Equal(AppStoreServerApiClient.HttpClientName, factory.LastName);
        }

        [Fact]
        public async Task MakeRequest_EncodesQueryParameters_AndSkipsEmptyQuery()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, "{}").Enqueue(HttpStatusCode.OK, "{}");
            var client = ClientFactory.Create(handler);

            await client.MakeRequest<SendTestNotificationResponse>("/x", HttpMethod.Get, new Dictionary<string, string> { ["revision"] = "a b&c=d" });
            await client.MakeRequest<SendTestNotificationResponse>("/x", HttpMethod.Get, new Dictionary<string, string>());

            Assert.Equal("?revision=a%20b%26c%3Dd", handler.Requests[0].Request.RequestUri!.Query);
            Assert.Equal(string.Empty, handler.Requests[1].Request.RequestUri!.Query);
        }

        [Fact]
        public async Task MakeRequest_Put_SerializesBodyAsJson()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.Accepted);
            var client = ClientFactory.Create(handler);

            await client.MakeRequest("/x", HttpMethod.Put, requestBody: new ExtendRenewalDateRequest { ExtendByDays = 5, ExtendReasonCode = ExtendReasonCode.Other, RequestIdentifier = "r" });

            var (request, body) = handler.Requests[0];
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal("application/json", request.Content!.Headers.ContentType!.MediaType);
            Assert.Equal("{\"extendByDays\":5,\"extendReasonCode\":2,\"requestIdentifier\":\"r\"}", body);
        }

        [Fact]
        public async Task MakeRequest_AppleErrorBody_IsSurfacedOnException()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.NotFound, "{\"errorCode\":4040010,\"errorMessage\":\"Transaction id not found.\"}");
            var client = ClientFactory.Create(handler);

            var ex = await Assert.ThrowsAsync<AppleApiException>(() => client.MakeRequest<JwsTransaction>("/x", HttpMethod.Get));

            Assert.Equal(HttpStatusCode.NotFound, ex.HttpStatusCode);
            Assert.Equal(AppleErrorCodes.TransactionIdNotFound, ex.ErrorCode);
            Assert.Equal("Transaction id not found.", ex.ApiErrorMessage);
            Assert.False(ex.IsRetryable);
        }

        [Fact]
        public async Task MakeRequest_NonJsonErrorBody_StillThrowsAppleApiException()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Unauthenticated") });
            var client = ClientFactory.Create(handler);

            var ex = await Assert.ThrowsAsync<AppleApiException>(() => client.MakeRequest("/x", HttpMethod.Delete));

            Assert.Equal(HttpStatusCode.Unauthorized, ex.HttpStatusCode);
            Assert.Null(ex.ErrorCode);
        }

        [Fact]
        public async Task MakeRequest_RateLimited_IsRetryable()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.TooManyRequests, "{\"errorCode\":4290000,\"errorMessage\":\"Rate limit exceeded.\"}");
            var client = ClientFactory.Create(handler);

            var ex = await Assert.ThrowsAsync<AppleApiException>(() => client.MakeRequest<JwsTransaction>("/x", HttpMethod.Get));

            Assert.Equal(AppleErrorCodes.RateLimitExceeded, ex.ErrorCode);
            Assert.True(ex.IsRetryable);
        }

        [Fact]
        public async Task MakeRequest_Cancellation_IsNotWrapped()
        {
            var client = ClientFactory.Create(new FakeHttpMessageHandler());
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.MakeRequest<JwsTransaction>("/x", HttpMethod.Get, cancellationToken: cts.Token));
        }

        [Fact]
        public async Task MakeRequest_TransportFailure_IsWrapped()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(_ => throw new HttpRequestException("connection refused"));
            var client = ClientFactory.Create(handler);

            var ex = await Assert.ThrowsAsync<AppleApiException>(() => client.MakeRequest<JwsTransaction>("/x", HttpMethod.Get));
            Assert.IsType<HttpRequestException>(ex.InnerException);
        }

        [Fact]
        public async Task MakeRequest_UnsupportedMethod_Throws()
        {
            var client = ClientFactory.Create(new FakeHttpMessageHandler());
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => client.MakeRequest("/x", HttpMethod.Patch));
        }

        [Fact]
        public async Task MakeRequest_EmptyPath_Throws()
        {
            var client = ClientFactory.Create(new FakeHttpMessageHandler());
            await Assert.ThrowsAsync<ArgumentException>(() => client.MakeRequest(" ", HttpMethod.Get));
        }

        [Fact]
        public void Constructor_NullArguments_Throw()
        {
            var settings = Options.Create(new AppleApiSettings());
            var factory = new FakeHttpClientFactory(new FakeHttpMessageHandler());

            Assert.Throws<ArgumentNullException>(() => new AppStoreServerApiClient(null!, new StaticTokenAuthenticator(), settings));
            Assert.Throws<ArgumentNullException>(() => new AppStoreServerApiClient(factory, null!, settings));
            Assert.Throws<ArgumentNullException>(() => new AppStoreServerApiClient(factory, new StaticTokenAuthenticator(), null!));
        }
    }
}
