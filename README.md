# Talegen.Apple.Storekit

A .NET client (targets .NET 8 and .NET 10; usable from any .NET 8+ app) for the [Apple App Store Server API](https://developer.apple.com/documentation/appstoreserverapi)
and [App Store Server Notifications V2](https://developer.apple.com/documentation/appstoreservernotifications),
including verification of Apple-signed data (JWS).

```bash
dotnet add package Talegen.Apple.Storekit
```

## What it covers

| Area | API |
| --- | --- |
| Get a transaction | `client.GetTransactionAsync(transactionId)` |
| Transaction history (one page / all pages) | `GetTransactionHistoryAsync`, `GetAllTransactionHistoryAsync` |
| Subscription statuses | `GetSubscriptionStatusAsync(transactionId, status?)` |
| Extend a subscription renewal date | `ExtendSubscriptionRenewalDateAsync` |
| Refund history | `GetRefundHistoryAsync` |
| Look up an order ID | `LookUpOrderIdAsync` |
| Send consumption information | `SendConsumptionInfoAsync` |
| Test notifications | `RequestTestNotificationAsync`, `GetTestNotificationStatusAsync` |
| Verify notifications / signed transactions | `ISignedDataVerifier` |

All API calls are extension methods on `IAppStoreServerApiClient` in the `Talegen.Apple.Storekit.Extensions` namespace,
and accept an optional per-call `EnvironmentType` override and a `CancellationToken`.

## Quick start

### 1. Get your credentials

In App Store Connect, go to **Users and Access → Integrations → In-App Purchase** and generate a key. You need:

- the **Issuer ID** (shown at the top of the page),
- the **Key ID**,
- the downloaded **`.p8` private key** (you can only download it once),
- your app's **bundle ID**.

Keep the private key in a secret store (Key Vault, AWS Secrets Manager, user-secrets) — never in source control.

### 2. Register the client

```csharp
using Talegen.Apple.Storekit.Extensions;
using Talegen.Apple.Storekit.Models;

builder.Services.AddAppleApiClient(options =>
{
    builder.Configuration.GetSection("Apple").Bind(options);
    // or set explicitly:
    // options.IssuerId    = "57246542-96fe-1a63-e053-0824d011072a";
    // options.KeyId       = "2X9R4HXF34";
    // options.BundleId    = "com.example.app";
    // options.PrivateKey  = File.ReadAllText("AuthKey_2X9R4HXF34.p8");
    // options.Environment = EnvironmentType.Sandbox;   // default: Production
});
```

```json
{
  "Apple": {
    "IssuerId": "57246542-96fe-1a63-e053-0824d011072a",
    "KeyId": "2X9R4HXF34",
    "BundleId": "com.example.app",
    "Environment": "Production"
  }
}
```

`PrivateKey` accepts the full PEM text of the `.p8` file or just its base64 body.
`AddAppleApiClient` registers everything it needs, including the `HttpClient`; you don't need to call `AddHttpClient()` yourself.

### 3. Call the API

```csharp
public class PurchaseService(IAppStoreServerApiClient apple)
{
    public async Task<bool> HasActiveSubscriptionAsync(string transactionId, CancellationToken ct)
    {
        var status = await apple.GetSubscriptionStatusAsync(
            transactionId,
            ApplePayloadMetadataStatusType.AutoRenewableSubscriptionActive,
            cancellationToken: ct);

        return status.Data.Any(group => group.LastTransactions.Count > 0);
    }
}
```

## How-to guides

- [Receive and verify App Store Server Notifications](docs/notifications.md)
- [Read transactions, history and subscriptions](docs/transactions-and-subscriptions.md)
- [Respond to consumption requests (refund requests)](docs/consumption-requests.md)
- [Handle errors, retries and sandbox fallback](docs/error-handling.md)

## Security note

`DecodeJwtPayload` / `DecodeUnverified` **only decode**; they don't check the signature. That is acceptable for data
the library fetched from Apple over TLS, but anything that reaches your server another way — notification webhooks,
or signed transactions posted by your app — must go through `ISignedDataVerifier`. See
[the notifications guide](docs/notifications.md).

## Building and testing

```bash
dotnet test test/Talegen.Apple.Storekit.Tests
```

See [CHANGELOG.md](CHANGELOG.md) for release notes and breaking changes.

## License

Apache 2.0 — see [LICENSE](LICENSE).
