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
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// This enumeration represents the auto-renew status.
    /// </summary>
    public enum AppleAutoRenewStatuses
    {
        /// <summary>
        /// Automatic renewal is off. 
        /// The customer has turned off automatic renewal for the subscription, and it won’t renew at the end of the current subscription period.
        /// </summary>
        AutomaticRenewalOff = 0,

        /// <summary>
        /// Automatic renewal is on. 
        /// The subscription renews at the end of the current subscription period.
        /// </summary>
        AutomaticRenewalOn = 1
    }

    public enum AppleExpirationIntent
    {
        /// <summary>
        /// The customer canceled their subscription.
        /// </summary>
        CustomerCanceled = 1,

        /// <summary>
        /// The subscription was canceled by the App Store because the customer’s payment was declined.
        /// </summary>
        BillingError = 2,

        /// <summary>
        /// The customer declined a price increase.
        /// </summary>
        CustomerDeclinedPriceIncrease = 3,

        /// <summary>
        /// The product is not available for purchase.
        /// </summary>
        ProductNotAvailable = 4,

        /// <summary>
        /// Unknown expiration intent.
        /// </summary>
        Unknown = 5
    }

    /// <summary>
    /// This enumeration represents the Apple price increase statuses.
    /// </summary>
    public enum ApplePriceIncreaseStatus
    {
        /// <summary>
        /// The customer hasn't yet responded to an auto-renewable 
        /// subscription price increase that requires customer consent.
        /// </summary>
        CustomerNotResponded = 0,

        /// <summary>
        /// The customer consented to an auto-renewable subscription price 
        /// increase that requires customer consent, or the App Store has 
        /// notified the customer of an auto-renewable subscription price 
        /// increase that doesn't require consent.
        /// </summary>
        CustomerConsented = 1,
    }

    /// <summary>
    /// This enumeration represents the renewal Info payload.
    /// </summary>
    public class JwsRenewalInfoDecodedPayload
    {
        /// <summary>
        /// Gets or sets the auto-renew product identifier.
        /// </summary>
        [JsonPropertyName("autoRenewProductId")]
        public string AutoRenewProductId { get; set; }

        /// <summary>
        /// Gets or sets the auto-renew status.
        /// </summary>
        [JsonPropertyName("autoRenewStatus")]
        public AppleAutoRenewStatuses AutoRenewStatus { get; set; }

        /// <summary>
        /// Gets or sets the currency code for the renewalPrice of the subscription. 
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets a list of win-back offer IDs that the customer is eligible for.
        /// </summary>
        [JsonPropertyName("eligibleWinBackOfferIds")]
        public List<string> EligibleWinBackOfferIds { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the server environment, either sandbox or production.
        /// </summary>
        [JsonPropertyName("environment")]
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets the reason the subscription expired.
        /// </summary>
        [JsonPropertyName("expirationIntent")]
        public AppleExpirationIntent? ExpirationIntent { get; set; }

        /// <summary>
        /// Gets or sets the grace period expires date.
        /// </summary>
        [JsonPropertyName("gracePeriodExpiresDate")]
        public long GracePeriodExpiresDate { get; set; }

        /// <summary>
        /// Gets the grace period expires date time.
        /// </summary>
        [JsonIgnore]
        public DateTime GracePeriodExpiresDateTime => DateTimeOffset.FromUnixTimeMilliseconds(this.GracePeriodExpiresDate).DateTime;

        /// <summary>
        /// Gets or sets a value that indicates whether the App Store is attempting to automatically renew an expired subscription.
        /// </summary>
        [JsonPropertyName("isInBillingRetryPeriod")]
        public bool IsInBillingRetryPeriod { get; set; }

        /// <summary>
        /// Gets or sets the payment mode for subscription offers on an auto-renewable subscription.
        /// </summary>
        [JsonPropertyName("offerDiscountType")]
        public AppleOfferDiscountType? OfferDiscountType { get; set; }

        /// <summary>
        /// Gets or sets the offer code or the promotional offer identifier.
        /// </summary>
        [JsonPropertyName("offerIdentifier")]
        public string? OfferIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the type of subscription offer.
        /// </summary>
        [JsonPropertyName("offerType")]
        public AppleOfferType? OfferType { get; set; }

        /// <summary>
        /// Gets or sets the transaction identifier of the original purchase associated with this transaction.
        /// </summary>
        [JsonPropertyName("originalTransactionId")]
        public string OriginalTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the status that indicates whether an auto-renewable 
        /// subscription is subject to a price increase.
        /// </summary>
        [JsonPropertyName("priceIncreaseStatus")]
        public ApplePriceIncreaseStatus PriceIncreaseStatus { get; set; }

        /// <summary>
        /// Gets or sets the product identifier of the In-App Purchase.
        /// </summary>
        [JsonPropertyName("productId")]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the earliest start date of the auto-renewable subscription 
        /// in a series of subscription purchases that ignores all lapses of paid 
        /// service that are 60 days or fewer.
        /// </summary>
        [JsonPropertyName("recentSubscriptionStartDate")]
        public long RecentSubscriptionStartDate { get; set; }

        /// <summary>
        /// Gets the recent subscription start date time.
        /// </summary>
        [JsonIgnore]
        public DateTime RecentSubscriptionStartDateTime => DateTimeOffset.FromUnixTimeMilliseconds(this.RecentSubscriptionStartDate).DateTime;

        /// <summary>
        /// Gets or sets the UNIX time, in milliseconds, when the most recent 
        /// auto-renewable subscription purchase expires. 
        /// </summary>
        [JsonPropertyName("renewalDate")]
        public long RenewalDate { get; set; }

        public DateTime RenewalDateTime => DateTimeOffset.FromUnixTimeMilliseconds(this.RenewalDate).DateTime;

        /// <summary>
        /// Gets or sets the renewal price, in milliunits, of the 
        /// auto-renewable subscription that renews at the next billing period.
        /// </summary>
        [JsonPropertyName("renewalPrice")]
        public long RenewalPrice { get; set; }

        /// <summary>
        /// Gets or sets the UNIX time, in milliseconds,  that the App Store signed the JSON Web Signature (JWS) data.
        /// </summary>
        [JsonPropertyName("signedDate")]
        public long SignedDate { get; set; }

        /// <summary>
        /// Gets the signed date time.
        /// </summary>
        [JsonIgnore]
        public DateTime SignedDateTime => DateTimeOffset.FromUnixTimeMilliseconds(this.SignedDate).DateTime;
    }
}
