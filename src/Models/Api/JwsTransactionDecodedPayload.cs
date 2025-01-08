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
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    /// <summary>
    /// An enumerated list of Apple Offer Discount Types.
    /// </summary>
    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AppleOfferDiscountType
    {
        [EnumMember(Value = "FREE_TRIAL")]
        FreeTrial,

        [EnumMember(Value = "PAY_AS_YOU_GO")]
        PayAsYouGo,

        [EnumMember(Value = "PAY_UP_FRONT")]
        PayUpFront
    }

    /// <summary>
    /// An enumerated list of Apple Offer Types.
    /// </summary>
    public enum AppleOfferType
    {
        /// <summary>
        /// The introductory offer for a subscription.
        /// </summary>
        Introductory = 0,

        /// <summary>
        /// The promotional offer for a subscription.
        /// </summary>
        Promotional = 2,

        /// <summary>
        /// The offer with an offer code for a subscription.
        /// </summary>
        OfferWithOfferCode = 3,

        /// <summary>
        /// The win-back offer for a subscription.
        /// </summary>
        WinBack = 4
    }

    /// <summary>
    /// An enumerated list of Apple Transaction Reasons.
    /// </summary>
    public enum AppleRevocationReason
    {
        /// <summary>
        /// The App Store refunded the transaction on behalf of the customer for other 
        /// reasons, for example, an accidental purchase.
        /// </summary>
        RefundedForOtherReasons = 0,

        /// <summary>
        /// The App Store refunded the transaction on behalf of the customer due to an
        /// </summary>
        RefundedDueToIssue = 1
    }

    /// <summary>
    /// An enumerated list of Apple Transaction Reasons.
    /// </summary>
    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AppleTransactionReason
    {
        /// <summary>
        /// The customer initiated the purchase, which may be for any in-app purchase type: 
        /// consumable, non-consumable, non-renewing subscription, or auto-renewable subscription.
        /// </summary>
        [EnumMember(Value = "PURCHASE")]
        Purchase,

        /// <summary>
        /// The App Store server initiated the purchase transaction to renew an auto-renewable subscription.
        /// </summary>
        [EnumMember(Value = "RENEWAL")]
        Renewal
    }

    /// <summary>
    /// This class represents the Apple Transaction Details.
    /// </summary>
    public class JwsTransactionDecodedPayload
    {
        /// <summary>
        /// Gets or sets the The UUID that an app optionally generates to 
        /// map a customer’s In-App Purchase with its resulting App Store transaction.
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/appaccounttoken"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("appAccountToken")]
        public string AppAccountToken { get; set; }

        /// <summary>
        /// Gets or sets the bundle id.
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/bundleid"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("bundleId")]
        public string BundleId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the three-letter ISO 4217 currency code for the price of the product.
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/currency"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the server environment, either sandbox or production. <see href="https://developer.apple.com/documentation/appstoreserverapi/environment"/>
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("environment")]
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets the UNIX time, in milliseconds, that the subscription expires or renews. <see href="https://developer.apple.com/documentation/appstoreserverapi/expiresdate"/>
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("expiresDate")]
        public long? ExpiresDate { get; set; }

        /// <summary>
        /// Gets or set s string that describes whether the transaction was 
        /// purchased by the customer, or is available to them through Family Sharing. 
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/inappownershiptype"/>
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("inAppOwnershipType")]
        public string InAppOwnershipType { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the customer upgraded to another subscription.
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/isupgraded"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("isUpgraded")]
        public bool IsUpgraded { get; set; }

        /// <summary>
        /// Gets or sets the payment mode for subscription offers on an auto-renewable subscription.
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/offerdiscounttype"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("offerDiscountType")]
        public AppleOfferDiscountType? OfferDiscountType { get; set; }

        /// <summary>
        /// Gets or sets the identifier that contains the offer code or the promotional offer identifier.
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/offeridentifier"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("offerIdentifier")]
        public string OfferIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the type of offer that was applied to the subscription.
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/offertype"/>
        /// </remarks> 
        [System.Text.Json.Serialization.JsonPropertyName("offerType")]
        public AppleOfferType? OfferType { get; set; }

        /// <summary>
        /// Gets or sets the UNIX time, in milliseconds, that represents the purchase 
        /// date of the original transaction identifier.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("originalPurchaseDate")]
        public long OriginalPurchaseDate { get; set; }

        /// <summary>
        /// Gets or sets the transaction identifier of the original purchase.
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/originaltransactionid"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("originalTransactionId")]
        public string OriginalTransactionId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the price, in milliunits, of the In-App Purchase that 
        /// the system records in the transaction.
        /// </summary>
        /// <remarks>
        /// An integer value that represents the price multiplied by 1000 of the in-app 
        /// purchase or subscription offer you configured in App Store Connect and that 
        /// the system records at the time of the purchase. For more information, see price. 
        /// The currency parameter indicates the currency of this price.
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/price"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("price")]
        public long Price { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the product
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/productid"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("productId")]
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the time that the App Store charged the customer’s account 
        /// for an In-App Purchase, a restored In-App Purchase, a subscription, or a 
        /// subscription renewal after a lapse.
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/purchasedate"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("purchaseDate")]
        public long? PurchaseDate { get; set; }

        /// <summary>
        /// Gets or sets the number of purchased consumable products.
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/quantity"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("quantity")]
        public int Quantity { get; set; }


        /// <summary>
        /// Gets or sets the UNIX time, in milliseconds, that the App Store refunded 
        /// the transaction or revoked it from Family Sharing.
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/revocationdate"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("revocationDate")]
        public long? RevocationDate { get; set; }

        /// <summary>
        /// Gets or sets the reason for a refunded transaction.
        /// </summary>
        /// <remarks>
        /// 0 The App Store refunded the transaction on behalf of the customer for other reasons, for example, an accidental purchase.
        /// 1 The App Store refunded the transaction on behalf of the customer due to an actual or perceived issue within your app.
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/revocationreason"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("revocationReason")]
        public AppleRevocationReason? RevocationReason { get; set; }

        /// <summary>
        /// Gets or sets the UNIX time, in milliseconds, that the App Store signed the JSON Web Signature data.
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/signeddate"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("signedDate")]
        public long? SignedDate { get; set; }

        /// <summary>
        /// Gets or sets the three-letter code that represents the country or region 
        /// associated with the App Store storefront of the purchase.
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/storefront"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("storefront")]
        public string Storefront { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the App Store storefront of the purchase.
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/storefrontid"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("storefrontId")]
        public string StorefrontId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the subscription group that the subscription belongs to.
        /// </summary>
        /// <remarks>
        /// Auto-renewable subscriptions always belong to a subscription group. You create the subscription 
        /// group identifiers in App Store Connect before you create and add an auto-renewable subscription. 
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/subscriptiongroupidentifier" />
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("subscriptionGroupIdentifier")]
        public string SubscriptionGroupIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the transaction
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/transactionid"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("transactionId")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the cause of a purchase transaction, which indicates whether it’s 
        /// a customer’s purchase or a renewal for an auto-renewable subscription that the 
        /// system initiates.
        /// </summary>
        /// <remarks>
        /// PURCHASE The customer initiated the purchase, which may be for any in-app purchase type: consumable, non-consumable, non-renewing subscription, or auto-renewable subscription.
        /// RENEWAL  The App Store server initiated the purchase transaction to renew an auto-renewable subscription.
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/transactionreason"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("transactionReason")]
        public AppleTransactionReason? TransactionReason { get; set; }

        /// <summary>
        /// Gets or sets the type of In-App Purchase products you can offer in your app.
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/type"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of subscription purchase events across devices, 
        /// including subscription renewals.
        /// </summary>
        /// <remarks>
        /// <see href="https://developer.apple.com/documentation/appstoreserverapi/weborderlineitemid"/>
        /// </remarks>
        [System.Text.Json.Serialization.JsonPropertyName("webOrderLineItemId")]
        public string WebOrderLineItemId { get; set; }
    }
}
