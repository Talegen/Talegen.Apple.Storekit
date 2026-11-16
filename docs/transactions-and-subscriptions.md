# How to read transactions, history and subscriptions

All methods below are extension methods on `IAppStoreServerApiClient` (`using Talegen.Apple.Storekit.Extensions;`).
Any transaction ID that belongs to the customer works as the lookup key — it doesn't have to be the original one.

## A single transaction

```csharp
JwsTransactionDecodedPayload transaction = await apple.GetTransactionAsync("2000000123456789");

Console.WriteLine($"{transaction.ProductId} bought {transaction.PurchaseDateUtc:u}, " +
                  $"price {transaction.Price / 1000m} {transaction.Currency}");
```

`Price` is in **milliunits** (multiply by 1000), so `10990` means `10.99`.
Date fields are UNIX milliseconds; use the `*Utc` helpers (`PurchaseDateUtc`, `ExpiresDateUtc`, `RevocationDateUtc`, …).

## Transaction history

One page (up to 20 transactions):

```csharp
var page = await apple.GetTransactionHistoryAsync(transactionId, new TransactionHistoryRequest
{
    ProductType = "AUTO_RENEWABLE",      // AUTO_RENEWABLE, NON_RENEWABLE, CONSUMABLE, NON_CONSUMABLE
    Sort = "DESCENDING",
    Revoked = false,
    StartDate = DateTimeOffset.UtcNow.AddYears(-1).ToUnixTimeMilliseconds(),
});

foreach (var t in page.DecodedTransactions) { /* ... */ }
// if page.HasMore, call again with Revision = page.Revision and the same filters
```

Every page, without managing revisions yourself:

```csharp
await foreach (var t in apple.GetAllTransactionHistoryAsync(transactionId, new TransactionHistoryRequest { Sort = "ASCENDING" }))
{
    Console.WriteLine(t.TransactionId);
}
```

> The API supports repeating `productId` / `productType` to filter on several values; this library currently sends
> one value per filter.

## Subscription status

```csharp
var status = await apple.GetSubscriptionStatusAsync(transactionId);

foreach (var group in status.Data)
{
    foreach (var item in group.LastTransactions)
    {
        var transaction = item.TransactionInfoDecodedPayload;
        var renewal = item.RenewalInfoDecodedPayload;

        Console.WriteLine($"{group.SubscriptionGroupIdentifier}: {item.Status}, " +
                          $"expires {transaction.ExpiresDateUtc:u}, auto-renew {renewal.AutoRenewStatus}");
    }
}
```

Filter to one status (active, expired, billing retry, grace period, revoked):

```csharp
var active = await apple.GetSubscriptionStatusAsync(transactionId, ApplePayloadMetadataStatusType.AutoRenewableSubscriptionActive);
```

## Extend a subscription

```csharp
var result = await apple.ExtendSubscriptionRenewalDateAsync(originalTransactionId, new ExtendRenewalDateRequest
{
    ExtendByDays = 7,                                  // 1–90
    ExtendReasonCode = ExtendReasonCode.ServiceIssueOrOutage,
    RequestIdentifier = Guid.NewGuid().ToString(),     // reuse the same value if you retry
});
```

## Refunds and order lookup

```csharp
var refunds = await apple.GetRefundHistoryAsync(transactionId);          // pass refunds.Revision for the next page
var order = await apple.LookUpOrderIdAsync("MK5TTTVWJH");                 // order ID from the customer's receipt email

if (order.Status == OrderLookupStatus.Valid)
{
    foreach (var t in order.DecodedTransactions) { /* ... */ }
}
```

## Environments

The client uses `AppleApiSettings.Environment` (default `Production`). Override per call:

```csharp
var t = await apple.GetTransactionAsync(transactionId, EnvironmentType.Sandbox);
```

The override applies to that call only. See [error-handling.md](error-handling.md) for the "try production, then
sandbox" pattern that App Review requires.
