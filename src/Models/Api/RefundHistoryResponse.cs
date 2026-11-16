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
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// This class represents the response of the Get Refund History endpoint.
    /// </summary>
    public class RefundHistoryResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the App Store has more refunded transactions than this response contains.
        /// </summary>
        [JsonPropertyName("hasMore")]
        public bool HasMore { get; set; }

        /// <summary>
        /// Gets or sets the token to pass as <c>revision</c> to request the next page.
        /// </summary>
        [JsonPropertyName("revision")]
        public string? Revision { get; set; }

        /// <summary>
        /// Gets or sets the refunded transactions, signed by Apple, in JWS format.
        /// </summary>
        [JsonPropertyName("signedTransactions")]
        public List<string> SignedTransactions { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the decoded transactions.
        /// </summary>
        [JsonIgnore]
        public List<JwsTransactionDecodedPayload> DecodedTransactions { get; set; } = new List<JwsTransactionDecodedPayload>();
    }

    /// <summary>
    /// Contains the order lookup statuses.
    /// </summary>
    public enum OrderLookupStatus
    {
        /// <summary>
        /// The order ID is valid.
        /// </summary>
        Valid = 0,

        /// <summary>
        /// The order ID is invalid.
        /// </summary>
        Invalid = 1,
    }

    /// <summary>
    /// This class represents the response of the Look Up Order ID endpoint.
    /// </summary>
    public class OrderLookupResponse
    {
        /// <summary>
        /// Gets or sets the status that indicates whether the order ID is valid.
        /// </summary>
        [JsonPropertyName("status")]
        public OrderLookupStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the in-app purchase transactions that are part of the order, signed by Apple, in JWS format.
        /// </summary>
        [JsonPropertyName("signedTransactions")]
        public List<string> SignedTransactions { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the decoded transactions.
        /// </summary>
        [JsonIgnore]
        public List<JwsTransactionDecodedPayload> DecodedTransactions { get; set; } = new List<JwsTransactionDecodedPayload>();
    }

    /// <summary>
    /// This class represents the response of the Request a Test Notification endpoint.
    /// </summary>
    public class SendTestNotificationResponse
    {
        /// <summary>
        /// Gets or sets the token used to check the status of the test notification.
        /// </summary>
        [JsonPropertyName("testNotificationToken")]
        public string TestNotificationToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// This class represents a single attempt to deliver a notification to your server.
    /// </summary>
    public class SendAttemptItem
    {
        /// <summary>
        /// Gets or sets the UNIX time, in milliseconds, of the attempt.
        /// </summary>
        [JsonPropertyName("attemptDate")]
        public long? AttemptDate { get; set; }

        /// <summary>
        /// Gets or sets the result of the attempt, e.g. <c>SUCCESS</c>, <c>TIMED_OUT</c>, <c>SSL_ISSUE</c>.
        /// </summary>
        [JsonPropertyName("sendAttemptResult")]
        public string? SendAttemptResult { get; set; }
    }

    /// <summary>
    /// This class represents the response of the Get Test Notification Status endpoint.
    /// </summary>
    public class CheckTestNotificationResponse
    {
        /// <summary>
        /// Gets or sets the test notification payload that the App Store sent, in JWS format.
        /// </summary>
        [JsonPropertyName("signedPayload")]
        public string? SignedPayload { get; set; }

        /// <summary>
        /// Gets or sets the delivery attempts the App Store made.
        /// </summary>
        [JsonPropertyName("sendAttempts")]
        public List<SendAttemptItem> SendAttempts { get; set; } = new List<SendAttemptItem>();
    }

    /// <summary>
    /// Contains the reasons for a subscription renewal date extension.
    /// </summary>
    public enum ExtendReasonCode
    {
        /// <summary>
        /// Undeclared; no information shared.
        /// </summary>
        Undeclared = 0,

        /// <summary>
        /// The renewal date extension is for customer satisfaction.
        /// </summary>
        CustomerSatisfaction = 1,

        /// <summary>
        /// The renewal date extension is for other reasons.
        /// </summary>
        Other = 2,

        /// <summary>
        /// The renewal date extension is due to a service issue or outage.
        /// </summary>
        ServiceIssueOrOutage = 3,
    }

    /// <summary>
    /// This class represents the request body of the Extend a Subscription Renewal Date endpoint.
    /// </summary>
    public class ExtendRenewalDateRequest
    {
        /// <summary>
        /// Gets or sets the number of days to extend the renewal date (maximum 90).
        /// </summary>
        [JsonPropertyName("extendByDays")]
        public int ExtendByDays { get; set; }

        /// <summary>
        /// Gets or sets the reason for the extension.
        /// </summary>
        [JsonPropertyName("extendReasonCode")]
        public ExtendReasonCode ExtendReasonCode { get; set; }

        /// <summary>
        /// Gets or sets a string that uniquely identifies the request, used for idempotency (maximum 128 characters).
        /// </summary>
        [JsonPropertyName("requestIdentifier")]
        public string RequestIdentifier { get; set; } = string.Empty;
    }

    /// <summary>
    /// This class represents the response of the Extend a Subscription Renewal Date endpoint.
    /// </summary>
    public class ExtendRenewalDateResponse
    {
        /// <summary>
        /// Gets or sets the new subscription expiration date, in UNIX time milliseconds.
        /// </summary>
        [JsonPropertyName("effectiveDate")]
        public long? EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets the original transaction identifier of the subscription.
        /// </summary>
        [JsonPropertyName("originalTransactionId")]
        public string? OriginalTransactionId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the extension succeeded.
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of subscription purchase events across devices.
        /// </summary>
        [JsonPropertyName("webOrderLineItemId")]
        public string? WebOrderLineItemId { get; set; }
    }
}
