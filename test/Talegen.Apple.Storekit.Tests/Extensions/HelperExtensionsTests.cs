namespace Talegen.Apple.Storekit.Tests.Extensions
{
    using Talegen.Apple.Storekit.Extensions;
    using Talegen.Apple.Storekit.Models.Api;
    using Talegen.Apple.Storekit.Tests.TestSupport;

    public class HelperExtensionsTests
    {
        [Theory]
        [InlineData(-1, AppleTimeUsed.Undeclared)]
        [InlineData(0, AppleTimeUsed.BetweenZeroAndFiveMinutes)]
        [InlineData(5, AppleTimeUsed.BetweenZeroAndFiveMinutes)]
        [InlineData(6, AppleTimeUsed.BetweenFiveAndSixtyMinutes)]
        [InlineData(60, AppleTimeUsed.BetweenFiveAndSixtyMinutes)]
        [InlineData(61, AppleTimeUsed.BetweenOneAndSixHours)]
        [InlineData(360, AppleTimeUsed.BetweenOneAndSixHours)]
        [InlineData(1440, AppleTimeUsed.BetweenSixAndTwentyFourHours)]
        [InlineData(1441, AppleTimeUsed.BetweenOneAndFourDays)]
        [InlineData(5760, AppleTimeUsed.BetweenOneAndFourDays)]
        [InlineData(5761, AppleTimeUsed.BetweenFourAndSixteenDays)]
        [InlineData(23040, AppleTimeUsed.BetweenFourAndSixteenDays)]
        [InlineData(23041, AppleTimeUsed.OverSixteenDays)]
        public void ToAppleTimeUsed_UsesAppleBuckets(int minutes, AppleTimeUsed expected)
        {
            Assert.Equal(expected, minutes.ToAppleTimeUsed());
        }

        [Theory]
        [InlineData("0", AppleDollarAmount.ZeroDollars)]
        [InlineData("0.01", AppleDollarAmount.BetweenOneCentAnd50Dollars)]
        [InlineData("49.99", AppleDollarAmount.BetweenOneCentAnd50Dollars)]
        [InlineData("49.995", AppleDollarAmount.BetweenOneCentAnd50Dollars)]
        [InlineData("50", AppleDollarAmount.Between50And100Dollars)]
        [InlineData("99.999", AppleDollarAmount.Between50And100Dollars)]
        [InlineData("100", AppleDollarAmount.Between100And500Dollars)]
        [InlineData("500", AppleDollarAmount.Between500And1000Dollars)]
        [InlineData("1000", AppleDollarAmount.Between1000And2000Dollars)]
        [InlineData("1999.99", AppleDollarAmount.Between1000And2000Dollars)]
        [InlineData("2000", AppleDollarAmount.Over2000Dollars)]
        [InlineData("-75", AppleDollarAmount.Between50And100Dollars)]
        public void ToAppleDollarAmount_UsesAppleBuckets(string amount, AppleDollarAmount expected)
        {
            Assert.Equal(expected, decimal.Parse(amount, System.Globalization.CultureInfo.InvariantCulture).ToAppleDollarAmount());
        }

        [Theory]
        [InlineData(0, AppleAccountTenure.BetweenZeroAndThreeDays)]
        [InlineData(3, AppleAccountTenure.BetweenZeroAndThreeDays)]
        [InlineData(4, AppleAccountTenure.BetweenThreeAndTenDays)]
        [InlineData(30, AppleAccountTenure.BetweenTenAndThirtyDays)]
        [InlineData(90, AppleAccountTenure.BetweenThirtyAndNinetyDays)]
        [InlineData(180, AppleAccountTenure.BetweenNinetyAndOneHundredEightyDays)]
        [InlineData(365, AppleAccountTenure.BetweenOneHundredEightyAndThreeHundredSixtyFiveDays)]
        [InlineData(366, AppleAccountTenure.OverThreeHundredSixtyFiveDays)]
        public void ToAppleAccountTenure_UsesAppleBuckets(int ageInDays, AppleAccountTenure expected)
        {
            var now = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
            Assert.Equal(expected, now.AddDays(-ageInDays).ToAppleAccountTenure(now));
        }

        [Fact]
        public void TransactionHistoryRequest_ToQueryParameters_UsesAppleNames()
        {
            var request = new TransactionHistoryRequest
            {
                Revision = "rev1",
                StartDate = 1698148800000,
                EndDate = 1698148900000,
                ProductId = "com.example.product",
                ProductType = "AUTO_RENEWABLE",
                InAppOwnershipType = "PURCHASED",
                Sort = "DESCENDING",
                Revoked = false,
                SubscriptionGroupIdentifier = "group",
            };

            var parameters = request.ToQueryParameters();

            Assert.Equal("rev1", parameters["revision"]);
            Assert.Equal("1698148800000", parameters["startDate"]);
            Assert.Equal("1698148900000", parameters["endDate"]);
            Assert.Equal("com.example.product", parameters["productId"]);
            Assert.Equal("AUTO_RENEWABLE", parameters["productType"]);
            Assert.Equal("PURCHASED", parameters["inAppOwnershipType"]);
            Assert.Equal("DESCENDING", parameters["sort"]);
            Assert.Equal("false", parameters["revoked"]);
            Assert.Equal("group", parameters["subscriptionGroupIdentifier"]);
        }

        [Fact]
        public void TransactionHistoryRequest_ToQueryParameters_OmitsUnsetValues()
        {
            Assert.Empty(new TransactionHistoryRequest().ToQueryParameters());
        }

        [Fact]
        public void DecodeJwtPayload_DecodesPayloadSegment()
        {
            string jws = Jws.Unsigned(new { transactionId = "123", productId = "p", price = 990 });
            var transaction = jws.DecodeJwtPayload<JwsTransactionDecodedPayload>();

            Assert.Equal("123", transaction.TransactionId);
            Assert.Equal(990, transaction.Price);
        }

        [Theory]
        [InlineData("not-a-jws")]
        [InlineData("a.b")]
        [InlineData("a.!!!.c")]
        public void DecodeJwtPayload_InvalidInput_Throws(string value)
        {
            Assert.ThrowsAny<ArgumentException>(() => value.DecodeJwtPayload<JwsTransactionDecodedPayload>());
        }

        [Fact]
        public void DecodeUnverified_DecodesNotificationBody()
        {
            var body = new AppleResponseBodyV2 { SignedPayload = Jws.Unsigned(new { notificationType = "REFUND", notificationUUID = Guid.Empty }) };
            Assert.Equal(AppleStoreNotificationType.Refund, body.DecodeUnverified().NotificationType);
        }

        [Fact]
        public void LastTransactionsItem_DecodesSignedFields()
        {
            var item = new LastTransactionsItem
            {
                SignedTransactionInfo = Jws.Unsigned(new { transactionId = "t1" }),
                SignedRenewalInfo = Jws.Unsigned(new { autoRenewStatus = 0, signedDate = 0 }),
            };

            Assert.Equal("t1", item.TransactionInfoDecodedPayload.TransactionId);
            Assert.Equal(AppleAutoRenewStatuses.AutomaticRenewalOff, item.RenewalInfoDecodedPayload.AutoRenewStatus);
        }

        [Fact]
        public void ToUtcDateTime_HandlesNull()
        {
            Assert.Null(((long?)null).ToUtcDateTime());
            Assert.Equal(DateTime.UnixEpoch, ((long?)0).ToUtcDateTime());
        }
    }
}
