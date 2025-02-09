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
    /// Contains an enumerated list of allowed notification types.
    /// </summary>
    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter<AppleStoreNotificationType>))]
    public enum AppleStoreNotificationType
    {
        /// <summary>
        /// A notification type that, along with its subtype, indicates that the customer subscribed to an auto-renewable subscription. 
        /// If the subtype is INITIAL_BUY, the customer either purchased or received access through Family Sharing to the subscription for 
        /// the first time. 
        /// If the subtype is RESUBSCRIBE, the user resubscribed or received access through Family Sharing to the same subscription 
        /// or to another subscription within the same subscription group.
        /// For notifications about other product type purchases, see the ONE_TIME_CHARGE notification type.
        /// </summary>
        [EnumMember(Value = "SUBSCRIBED")]
        Subscribed,

        /// <summary>
        /// A notification type that, along with its subtype, indicates that the customer made a change to their subscription plan. 
        /// 
        /// If the subtype is UPGRADE, the user upgraded their subscription. The upgrade goes into effect immediately, starting a 
        /// new billing period, and the user receives a prorated refund for the unused portion of the previous period. 
        /// 
        /// If the subtype is DOWNGRADE, the customer downgraded their subscription. Downgrades take effect at the next renewal 
        /// date and don’t affect the currently active plan.
        /// 
        /// If the subtype is empty, the user changed their renewal preference back to the current subscription, effectively canceling a downgrade.
        /// </summary>
        [EnumMember(Value = "DID_CHANGE_RENEWAL_PREF")]
        ChangeRenewalPreference,

        /// <summary>
        /// A notification type that, along with its subtype, indicates that the customer made a change to the subscription renewal status. 
        /// 
        /// If the subtype is AUTO_RENEW_ENABLED, the customer reenabled subscription auto-renewal. 
        /// If the subtype is AUTO_RENEW_DISABLED, the customer disabled subscription auto-renewal, or the App Store disabled subscription 
        /// auto-renewal after the customer requested a refund.
        /// </summary>
        [EnumMember(Value = "DID_CHANGE_RENEWAL_STATUS")]
        ChangeRenewalStatus,

        /// <summary>
        /// A notification type that, along with its subtype, indicates that a customer with an active subscription redeemed a subscription offer.
        /// If the subtype is UPGRADE, the customer redeemed an offer to upgrade their active subscription, which goes into effect immediately. 
        /// If the subtype is DOWNGRADE, the customer redeemed an offer to downgrade their active subscription, which goes into effect at the next 
        /// renewal date. 
        /// If the customer redeemed an offer for their active subscription, you receive an OFFER_REDEEMED notification type without a subtype.
        /// </summary>
        [EnumMember(Value = "OFFER_REDEEMED")]
        OfferRedeemed,

        /// <summary>
        /// A notification type that, along with its subtype, indicates that the subscription successfully renewed. 
        /// If the subtype is BILLING_RECOVERY, the expired subscription that previously failed to renew has successfully renewed. 
        /// If the subtype is empty, the active subscription has successfully auto-renewed for a new transaction period. 
        /// Provide the customer with access to the subscription’s content or service.
        /// </summary>
        [EnumMember(Value = "DID_RENEW")]
        Renewed,

        /// <summary>
        /// A notification type that, along with its subtype, indicates that a subscription expired. 
        /// If the subtype is VOLUNTARY, the subscription expired after the user disabled subscription renewal. 
        /// If the subtype is BILLING_RETRY, the subscription expired because the billing retry period ended without a successful billing transaction. 
        /// If the subtype is PRICE_INCREASE, the subscription expired because the customer didn't consent to a price increase that requires customer consent. 
        /// If the subtype is PRODUCT_NOT_FOR_SALE, the subscription expired because the product wasn't available for purchase at the time the subscription attempted to renew.
        /// A notification without a subtype indicates that the subscription expired for some other reason.
        /// </summary>
        [EnumMember(Value = "EXPIRED")]
        Expired,

        /// <summary>
        /// A notification type that, along with its subtype, indicates that the subscription failed to renew due to a billing issue. 
        /// The subscription enters the billing retry period. If the subtype is GRACE_PERIOD, continue to provide service through the grace period. 
        /// If the subtype is empty, the subscription isn’t in a grace period and you can stop providing the subscription service.
        /// 
        /// Inform the customer that there may be an issue with their billing information. The App Store continues to retry billing for 60 days, 
        /// or until the customer resolves their billing issue or cancels their subscription, whichever comes first. 
        /// </summary>
        [EnumMember(Value = "DID_FAIL_TO_RENEW")]
        FailedToRenew,

        /// <summary>
        /// A notification type that indicates that the billing grace period has ended without renewing the subscription, 
        /// so you can turn off access to the service or content. Inform the customer that there may be an issue with their billing information. 
        /// The App Store continues to retry billing for 60 days, or until the customer resolves their billing issue or cancels their subscription, 
        /// whichever comes first.
        /// </summary>
        [EnumMember(Value = "GRACE_PERIOD_EXPIRED")]
        GracePeriodExpired,

        /// <summary>
        /// A notification type that, along with its subtype, indicates that the system has informed the customer of an auto-renewable subscription price increase.
        /// If the price increase requires customer consent, the subtype is PENDING if the customer hasn't responded to the price increase, or ACCEPTED if the customer has consented to the price increase.
        /// If the price increase doesn't require customer consent, the subtype is ACCEPTED.
        /// </summary>
        [EnumMember(Value = "PRICE_INCREASE")]
        PriceIncrease,

        /// <summary>
        /// A notification type that indicates that the App Store successfully refunded a transaction for a consumable in-app purchase, 
        /// a non-consumable in-app purchase, an auto-renewable subscription, or a non-renewing subscription.
        /// The revocationDate contains the timestamp of the refunded transaction. 
        /// The originalTransactionId and productId identify the original transaction and product. The revocationReason contains the reason.
        /// </summary>
        [EnumMember(Value = "REFUND")]
        Refund,

        /// <summary>
        /// A notification type that indicates the App Store declined a refund request.
        /// </summary>
        [EnumMember(Value = "REFUND_DECLINED")]
        RefundDeclined,

        /// <summary>
        /// A notification type that indicates that the customer initiated a refund request for a consumable in-app purchase or 
        /// auto-renewable subscription, and the App Store is requesting that you provide consumption data.
        /// </summary>
        [EnumMember(Value = "CONSUMPTION_REQUEST")]
        ConsumptionRequest,

        /// <summary>
        /// A notification type that, along with its subtype, indicates that the App Store is attempting to extend the subscription 
        /// renewal date that you request by calling Extend Subscription Renewal Dates for All Active Subscribers.
        /// If the subtype is SUMMARY, the App Store completed extending the renewal date for all eligible subscribers. See the summary in the responseBodyV2DecodedPayload for details. 
        /// If the subtype is FAILURE, the renewal date extension didn't succeed for a specific subscription. See the data in the responseBodyV2DecodedPayload for details.
        /// </summary>
        [EnumMember(Value = "RENEWAL_EXTENDED")]
        RenewalExtended,

        /// <summary>
        /// A notification type that indicates that an in-app purchase the customer was entitled to through Family Sharing is no longer available through sharing. 
        /// The App Store sends this notification when a purchaser disables Family Sharing for their purchase, the purchaser (or family member) leaves the family group, 
        /// or the purchaser receives a refund. Your app also receives a paymentQueue(_:didRevokeEntitlementsForProductIdentifiers:) call. Family Sharing applies to 
        /// non-consumable in-app purchases and auto-renewable subscriptions. For more information about Family Sharing, see Supporting Family Sharing in your app.
        /// </summary>
        [EnumMember(Value = "REVOKE")]
        Revoke,

        /// <summary>
        /// A notification type that the App Store server sends when you request it by calling the Request a Test Notification endpoint. 
        /// Call that endpoint to test whether your server is receiving notifications. You receive this notification only at your request. 
        /// </summary>
        [EnumMember(Value = "TEST")]
        Test,

        /// <summary>
        /// A notification type that indicates the App Store extended the subscription renewal date for a specific subscription. 
        /// You request subscription-renewal-date extensions by calling Extend a Subscription Renewal Date or Extend Subscription 
        /// Renewal Dates for All Active Subscribers in the App Store Server API.
        /// </summary>
        [EnumMember(Value = "RENEWAL_EXTENSION")]
        RenewalExtension,

        /// <summary>
        /// A notification type that indicates the App Store reversed a previously granted refund due to a dispute that the customer raised. 
        /// If your app revoked content or services as a result of the related refund, it needs to reinstate them. 
        /// This notification type can apply to any in-app purchase type: consumable, non-consumable, non-renewing subscription, 
        /// and auto-renewable subscription. For auto-renewable subscriptions, the renewal date remains unchanged when the App Store reverses a refund.
        /// </summary>
        [EnumMember(Value = "REFUND_REVERSED")]
        RefundReversed,

        /// <summary>
        /// A notification type that, along with its subtype UNREPORTED, indicates that Apple created an external purchase token for your app, 
        /// but didn't receive a report. For more information about reporting the token, see externalPurchaseToken.
        /// This notification applies only to apps that use the External Purchase to provide alternative payment options.
        /// </summary>
        [EnumMember(Value = "EXTERNAL_PURCHASE_TOKEN")]
        ExternalPurchaseToken,

        /// <summary>
        /// The ONE_TIME_CHARGE notification is currently available only in the sandbox environment.
        /// A notification type that indicates the customer purchased a consumable, non-consumable, or non-renewing subscription. 
        /// The App Store also sends this notification when the customer receives access to a non-consumable product through Family Sharing.
        /// </summary>
        [EnumMember(Value = "ONE_TIME_CHARGE")]
        OneTimeCharge,
    }

    /// <summary>
    /// Contains an enumerated list of sub-field notification types.
    /// </summary>
    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter<AppleStoreSubNotificationType>))]
    public enum AppleStoreSubNotificationType
    {
        /// <summary>
        /// Applies to the SUBSCRIBED notificationType. A notification with this subtype indicates that the user purchased 
        /// the subscription for the first time or that the user received access to the subscription through Family Sharing 
        /// for the first time.
        /// </summary>
        [EnumMember(Value = "INITIAL_BUY")]
        InitialBuy,

        /// <summary>
        /// Applies to the SUBSCRIBED notificationType. A notification with this subtype indicates that the user resubscribed 
        /// or received access through Family Sharing to the same subscription or to another subscription within the 
        /// same subscription group.
        /// </summary>
        [EnumMember(Value = "RESUBSCRIBE")]
        Resubscribe,

        /// <summary>
        /// Applies to the DID_CHANGE_RENEWAL_PREF and OFFER_REDEEMED notificationType. 
        /// A notification with this subtype indicates that the user downgraded their subscription or 
        /// cross-graded to a subscription with a different duration. 
        /// Downgrades take effect at the next renewal date.
        /// </summary>
        [EnumMember(Value = "DOWNGRADE")]
        Downgrade,

        /// <summary>
        /// Applies to the DID_CHANGE_RENEWAL_PREF and OFFER_REDEEMED notificationType. 
        /// A notification with this subtype indicates that the user upgraded their subscription or 
        /// cross-graded to a subscription with the same duration. Upgrades take effect immediately.
        /// </summary>
        [EnumMember(Value = "UPGRADE")]
        Upgrade,

        /// <summary>
        /// Applies to the DID_CHANGE_RENEWAL_STATUS notificationType. 
        /// A notification with this subtype indicates that the user enabled subscription auto-renewal.
        /// </summary>
        [EnumMember(Value = "AUTO_RENEW_ENABLED")]
        AutoRenewEnabled,

        /// <summary>
        /// Applies to the DID_CHANGE_RENEWAL_STATUS notificationType. 
        /// A notification with this subtype indicates that the user disabled subscription auto-renewal, or the App Store disabled subscription auto-renewal 
        /// after the user requested a refund.
        /// </summary>
        [EnumMember(Value = "AUTO_RENEW_DISABLED")]
        AutoRenewDisabled,

        /// <summary>
        /// Applies to the EXPIRED notificationType. A notification with this subtype indicates that the subscription expired after 
        /// the user disabled subscription auto-renewal.
        /// </summary>
        [EnumMember(Value = "VOLUNTARY")]
        Voluntary,

        /// <summary>
        /// Applies to the EXPIRED notificationType. A notification with this subtype indicates that the subscription expired because 
        /// the subscription failed to renew before the billing retry period ended.
        /// </summary>
        [EnumMember(Value = "BILLING_RETRY")]
        BillingRetry,

        /// <summary>
        /// Applies to the EXPIRED notificationType. A notification with this subtype indicates that the subscription expired because 
        /// the user didn't consent to a price increase.
        /// </summary>
        [EnumMember(Value = "PRICE_INCREASE")]
        PriceIncrease,

        /// <summary>
        /// Applies to the DID_FAIL_TO_RENEW notificationType. A notification with this subtype indicates that the subscription 
        /// failed to renew due to a billing issue. Continue to provide access to the subscription during the grace period.
        /// </summary>
        [EnumMember(Value = "GRACE_PERIOD")]
        GracePeriod,

        /// <summary>
        /// Applies to the PRICE_INCREASE notificationType. A notification with this subtype indicates that the system informed 
        /// the user of the subscription price increase, but the user hasn't accepted it.
        /// </summary>
        [EnumMember(Value = "PENDING")]
        Pending,

        /// <summary>
        /// Applies to the PRICE_INCREASE notificationType. A notification with this subtype indicates that the customer consented to the 
        /// subscription price increase if the price increase requires customer consent, or that the system notified them of a price 
        /// increase if the price increase doesn't require customer consent.
        /// </summary>
        [EnumMember(Value = "ACCEPTED")]
        Accepted,

        /// <summary>
        /// Applies to the DID_RENEW notificationType. A notification with this subtype indicates that the expired subscription 
        /// that previously failed to renew has successfully renewed.
        /// </summary>
        [EnumMember(Value = "BILLING_RECOVERY")]
        BillingRecovery,

        /// <summary>
        /// Applies to the EXPIRED notificationType. A notification with this subtype indicates that the subscription expired 
        /// because the product wasn't available for purchase at the time the subscription attempted to renew.
        /// </summary>
        [EnumMember(Value = "PRODUCT_NOT_FOR_SALE")]
        ProductNotForSale,

        /// <summary>
        /// Applies to the RENEWAL_EXTENSION notificationType. A notification with this subtype indicates that the App Store 
        /// server completed your request to extend the subscription renewal date for all eligible subscribers. 
        /// For the summary details, see the summary object in the responseBodyV2DecodedPayload. 
        /// </summary>
        [EnumMember(Value = "SUMMARY")]
        Summary,

        /// <summary>
        /// Applies to the RENEWAL_EXTENSION notificationType. A notification with this subtype indicates that the 
        /// subscription-renewal-date extension failed for an individual subscription. 
        /// For details, see the data object in the responseBodyV2DecodedPayload. For information on the request, 
        /// see Extend Subscription Renewal Dates for All Active Subscribers.
        /// </summary>
        [EnumMember(Value = "FAILURE")]
        Failure,

        /// <summary>
        /// Applies to the EXTERNAL_PURCHASE_TOKEN notificationType. A notification with this subtype indicates that 
        /// Apple created a token for your app but didn't receive a report. 
        /// For more information about reporting the token, see externalPurchaseToken.
        /// </summary>
        [EnumMember(Value = "UNREPORTED")]
        Unreported,
    }

    /// <summary>
    /// This class defines the Apple response body version 2 decoded payload.
    /// </summary>
    /// <remarks>
    /// The payload can contain only one of the following three fields:
    /// 
    /// 1. A data object, which contains details including the environment, the app metadata, and the signed transaction and subscription renewal information.
    /// 2. A summary object, which contains information only when the notification is a RENEWAL_EXTENSION with a SUMMARY subtype.For more information, see Extend Subscription Renewal Dates for All Active Subscribers.
    /// 3. An externalPurchaseToken, which contains an external purchase token only when the notification is EXTERNAL_PURCHASE_TOKEN.For more information about this notification, see externalPurchaseToken.
    /// </remarks>
    public class AppleResponseBodyV2DecodedPayload
    {
        /// <summary>
        /// Gets or sets the payload notification type.
        /// </summary>
        [JsonPropertyName("notificationType")]
        public AppleStoreNotificationType NotificationType { get; set; }

        /// <summary>
        /// Gets or sets a value that further describes an event of type notificationType. It’s present only for specific version 2 notifications.
        /// </summary>
        [JsonPropertyName("subtype")]
        public AppleStoreSubNotificationType SubNotificationType { get; set; }

        /// <summary>
        /// Gets or sets the object that contains the app metadata and signed renewal and transaction information.
        /// The data, summary, and externalPurchaseToken fields are mutually exclusive.The payload contains only one of these fields.
        /// </summary>
        [JsonPropertyName("data")]
        public ApplePayloadMetadata? Data { get; set; }

        /// <summary>
        /// Gets or sets the summary data that appears when the App Store server completes your request to extend a subscription renewal date 
        /// for eligible subscribers. For more information, see Extend Subscription Renewal Dates for All Active Subscribers.
        /// The data, summary, and externalPurchaseToken fields are mutually exclusive.The payload contains only one of these fields.
        /// </summary>
        [JsonPropertyName("summary")]
        public ApplePayloadSummary? Summary { get; set; }

        /// <summary>
        /// Gets or sets the token when the notificationType is EXTERNAL_PURCHASE_TOKEN. 
        /// The data, summary, and externalPurchaseToken fields are mutually exclusive.The payload contains only one of these fields.
        /// </summary>
        [JsonPropertyName("externalPurchaseToken")]
        public ApplePayloadExternalPurchaseToken? ExternalPurchaseToken { get; set; }

        /// <summary>
        /// Gets or sets the version of the payload. The App Store Server Notification version number, "2.0".
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the UNIX time, in milliseconds, that the App Store signed the JSON Web Signature data.
        /// </summary>
        [JsonPropertyName("signedDate")]
        public long? SignedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the notification.
        /// </summary>
        [JsonPropertyName("notificationUUID")]
        public Guid NotificationIdentifier { get; set; }
    }
}
