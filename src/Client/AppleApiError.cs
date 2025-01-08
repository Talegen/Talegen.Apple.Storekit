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
    using System.Text.Json.Serialization;

    /// <summary>
    /// Contains an enumerated list of Apple error codes.
    /// </summary>
    public enum AppleErrorCodes : long
    {
        /**
         * An error that indicates an invalid request.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/generalbadrequesterror">GeneralBadRequestError</a>
         */
        GeneralBadRequest = 4000000L,

        /**
         * An error that indicates an invalid app identifier.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidappidentifiererror">InvalidAppIdentifierError</a>
         */
        InvalidAppIdentifier = 4000002L,

        /**
         * An error that indicates an invalid request revision.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidrequestrevisionerror">InvalidRequestRevisionError</a>
         */
        InvalidRequestRevision = 4000005L,

        /**
         * An error that indicates an invalid transaction identifier.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidtransactioniderror">InvalidTransactionIdError</a>
         */
        InvalidTransactionId = 4000006L,

        /**
         * An error that indicates an invalid original transaction identifier.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidoriginaltransactioniderror">InvalidOriginalTransactionIdError</a>
         */
        InvalidOriginalTransactionId = 4000008L,

        /**
         * An error that indicates an invalid extend-by-days value.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidextendbydayserror">InvalidExtendByDaysError</a>
         */
        InvalidExtendByDays = 4000009L,

        /**
         * An error that indicates an invalid reason code.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidextendreasoncodeerror">InvalidExtendReasonCodeError</a>
         */
        InvalidExtendReasonCode = 4000010L,

        /**
         * An error that indicates an invalid request identifier.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidrequestidentifiererror">InvalidRequestIdentifierError</a>
         */
        InvalidRequestIdentifier = 4000011L,

        /**
         * An error that indicates that the start date is earlier than the earliest allowed date.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/startdatetoofarinpasterror">StartDateTooFarInPastError</a>
         */
        StartDateTooFarInPast = 4000012L,

        /**
         * An error that indicates that the end date precedes the start date, or the two dates are equal.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/startdateafterenddateerror">StartDateAfterEndDateError</a>
         */
        StartDateAfterEndDate = 4000013L,

        /**
         * An error that indicates the pagination token is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidpaginationtokenerror">InvalidPaginationTokenError</a>
         */
        InvalidPaginationToken = 4000014L,

        /**
         * An error that indicates the start date is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidstartdateerror">InvalidStartDateError</a>
         */
        InvalidStartDate = 4000015L,

        /**
         * An error that indicates the end date is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidenddateerror">InvalidEndDateError</a>
         */
        InvalidEndDate = 4000016L,

        /**
         * An error that indicates the pagination token expired.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/paginationtokenexpirederror">PaginationTokenExpiredError</a>
         */
        PaginationTokenExpired = 4000017L,

        /**
         * An error that indicates the notification type or subtype is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidnotificationtypeerror">InvalidNotificationTypeError</a>
         */
        InvalidNotificationType = 4000018L,

        /**
         * An error that indicates the request is invalid because it has too many constraints applied.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/multiplefilterssuppliederror">MultipleFiltersSuppliedError</a>
         */
        MultipleFiltersSupplied = 4000019L,

        /**
         * An error that indicates the test notification token is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidtestnotificationtokenerror">InvalidTestNotificationTokenError</a>
         */
        InvalidTestNotificationToken = 4000020L,

        /**
         * An error that indicates an invalid sort parameter.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidsorterror">InvalidSortError</a>
         */
        InvalidSort = 4000021L,

        /**
         * An error that indicates an invalid product type parameter.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidproducttypeerror">InvalidProductTypeError</a>
         */
        InvalidProductType = 4000022L,

        /**
         * An error that indicates the product ID parameter is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidproductiderror">InvalidProductIdError</a>
         */
        InvalidProductId = 4000023L,

        /**
         * An error that indicates an invalid subscription group identifier.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidsubscriptiongroupidentifiererror">InvalidSubscriptionGroupIdentifierError</a>
         */
        InvalidSubscriptionGroupIdentifier = 4000024L,

        /**
         * An error that indicates the query parameter exclude-revoked is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidexcluderevokederror">InvalidExcludeRevokedError</a>
         */
        [Obsolete("This value is no longer valid.")]
        InvalidExcludeRevoked = 4000025L,

        /**
         * An error that indicates an invalid in-app ownership type parameter.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidinappownershiptypeerror">InvalidInAppOwnershipTypeError</a>
         */
        InvalidInAppOwnershipType = 4000026L,

        /**
         * An error that indicates a required storefront country code is empty.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidemptystorefrontcountrycodelisterror">InvalidEmptyStorefrontCountryCodeListError</a>
         */
        InvalidEmptyStorefrontCountryCodeList = 4000027L,

        /**
         * An error that indicates a storefront code is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidstorefrontcountrycodeerror">InvalidStorefrontCountryCodeError</a>
         */
        InvalidStorefrontCountryCode = 4000028L,

        /**
         * An error that indicates the revoked parameter contains an invalid value.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidrevokederror">InvalidRevokedError</a>
         */
        InvalidRevoked = 4000030L,

        /**
         * An error that indicates the status parameter is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidstatuserror">InvalidStatusError</a>
         */
        InvalidStatus = 4000031L,

        /**
         * An error that indicates the value of the account tenure field is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidaccounttenureerror">InvalidAccountTenureError</a>
         */
        InvalidAccountTenure = 4000032L,

        /**
         * An error that indicates the value of the app account token field is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidappaccounttokenerror">InvalidAppAccountTokenError</a>
         */
        InvalidAppAccountToken = 4000033L,

        /**
         * An error that indicates the value of the consumption status field is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidconsumptionstatuserror">InvalidConsumptionStatusError</a>
         */
        InvalidConsumptionStatus = 4000034L,

        /**
         * An error that indicates the customer consented field is invalid or doesn't indicate that the customer consented.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidcustomerconsentederror">InvalidCustomerConsentedError</a>
         */
        InvalidCustomerConsented = 4000035L,

        /**
         * An error that indicates the value in the delivery status field is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invaliddeliverystatuserror">InvalidDeliveryStatusError</a>
         */
        InvalidDeliveryStatus = 4000036L,

        /**
         * An error that indicates the value in the lifetime dollars purchased field is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidlifetimedollarspurchasederror">InvalidLifetimeDollarsPurchasedError</a>
         */
        InvalidLifetimeDollarsPurchased = 4000037L,

        /**
         * An error that indicates the value in the lifetime dollars refunded field is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidlifetimedollarsrefundederror">InvalidLifetimeDollarsRefundedError</a>
         */
        InvalidLifetimeDollarsRefunded = 4000038L,

        /**
         * An error that indicates the value in the platform field is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidplatformerror">InvalidPlatformError</a>
         */
        InvalidPlatform = 4000039L,

        /**
         * An error that indicates the value in the playtime field is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidplaytimeerror">InvalidPlayTimeError</a>
         */
        InvalidPlayTime = 4000040L,

        /**
         * An error that indicates the value in the sample content provided field is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidsamplecontentprovidederror">InvalidSampleContentProvidedError</a>
         */
        InvalidSampleContentProvided = 4000041L,

        /**
         * An error that indicates the value in the user status field is invalid.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invaliduserstatuserror">InvalidUserStatusError</a>
         */
        InvalidUserStatus = 4000042L,

        /**
         * An error that indicates the transaction identifier doesn't represent a consumable in-app purchase.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidtransactionnotconsumableerror">InvalidTransactionNotConsumableError</a>
         */
        [Obsolete]
        InvalidTransactionNotConsumable = 4000043L,

        /**
         * An error that indicates the transaction identifier represents an unsupported in-app purchase type.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/invalidtransactiontypenotsupportederror">InvalidTransactionTypeNotSupportedError</a>
         */
        InvalidTransactionTypeNotSupported = 4000047L,

        /**
         * An error that indicates the subscription doesn't qualify for a renewal-date extension due to its subscription state.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/subscriptionextensionineligibleerror">SubscriptionExtensionIneligibleError</a>
         */
        SubscriptionExtensionIneligible = 4030004L,

        /**
         * An error that indicates the subscription doesn't qualify for a renewal-date extension because it has already received the maximum extensions.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/subscriptionmaxextensionerror">SubscriptionMaxExtensionError</a>
         */
        SubscriptionMaxExtension = 4030005L,

        /**
         * An error that indicates a subscription isn't directly eligible for a renewal date extension because the user obtained it through Family Sharing.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/familysharedsubscriptionextensionineligibleerror">FamilySharedSubscriptionExtensionIneligibleError</a>
         */
        FamilySharedSubscriptionExtensionIneligible = 4030007L,

        /**
         * An error that indicates the App Store account wasn't found.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/accountnotfounderror">AccountNotFoundError</a>
         */
        AccountNotFound = 4040001L,

        /**
         * An error response that indicates the App Store account wasn't found, but you can try again.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/accountnotfoundretryableerror">AccountNotFoundRetryableError</a>
         */
        AccountNotFoundRetryable = 4040002L,

        /**
         * An error that indicates the app wasn't found.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/appnotfounderror">AppNotFoundError</a>
         */
        AppNotFound = 4040003L,

        /**
         * An error response that indicates the app wasn't found, but you can try again.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/appnotfoundretryableerror">AppNotFoundRetryableError</a>
         */
        AppNotFoundRetryable = 4040004L,

        /**
         * An error that indicates an original transaction identifier wasn't found.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/originaltransactionidnotfounderror">OriginalTransactionIdNotFoundError</a>
         */
        OriginalTransactionIdNotFound = 4040005L,

        /**
         * An error response that indicates the original transaction identifier wasn't found, but you can try again.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/originaltransactionidnotfoundretryableerror">OriginalTransactionIdNotFoundRetryableError</a>
         */
        OrigionalTransactionIdNotFoundRetryable = 4040006L,

        /**
         * An error that indicates that the App Store server couldn't find a notifications URL for your app in this environment.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/servernotificationurlnotfounderror">ServerNotificationUrlNotFoundError</a>
         */
        ServerNotificationUrlNotFound = 4040007L,

        /**
         * An error that indicates that the test notification token is expired or the test notification status isn't available.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/testnotificationnotfounderror">TestNotificationNotFoundError</a>
         */
        TestNotificationNotFound = 4040008L,

        /**
         * An error that indicates the server didn't find a subscription-renewal-date extension request for the request identifier and product identifier you provided.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/statusrequestnotfounderror">StatusRequestNotFoundError</a>
         */
        StatusRequestNotFound = 4040009L,

        /**
         * An error that indicates a transaction identifier wasn't found.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/transactionidnotfounderror">TransactionIdNotFoundError</a>
         */
        TransactionIdNotFound = 4040010L,

        /**
         * An error that indicates that the request exceeded the rate limit.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/ratelimitexceedederror">RateLimitExceededError</a>
         */
        RateLimitExceeded = 4290000L,

        /**
         * An error that indicates a general internal error.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/generalinternalerror">GeneralInternalError</a>
         */
        GeneralInternal = 5000000L,

        /**
         * An error response that indicates an unknown error occurred, but you can try again.
         *
         * @see <a href="https://developer.apple.com/documentation/appstoreserverapi/generalinternalretryableerror">GeneralInternalRetryableError</a>
         */
        GeneralInternalRetryable = 5000001L
    }

    /// <summary>
    /// This class represents an API error response.
    /// </summary>
    public class AppleApiError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppleApiError"/> class.
        /// </summary>
        public AppleApiError() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleApiError"/> class.
        /// </summary>
        /// <param name="errorCode">Contains the error code to initialize.</param>
        public AppleApiError(AppleErrorCodes errorCode, string errorMessage = "")
        {
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        [JsonPropertyName("errorCode")]
        public AppleErrorCodes ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
