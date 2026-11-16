# Changelog

## 2.1.0

### Added

- Multi-target `net8.0` and `net10.0`. CI installs both SDKs and runs the tests on each framework.

### Fixed

- 2.0.3 depended on `Microsoft.Extensions.Http` 11.0.0, which doesn't support `net8.0`; .NET 8 consumers got
  "doesn't support net8.0" warnings and the untested netstandard2.0 build of those packages. The dependency is
  now 10.0.12, which supports both `net8.0` and `net10.0`; apps on newer versions can still reference 11.x.
- `logo.ico` was packed as `content`/`contentFiles`, adding an `Assets/logo.ico` file to every consuming project.
- Certificates are loaded with `X509CertificateLoader` on .NET 9+ (the `X509Certificate2(byte[])` constructor is
  obsolete there, `SYSLIB0057`).

## 2.0.0

### Fixed

- **Notification and offer enums didn't deserialize.** `System.Text.Json` ignores `[EnumMember]`, so values such as
  `DID_CHANGE_RENEWAL_PREF`, `DID_RENEW`, `INITIAL_BUY` and `FREE_TRIAL` threw `JsonException`. A new
  `EnumMemberJsonConverter<T>` reads and writes Apple's wire values; unrecognized values map to a new `Unknown` member.
- **Production responses failed to deserialize.** `appAppleId` is a number in Apple's JSON but was declared as
  `string` on `SubscriptionStatusResponse` and `TransactionHistoryResponse`.
- **Transaction history filters were ignored.** Query parameters were sent with PascalCase names and `True`/`False`.
- `AppleOfferType.Introductory` was `0`; Apple uses `1`.
- `GetSubscriptionStatusAsync` filtered on `AppleAutoRenewStatuses`; the API's `status` filter is a subscription status
  (`ApplePayloadMetadataStatusType`). The old overload is `[Obsolete]`.
- `ToAppleTimeUsed` placed 5761–5860 minutes in the wrong bucket (4 days is 5760 minutes, not 5860).
- `ToAppleDollarAmount` misclassified amounts with more than two decimals (e.g. `49.995`).
- Argument validation was a no-op everywhere (`ThrowIfNull(nameof(x))` checks a string literal).
- `AddAppleApiClient` didn't register `IHttpClientFactory`, so resolving the client failed unless the caller had called
  `AddHttpClient()`.
- A per-call environment override changed the client's environment for all later calls on the same instance.
- GET requests lost Apple's error body; cancellation was wrapped as an HTTP 500 `AppleApiException`.
- The bearer token was re-signed on every request and leaked an `ECDsa` instance and a cached signature provider each time.
- `DateTime` helpers returned `DateTimeKind.Unspecified` values and 1970-01-01 for absent dates.

### Added

- `ISignedDataVerifier` / `SignedDataVerifier` and `AddAppleSignedDataVerifier(...)`: verify the x5c chain to Apple's
  root CA and the ES256 signature of notifications, transactions and renewal info.
- Endpoints: `GetAllTransactionHistoryAsync` (auto-paging), `GetRefundHistoryAsync`, `LookUpOrderIdAsync`,
  `ExtendSubscriptionRenewalDateAsync`, `RequestTestNotificationAsync`, `GetTestNotificationStatusAsync`.
- `AppleApiException.ApiErrorMessage` and `IsRetryable`.
- `*Utc` date helpers on transactions and notifications; `appTransactionId`, `offerPeriod`, `appAccountToken` fields.
- `TransactionHistoryRequest.ToQueryParameters()`, `ToAppleAccountTenure(utcNow)`.
- Unit test project (`test/Talegen.Apple.Storekit.Tests`), run in CI before packaging.
- README and how-to guides in `docs/`.

### Breaking changes

- `SubscriptionStatusResponse.AppAppleId` and `TransactionHistoryResponse.AppAppleId`: `string` → `long?`.
- `AppleResponseBodyV2DecodedPayload.SubNotificationType` is nullable (absent subtype is `null`, not `InitialBuy`).
- `ApplePayloadMetadata.Status` and `AppAppleId`, `ApplePayloadSummary.AppAppleId`,
  `ApplePayloadExternalPurchaseToken.AppAppleId` are nullable.
- `JwsRenewalInfoDecodedPayload`: `GracePeriodExpiresDate`, `RecentSubscriptionStartDate`, `RenewalDate`,
  `RenewalPrice`, `PriceIncreaseStatus` and their `DateTime` helpers are nullable; helpers now return UTC.
- `AppleOfferType.Introductory` changed from `0` to `1`.
- `BearerTokenAuthenticator` validates its arguments and the private key in the constructor, and is registered as a singleton.
- `AppleErrorCodes.OrigionalTransactionIdNotFoundRetryable` is obsolete; use `OriginalTransactionIdNotFoundRetryable`.
- The obsolete `GetSubscriptionStatusAsync(..., AppleAutoRenewStatuses status, ...)` overload takes a non-nullable
  status, so `GetSubscriptionStatusAsync(id, null)` binds to the current overload.
