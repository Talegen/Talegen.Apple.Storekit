namespace Talegen.Apple.Storekit.Tests.Extensions
{
    using System.Net;
    using System.Text.Json;
    using Talegen.Apple.Storekit.Extensions;
    using Talegen.Apple.Storekit.Models.Api;
    using Talegen.Apple.Storekit.Tests.TestSupport;

    public class ApiExtensionsTests
    {
        private static string Json(object value) => JsonSerializer.Serialize(value);

        [Fact]
        public async Task GetTransactionAsync_DecodesSignedTransaction()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, Json(new { signedTransactionInfo = Jws.Unsigned(new { transactionId = "1000", productId = "p" }) }));
            var client = ClientFactory.Create(handler);

            var transaction = await client.GetTransactionAsync("1000");

            Assert.Equal("1000", transaction.TransactionId);
            Assert.Equal("/inApps/v1/transactions/1000", handler.Requests[0].Request.RequestUri!.AbsolutePath);
        }

        [Fact]
        public async Task GetTransactionHistoryAsync_SendsFiltersAndDecodes()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, Json(new
            {
                hasMore = false,
                revision = "r2",
                signedTransactions = new[] { Jws.Unsigned(new { transactionId = "a" }), Jws.Unsigned(new { transactionId = "b" }) },
            }));
            var client = ClientFactory.Create(handler);

            var page = await client.GetTransactionHistoryAsync("1000", new TransactionHistoryRequest { Sort = "DESCENDING", Revoked = true });

            Assert.Equal(new[] { "a", "b" }, page.DecodedTransactions.Select(t => t.TransactionId));
            var uri = handler.Requests[0].Request.RequestUri!;
            Assert.Equal("/inApps/v2/history/1000", uri.AbsolutePath);
            Assert.Contains("sort=DESCENDING", uri.Query);
            Assert.Contains("revoked=true", uri.Query);
        }

        [Fact]
        public async Task GetAllTransactionHistoryAsync_FollowsRevisionUntilDone()
        {
            var handler = new FakeHttpMessageHandler()
                .Enqueue(HttpStatusCode.OK, Json(new { hasMore = true, revision = "r1", signedTransactions = new[] { Jws.Unsigned(new { transactionId = "a" }) } }))
                .Enqueue(HttpStatusCode.OK, Json(new { hasMore = false, revision = "r2", signedTransactions = new[] { Jws.Unsigned(new { transactionId = "b" }) } }));
            var client = ClientFactory.Create(handler);
            var filters = new TransactionHistoryRequest { ProductType = "CONSUMABLE" };

            var ids = new List<string>();
            await foreach (var transaction in client.GetAllTransactionHistoryAsync("1000", filters))
            {
                ids.Add(transaction.TransactionId);
            }

            Assert.Equal(new[] { "a", "b" }, ids);
            Assert.DoesNotContain("revision", handler.Requests[0].Request.RequestUri!.Query);
            Assert.Contains("revision=r1", handler.Requests[1].Request.RequestUri!.Query);
            Assert.Contains("productType=CONSUMABLE", handler.Requests[1].Request.RequestUri!.Query);
            Assert.Null(filters.Revision);
        }

        [Fact]
        public async Task GetSubscriptionStatusAsync_SendsSubscriptionStatusFilter()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, Json(new { appAppleId = 1L, bundleId = "b", environment = "Production", data = Array.Empty<object>() }));
            var client = ClientFactory.Create(handler);

            var response = await client.GetSubscriptionStatusAsync("1000", ApplePayloadMetadataStatusType.AutoRenewableSubscriptionInGracePeriod);

            Assert.Equal(1L, response.AppAppleId);
            Assert.Equal("/inApps/v1/subscriptions/1000", handler.Requests[0].Request.RequestUri!.AbsolutePath);
            Assert.Equal("?status=4", handler.Requests[0].Request.RequestUri!.Query);
        }

        [Fact]
        public async Task GetSubscriptionStatusAsync_NullStatus_BindsToCurrentOverload()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, Json(new { data = Array.Empty<object>() }));
            var client = ClientFactory.Create(handler);

            // must compile without CS0121 and send no status filter.
            await client.GetSubscriptionStatusAsync("1000", null, environment: Talegen.Apple.Storekit.Models.EnvironmentType.Sandbox);

            Assert.Equal(string.Empty, handler.Requests[0].Request.RequestUri!.Query);
            Assert.Equal("api.storekit-sandbox.itunes.apple.com", handler.Requests[0].Request.RequestUri!.Host);
        }

        [Fact]
        public async Task SendConsumptionInfoAsync_PutsToConsumptionEndpoint()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.Accepted);
            var client = ClientFactory.Create(handler);

            await client.SendConsumptionInfoAsync("1000", new ConsumptionRequest());

            Assert.Equal(HttpMethod.Put, handler.Requests[0].Request.Method);
            Assert.Equal("/inApps/v1/transactions/consumption/1000", handler.Requests[0].Request.RequestUri!.AbsolutePath);
            Assert.Contains("\"customerConsented\":true", handler.Requests[0].Body);
        }

        [Fact]
        public async Task SendConsumptionInfoAsync_ValidatesArguments()
        {
            var client = ClientFactory.Create(new FakeHttpMessageHandler());

            await Assert.ThrowsAnyAsync<ArgumentException>(() => client.SendConsumptionInfoAsync("", new ConsumptionRequest()));
            await Assert.ThrowsAsync<ArgumentNullException>(() => client.SendConsumptionInfoAsync("1000", null!));
        }

        [Fact]
        public async Task GetRefundHistoryAsync_PassesRevisionAndDecodes()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, Json(new { hasMore = false, revision = "x", signedTransactions = new[] { Jws.Unsigned(new { transactionId = "r" }) } }));
            var client = ClientFactory.Create(handler);

            var response = await client.GetRefundHistoryAsync("1000", "rev");

            Assert.Equal("r", Assert.Single(response.DecodedTransactions).TransactionId);
            Assert.Equal("/inApps/v2/refund/lookup/1000", handler.Requests[0].Request.RequestUri!.AbsolutePath);
            Assert.Equal("?revision=rev", handler.Requests[0].Request.RequestUri!.Query);
        }

        [Fact]
        public async Task LookUpOrderIdAsync_DecodesTransactions()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, Json(new { status = 1, signedTransactions = Array.Empty<string>() }));
            var client = ClientFactory.Create(handler);

            var response = await client.LookUpOrderIdAsync("MK5TTTVWJH");

            Assert.Equal(OrderLookupStatus.Invalid, response.Status);
            Assert.Equal("/inApps/v1/lookup/MK5TTTVWJH", handler.Requests[0].Request.RequestUri!.AbsolutePath);
        }

        [Fact]
        public async Task ExtendSubscriptionRenewalDateAsync_PutsRequest()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, Json(new { effectiveDate = 1698148900000, originalTransactionId = "1000", success = true, webOrderLineItemId = "w" }));
            var client = ClientFactory.Create(handler);

            var response = await client.ExtendSubscriptionRenewalDateAsync("1000", new ExtendRenewalDateRequest { ExtendByDays = 7, ExtendReasonCode = ExtendReasonCode.ServiceIssueOrOutage, RequestIdentifier = "req-1" });

            Assert.True(response.Success);
            Assert.Equal("/inApps/v1/subscriptions/extend/1000", handler.Requests[0].Request.RequestUri!.AbsolutePath);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(91)]
        public async Task ExtendSubscriptionRenewalDateAsync_RejectsOutOfRangeDays(int days)
        {
            var client = ClientFactory.Create(new FakeHttpMessageHandler());
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => client.ExtendSubscriptionRenewalDateAsync("1000", new ExtendRenewalDateRequest { ExtendByDays = days, RequestIdentifier = "r" }));
        }

        [Fact]
        public async Task TestNotification_RequestAndStatus()
        {
            var handler = new FakeHttpMessageHandler()
                .Enqueue(HttpStatusCode.OK, Json(new { testNotificationToken = "tok" }))
                .Enqueue(HttpStatusCode.OK, Json(new { signedPayload = "a.b.c", sendAttempts = new[] { new { attemptDate = 1698148900000, sendAttemptResult = "SUCCESS" } } }));
            var client = ClientFactory.Create(handler);

            var sent = await client.RequestTestNotificationAsync();
            var status = await client.GetTestNotificationStatusAsync(sent.TestNotificationToken);

            Assert.Equal(HttpMethod.Post, handler.Requests[0].Request.Method);
            Assert.Equal("/inApps/v1/notifications/test", handler.Requests[0].Request.RequestUri!.AbsolutePath);
            Assert.Equal("/inApps/v1/notifications/test/tok", handler.Requests[1].Request.RequestUri!.AbsolutePath);
            Assert.Equal("SUCCESS", Assert.Single(status.SendAttempts).SendAttemptResult);
        }

        [Fact]
        public async Task PathSegments_AreEscaped()
        {
            var handler = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, Json(new { status = 0, signedTransactions = Array.Empty<string>() }));
            var client = ClientFactory.Create(handler);

            await client.LookUpOrderIdAsync("../admin");

            Assert.Equal("/inApps/v1/lookup/..%2Fadmin", handler.Requests[0].Request.RequestUri!.AbsolutePath);
        }
    }
}
