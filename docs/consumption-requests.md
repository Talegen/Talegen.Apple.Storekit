# How to respond to consumption requests

When a customer asks Apple for a refund on a consumable or auto-renewable subscription, the App Store sends a
`CONSUMPTION_REQUEST` notification. You have **12 hours** to reply with consumption information, which Apple uses
when deciding the refund.

Only send data if the customer consented to share it (`CustomerConsented` must be `true`; Apple rejects anything else).

```csharp
using Talegen.Apple.Storekit.Extensions;
using Talegen.Apple.Storekit.Models;
using Talegen.Apple.Storekit.Models.Api;

async Task HandleConsumptionRequestAsync(AppleResponseBodyV2DecodedPayload notification, ISignedDataVerifier verifier, IAppStoreServerApiClient apple)
{
    var transaction = verifier.VerifyAndDecodeTransaction(notification.Data!.SignedTransactionInfo);
    var account = await LoadAccountAsync(transaction.AppAccountToken);   // your own lookup

    var request = new ConsumptionRequest
    {
        CustomerConsented = true,
        AppAccountToken = account.AppAccountToken,              // Guid.Empty if unknown
        AccountTenure = account.CreatedUtc.ToAppleAccountTenure(),
        PlayTime = account.MinutesPlayed.ToAppleTimeUsed(),
        LifetimeDollarsPurchased = account.LifetimeUsdSpent.ToAppleDollarAmount(),
        LifetimeDollarsRefunded = account.LifetimeUsdRefunded.ToAppleDollarAmount(),
        ConsumptionStatus = AppleConsumptionStatus.PartiallyConsumed,
        DeliveryStatus = AppleDeliveryStatus.DeliveredAndWorking,
        Platform = PlatformType.Apple,
        SampleContentProvided = true,
        UserStatus = AppleAccountStatus.Active,
        RefundPreference = AppleRefundPreferenceType.NoPreference,
    };

    await apple.SendConsumptionInfoAsync(transaction.TransactionId, request,
        environment: notification.Data.Environment == "Sandbox" ? EnvironmentType.Sandbox : EnvironmentType.Production);
}
```

## Helper buckets

| Helper | Input | Buckets |
| --- | --- | --- |
| `int.ToAppleTimeUsed()` | minutes | ≤5 min, ≤60 min, ≤6 h, ≤24 h, ≤4 days, ≤16 days, over 16 days (negative → `Undeclared`) |
| `decimal.ToAppleDollarAmount()` | USD | 0, <50, <100, <500, <1000, <2000, ≥2000 (sign ignored) |
| `DateTime.ToAppleAccountTenure()` | account creation (UTC) | ≤3, ≤10, ≤30, ≤90, ≤180, ≤365 days, over 365 days |

`ToAppleAccountTenure(utcNow)` takes an explicit reference time if you need deterministic results.
