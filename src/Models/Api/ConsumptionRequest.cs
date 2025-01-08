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
namespace Talegen.Apple.Storekit.Models.Api
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// This enumeration represents the Apple account tenure.
    /// </summary>
    public enum AppleAccountTenure
    {
        Undeclared = 0,
        BetweenZeroAndThreeDays = 1,
        BetweenThreeAndTenDays = 2,
        BetweenTenAndThirtyDays = 3,
        BetweenThirtyAndNinetyDays = 4,
        BetweenNinetyAndOneHundredEightyDays = 5,
        BetweenOneHundredEightyAndThreeHundredSixtyFiveDays = 6,
        OverThreeHundredSixtyFiveDays = 7,
    }

    /// <summary>
    /// This enumeration represents the Apple consumption status.
    /// </summary>
    public enum AppleConsumptionStatus
    {
        /// <summary>
        /// The status is undeclared.
        /// </summary>
        Undeclared = 0,

        /// <summary>
        /// The consumable was not consumed.
        /// </summary>
        NotConsumed = 1,

        /// <summary>
        /// The consumable was partially consumed.
        /// </summary>
        PartiallyConsumed = 2,

        /// <summary>
        /// The consumable was fully consumed.
        /// </summary>
        FullyConsumed = 3,
    }

    /// <summary>
    /// This enumeration represents the Apple delivery status.
    /// </summary>
    public enum AppleDeliveryStatus
    {
        /// <summary>
        /// The app delivered the consumable in-app purchase and it’s working properly.
        /// </summary>
        DeliveredAndWorking = 0,

        /// <summary>
        /// The app didn't deliver the consumable in-app purchase due to a quality issue.
        /// </summary>
        NotDeliveredDueToQualityIssue = 1,

        /// <summary>
        /// The app delivered the wrong consumable in-app purchase.
        /// </summary>
        DeliveredWrongItem = 2,

        /// <summary>
        /// The app didn't deliver the consumable in-app purchase due to a server outage.
        /// </summary>
        NotDeliveredDueToServerOutage = 3,

        /// <summary>
        /// The app didn't deliver the consumable in-app purchase due to a currency change.
        /// </summary>
        NotDeliveredDueToCurrencyChange = 4,

        /// <summary>
        /// The app didn't deliver the consumable in-app purchase due to another reason.
        /// </summary>
        NotDeliveredDueToOtherReason = 5,
    }

    public enum AppleDollarAmount
    {
        /// <summary>
        /// Amount is undeclared. 
        /// Use this value to avoid providing information for this field.
        /// </summary>
        Undeclared = 0,

        /// <summary>
        /// Amount is 0 USD.
        /// </summary>
        ZeroDollars = 1,

        /// <summary>
        /// Amount is between 0.01–49.99 USD.
        /// </summary>
        BetweenOneCentAnd50Dollars = 2,

        /// <summary>
        /// Amount is between 50–99.99 USD.
        /// </summary>
        Between50And100Dollars = 3,

        /// <summary>
        /// Amount is between 100–499.99 USD.
        /// </summary>
        Between100And500Dollars = 4,

        /// <summary>
        /// Amount is between 500–999.99 USD.
        /// </summary>
        Between500And1000Dollars = 5,

        /// <summary>
        /// Amount is between 1000–1999.99 USD.
        /// </summary>
        Between1000And2000Dollars = 6,

        /// <summary>
        /// Amount is over 2000 USD.
        /// </summary>
        Over2000Dollars = 7,
    }

    public enum PlatformType
    {
        /// <summary>
        /// The platform is undeclared. Use this value to avoid providing information for this field.
        /// </summary>
        Undeclared = 0,

        /// <summary>
        /// The platform is Apple.
        /// </summary>
        Apple = 1,

        /// <summary>
        /// The platform is non-Apple.
        /// </summary>
        NonApple = 2,
    }

    public enum AppleRefundPreferenceType
    {
        /// <summary>
        /// The refund preference is undeclared. 
        /// Use this value to avoid providing information for this field.
        /// </summary>
        Undeclared = 0,

        /// <summary>
        /// You prefer that Apple grants the refund.
        /// </summary>
        GrantRefund = 1,

        /// <summary>
        /// You prefer that Apple declines the refund.
        /// </summary>
        DeclineRefund = 2,

        /// <summary>
        /// You have no preference for the refund.
        /// </summary>
        NoPreference = 3,
    }

    /// <summary>
    /// This enumeration represents the Apple time used.
    /// </summary>
    public enum AppleTimeUsed
    {
        Undeclared = 0,

        BetweenZeroAndFiveMinutes = 1,

        BetweenFiveAndSixtyMinutes = 2,

        BetweenOneAndSixHours = 3,

        BetweenSixAndTwentyFourHours = 4,

        BetweenOneAndFourDays = 5,

        BetweenFourAndSixteenDays = 6,

        OverSixteenDays = 7,
    }

    /// <summary>
    /// This enumeration represents the account status.
    /// </summary>
    public enum AppleAccountStatus
    {
        /// <summary>
        /// The account status is undeclared.
        /// </summary>
        Undeclared = 0,
        
        /// <summary>
        /// The account is active.
        /// </summary>
        Active = 1,
        
        /// <summary>
        /// The account is suspended.
        /// </summary>
        Suspended = 2,

        /// <summary>
        /// The account is terminated.
        /// </summary>
        Terminated = 3,

        /// <summary>
        /// The account is limited access.
        /// </summary>
        Limited = 4,
    }

    /// <summary>
    /// This class represents the consumption request model.
    /// </summary>
    public class ConsumptionRequest
    {
        /// <summary>
        /// Gets or sets the age of the customer’s account
        /// </summary>
        [JsonPropertyName("accountTenure")]
        public AppleAccountTenure AccountTenure { get; set; }

        /// <summary>
        /// Gets or sets the UUID of the in-app user account that completed 
        /// the in-app purchase transaction.
        /// </summary>
        [JsonPropertyName("appAccountToken")]
        public Guid AppAccountToken { get; set; }

        /// <summary>
        /// Gets or sets the extent to which the customer consumed the in-app purchase.
        /// </summary>
        [JsonPropertyName("consumptionStatus")]
        public AppleConsumptionStatus ConsumptionStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicates whether the customer consented to 
        /// provide consumption data.
        /// </summary>
        /// <remarks>
        /// Note: The App Store server rejects requests that have a customerConsented 
        /// value other than true by returning an HTTP 400 error 
        /// with an InvalidCustomerConsentError.
        /// </remarks>
        [JsonPropertyName("customerConsented")]
        public bool CustomerConsented { get; set; } = true;

        /// <summary>
        /// Gets or sets the delivery status.
        /// </summary>
        [JsonPropertyName("deliveryStatus")]
        public AppleDeliveryStatus DeliveryStatus { get; set; }

        /// <summary>
        /// Gets or sets the total amount, in USD, of in-app purchases the 
        /// customer has made in your app, across all platforms.
        /// </summary>
        [JsonPropertyName("lifetimeDollarsPurchased")]
        public AppleDollarAmount LifetimeDollarsPurchased { get; set; }

        /// <summary>
        /// Gets or sets the total amount, in USD, of refunds the customer 
        /// has received, in your app, across all platforms.
        /// </summary>
        [JsonPropertyName("lifetimeDollarsRefunded")]
        public AppleDollarAmount LifetimeDollarsRefunded { get; set; }

        /// <summary>
        /// Gets or sets the platform on which the customer consumed the in-app purchase
        /// </summary>
        [JsonPropertyName("platform")]
        public PlatformType Platform { get; set; }

        /// <summary>
        /// Gets or sets the amount of time that the customer used the app.
        /// </summary>
        [JsonPropertyName("playTime")]
        public AppleTimeUsed PlayTime { get; set; }

        /// <summary>
        /// Gets or sets your preference, based on your operational logic, 
        /// as to whether Apple should grant the refund.
        /// </summary>
        [JsonPropertyName("refundPreference")]
        public AppleRefundPreferenceType RefundPreference { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether you provided, 
        /// prior to its purchase, a free sample or trial of the content, 
        /// or information about its functionality.
        /// </summary>
        [JsonPropertyName("sampleContentProvided")]
        public bool SampleContentProvided { get; set; }

        /// <summary>
        /// Gets or sets the status of the customer’s account in your app.
        /// </summary>
        [JsonPropertyName("userStatus")]
        public AppleAccountStatus UserStatus { get; set; }
    }
}
