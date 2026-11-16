namespace Talegen.Apple.Storekit.Tests.Models
{
    using System.Text.Json;
    using Talegen.Apple.Storekit.Client;
    using Talegen.Apple.Storekit.Models.Api;

    public class ModelSerializationTests
    {
        private static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, AppStoreServerApiClient.JsonOptions)!;

        [Theory]
        [InlineData("DID_CHANGE_RENEWAL_PREF", AppleStoreNotificationType.ChangeRenewalPreference)]
        [InlineData("DID_CHANGE_RENEWAL_STATUS", AppleStoreNotificationType.ChangeRenewalStatus)]
        [InlineData("DID_RENEW", AppleStoreNotificationType.Renewed)]
        [InlineData("DID_FAIL_TO_RENEW", AppleStoreNotificationType.FailedToRenew)]
        [InlineData("GRACE_PERIOD_EXPIRED", AppleStoreNotificationType.GracePeriodExpired)]
        [InlineData("CONSUMPTION_REQUEST", AppleStoreNotificationType.ConsumptionRequest)]
        [InlineData("ONE_TIME_CHARGE", AppleStoreNotificationType.OneTimeCharge)]
        [InlineData("SUBSCRIBED", AppleStoreNotificationType.Subscribed)]
        [InlineData("TEST", AppleStoreNotificationType.Test)]
        public void NotificationType_ReadsAppleWireValues(string wireValue, AppleStoreNotificationType expected)
        {
            var payload = Deserialize<AppleResponseBodyV2DecodedPayload>($"{{\"notificationType\":\"{wireValue}\"}}");
            Assert.Equal(expected, payload.NotificationType);
        }

        [Fact]
        public void NotificationType_UnrecognizedValue_MapsToUnknown()
        {
            var payload = Deserialize<AppleResponseBodyV2DecodedPayload>("{\"notificationType\":\"SOME_FUTURE_TYPE\",\"subtype\":\"SOME_FUTURE_SUBTYPE\"}");
            Assert.Equal(AppleStoreNotificationType.Unknown, payload.NotificationType);
            Assert.Equal(AppleStoreSubNotificationType.Unknown, payload.SubNotificationType);
        }

        [Fact]
        public void NotificationSubtype_IsNull_WhenAbsent()
        {
            var payload = Deserialize<AppleResponseBodyV2DecodedPayload>("{\"notificationType\":\"DID_RENEW\"}");
            Assert.Null(payload.SubNotificationType);
        }

        [Theory]
        [InlineData("INITIAL_BUY", AppleStoreSubNotificationType.InitialBuy)]
        [InlineData("AUTO_RENEW_DISABLED", AppleStoreSubNotificationType.AutoRenewDisabled)]
        [InlineData("BILLING_RECOVERY", AppleStoreSubNotificationType.BillingRecovery)]
        [InlineData("PRODUCT_NOT_FOR_SALE", AppleStoreSubNotificationType.ProductNotForSale)]
        public void NotificationSubtype_ReadsAppleWireValues(string wireValue, AppleStoreSubNotificationType expected)
        {
            var payload = Deserialize<AppleResponseBodyV2DecodedPayload>($"{{\"notificationType\":\"SUBSCRIBED\",\"subtype\":\"{wireValue}\"}}");
            Assert.Equal(expected, payload.SubNotificationType);
        }

        [Fact]
        public void NotificationType_WritesAppleWireValue()
        {
            string json = JsonSerializer.Serialize(new AppleResponseBodyV2DecodedPayload
            {
                NotificationType = AppleStoreNotificationType.ChangeRenewalPreference,
                SubNotificationType = AppleStoreSubNotificationType.AutoRenewEnabled,
            });

            Assert.Contains("\"notificationType\":\"DID_CHANGE_RENEWAL_PREF\"", json);
            Assert.Contains("\"subtype\":\"AUTO_RENEW_ENABLED\"", json);
        }

        [Fact]
        public void Notification_DecodesFullDataPayload()
        {
            const string json = """
            {
              "notificationType": "SUBSCRIBED",
              "subtype": "INITIAL_BUY",
              "notificationUUID": "002e14d5-51f5-4503-b5a8-c3a1af68eb20",
              "version": "2.0",
              "signedDate": 1700000000000,
              "data": {
                "appAppleId": 1234567890,
                "bundleId": "com.example.app",
                "bundleVersion": "42",
                "environment": "Production",
                "signedTransactionInfo": "a.b.c",
                "signedRenewalInfo": "d.e.f",
                "status": 1
              }
            }
            """;

            var payload = Deserialize<AppleResponseBodyV2DecodedPayload>(json);

            Assert.Equal(Guid.Parse("002e14d5-51f5-4503-b5a8-c3a1af68eb20"), payload.NotificationIdentifier);
            Assert.Equal(1234567890L, payload.Data!.AppAppleId);
            Assert.Equal(ApplePayloadMetadataStatusType.AutoRenewableSubscriptionActive, payload.Data.Status);
            Assert.Equal(new DateTime(2023, 11, 14, 22, 13, 20, DateTimeKind.Utc), payload.SignedDateUtc);
            Assert.Equal(DateTimeKind.Utc, payload.SignedDateUtc!.Value.Kind);
        }

        [Fact]
        public void NotificationMetadata_StatusAndAppAppleIdAreNull_WhenAbsent()
        {
            var payload = Deserialize<AppleResponseBodyV2DecodedPayload>("{\"notificationType\":\"ONE_TIME_CHARGE\",\"data\":{\"bundleId\":\"com.example.app\",\"environment\":\"Sandbox\"}}");
            Assert.Null(payload.Data!.Status);
            Assert.Null(payload.Data.AppAppleId);
        }

        [Fact]
        public void SubscriptionStatusResponse_ReadsNumericAppAppleId()
        {
            const string json = """
            {
              "environment": "Production",
              "bundleId": "com.example.app",
              "appAppleId": 5454545454,
              "data": [
                {
                  "subscriptionGroupIdentifier": "sub_group_one",
                  "lastTransactions": [
                    { "status": 4, "originalTransactionId": "3749183", "signedTransactionInfo": "", "signedRenewalInfo": "" }
                  ]
                }
              ]
            }
            """;

            var response = Deserialize<SubscriptionStatusResponse>(json);

            Assert.Equal(5454545454L, response.AppAppleId);
            var item = Assert.Single(Assert.Single(response.Data).LastTransactions);
            Assert.Equal(ApplePayloadMetadataStatusType.AutoRenewableSubscriptionInGracePeriod, item.Status);
            Assert.Null(item.TransactionInfoDecodedPayload);
            Assert.Null(item.RenewalInfoDecodedPayload);
        }

        [Fact]
        public void TransactionHistoryResponse_ReadsNumericAppAppleId()
        {
            var response = Deserialize<TransactionHistoryResponse>("{\"appAppleId\":323232,\"bundleId\":\"com.example.app\",\"environment\":\"Production\",\"hasMore\":true,\"revision\":\"rev\",\"signedTransactions\":[]}");
            Assert.Equal(323232L, response.AppAppleId);
            Assert.True(response.HasMore);
        }

        [Fact]
        public void TransactionPayload_DecodesAllFields()
        {
            const string json = """
            {
              "transactionId": "23456",
              "originalTransactionId": "12345",
              "webOrderLineItemId": "34343",
              "bundleId": "com.example.app",
              "productId": "com.example.product",
              "subscriptionGroupIdentifier": "55555",
              "purchaseDate": 1698148900000,
              "originalPurchaseDate": 1698148800000,
              "expiresDate": 1698149000000,
              "quantity": 1,
              "type": "Auto-Renewable Subscription",
              "appAccountToken": "7e3fb20b-4cdb-47cc-936d-99d65f608138",
              "inAppOwnershipType": "PURCHASED",
              "signedDate": 1698148900000,
              "revocationReason": 1,
              "revocationDate": 1698148950000,
              "isUpgraded": true,
              "offerType": 1,
              "offerIdentifier": "abc.123",
              "environment": "LocalTesting",
              "storefront": "USA",
              "storefrontId": "143441",
              "transactionReason": "PURCHASE",
              "currency": "USD",
              "price": 10990,
              "offerDiscountType": "PAY_AS_YOU_GO",
              "offerPeriod": "P1M",
              "appTransactionId": "71134"
            }
            """;

            var transaction = Deserialize<JwsTransactionDecodedPayload>(json);

            Assert.Equal(AppleOfferType.Introductory, transaction.OfferType);
            Assert.Equal(AppleOfferDiscountType.PayAsYouGo, transaction.OfferDiscountType);
            Assert.Equal(AppleTransactionReason.Purchase, transaction.TransactionReason);
            Assert.Equal(AppleRevocationReason.RefundedDueToIssue, transaction.RevocationReason);
            Assert.Equal(10990L, transaction.Price);
            Assert.Equal("P1M", transaction.OfferPeriod);
            Assert.Equal("71134", transaction.AppTransactionId);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1698149000000).UtcDateTime, transaction.ExpiresDateUtc);
            Assert.Equal(DateTimeKind.Utc, transaction.PurchaseDateUtc!.Value.Kind);
        }

        [Theory]
        [InlineData("FREE_TRIAL", AppleOfferDiscountType.FreeTrial)]
        [InlineData("PAY_UP_FRONT", AppleOfferDiscountType.PayUpFront)]
        [InlineData("SOMETHING_NEW", AppleOfferDiscountType.Unknown)]
        public void OfferDiscountType_ReadsNullableProperty(string wireValue, AppleOfferDiscountType expected)
        {
            var transaction = Deserialize<JwsTransactionDecodedPayload>($"{{\"offerDiscountType\":\"{wireValue}\"}}");
            Assert.Equal(expected, transaction.OfferDiscountType);
        }

        [Fact]
        public void OfferDiscountType_NullJson_IsNull()
        {
            var transaction = Deserialize<JwsTransactionDecodedPayload>("{\"offerDiscountType\":null}");
            Assert.Null(transaction.OfferDiscountType);
        }

        [Theory]
        [InlineData(1, AppleOfferType.Introductory)]
        [InlineData(2, AppleOfferType.Promotional)]
        [InlineData(3, AppleOfferType.OfferWithOfferCode)]
        [InlineData(4, AppleOfferType.WinBack)]
        public void OfferType_MatchesAppleNumbering(int wireValue, AppleOfferType expected)
        {
            Assert.Equal(expected, Deserialize<JwsTransactionDecodedPayload>($"{{\"offerType\":{wireValue}}}").OfferType);
        }

        [Fact]
        public void RenewalInfo_OptionalDatesAreNull_WhenAbsent()
        {
            var renewal = Deserialize<JwsRenewalInfoDecodedPayload>("{\"autoRenewStatus\":1,\"signedDate\":1698148800000,\"environment\":\"Sandbox\"}");

            Assert.Equal(AppleAutoRenewStatuses.AutomaticRenewalOn, renewal.AutoRenewStatus);
            Assert.Null(renewal.GracePeriodExpiresDate);
            Assert.Null(renewal.GracePeriodExpiresDateTime);
            Assert.Null(renewal.RenewalDateTime);
            Assert.Null(renewal.PriceIncreaseStatus);
            Assert.Null(renewal.RenewalPrice);
            Assert.Equal(DateTimeKind.Utc, renewal.SignedDateTime.Kind);
        }

        [Fact]
        public void RenewalInfo_DecodesPresentFields()
        {
            const string json = """
            {
              "expirationIntent": 1,
              "originalTransactionId": "12345",
              "autoRenewProductId": "com.example.product.2",
              "productId": "com.example.product",
              "autoRenewStatus": 1,
              "isInBillingRetryPeriod": true,
              "priceIncreaseStatus": 0,
              "gracePeriodExpiresDate": 1698148900000,
              "offerType": 2,
              "offerIdentifier": "abc.123",
              "signedDate": 1698148800000,
              "environment": "LocalTesting",
              "recentSubscriptionStartDate": 1698148800000,
              "renewalDate": 1698148850000,
              "currency": "USD",
              "renewalPrice": 9990,
              "offerDiscountType": "PAY_AS_YOU_GO",
              "eligibleWinBackOfferIds": ["eligible1", "eligible2"]
            }
            """;

            var renewal = Deserialize<JwsRenewalInfoDecodedPayload>(json);

            Assert.Equal(AppleExpirationIntent.CustomerCanceled, renewal.ExpirationIntent);
            Assert.Equal(ApplePriceIncreaseStatus.CustomerNotResponded, renewal.PriceIncreaseStatus);
            Assert.Equal(AppleOfferType.Promotional, renewal.OfferType);
            Assert.Equal(9990L, renewal.RenewalPrice);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1698148900000).UtcDateTime, renewal.GracePeriodExpiresDateTime);
            Assert.Equal(2, renewal.EligibleWinBackOfferIds.Count);
        }

        [Fact]
        public void AppleApiError_ReadsErrorCodeAndMessage()
        {
            var error = Deserialize<AppleApiError>("{\"errorCode\":4040010,\"errorMessage\":\"Transaction id not found.\"}");
            Assert.Equal(AppleErrorCodes.TransactionIdNotFound, error.ErrorCode);
            Assert.Equal("Transaction id not found.", error.ErrorMessage);
        }

        [Fact]
        public void ConsumptionRequest_SerializesAsAppleExpects()
        {
            var request = new ConsumptionRequest
            {
                AccountTenure = AppleAccountTenure.BetweenTenAndThirtyDays,
                AppAccountToken = Guid.Parse("7389a31a-fb6d-4569-a2a6-db7d85d84813"),
                ConsumptionStatus = AppleConsumptionStatus.PartiallyConsumed,
                DeliveryStatus = AppleDeliveryStatus.DeliveredAndWorking,
                LifetimeDollarsPurchased = AppleDollarAmount.Between100And500Dollars,
                Platform = PlatformType.Apple,
                PlayTime = AppleTimeUsed.BetweenOneAndSixHours,
                RefundPreference = AppleRefundPreferenceType.DeclineRefund,
                SampleContentProvided = true,
                UserStatus = AppleAccountStatus.Active,
            };

            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(request, AppStoreServerApiClient.JsonOptions));
            var root = doc.RootElement;

            Assert.Equal(3, root.GetProperty("accountTenure").GetInt32());
            Assert.Equal("7389a31a-fb6d-4569-a2a6-db7d85d84813", root.GetProperty("appAccountToken").GetString());
            Assert.True(root.GetProperty("customerConsented").GetBoolean());
            Assert.Equal(4, root.GetProperty("lifetimeDollarsPurchased").GetInt32());
            Assert.Equal(2, root.GetProperty("refundPreference").GetInt32());
        }
    }
}
