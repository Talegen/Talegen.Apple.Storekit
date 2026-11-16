# How to receive and verify App Store Server Notifications V2

The App Store POSTs a JSON body `{ "signedPayload": "<JWS>" }` to the URL you configure in App Store Connect
(**App Information → App Store Server Notifications**, separately for Production and Sandbox).

Anyone can POST to that URL, so **always verify the signature** before trusting the payload.

## 1. Download Apple's root certificate

Download **Apple Root CA - G3** (`AppleRootCA-G3.cer`) from <https://www.apple.com/certificateauthority/> and ship it
with your app (for example as a content file or a mounted secret). The verifier trusts only the roots you give it.

## 2. Register the verifier

```csharp
using System.Security.Cryptography.X509Certificates;
using Talegen.Apple.Storekit.Extensions;
using Talegen.Apple.Storekit.Models;

builder.Services.AddAppleSignedDataVerifier(options =>
{
    options.RootCertificates.Add(new X509Certificate2("certs/AppleRootCA-G3.cer"));
    options.BundleId    = "com.example.app";
    options.AppAppleId  = 1234567890;               // checked for Production payloads
    options.Environment = EnvironmentType.Production;
    options.EnableOnlineChecks = false;             // true = OCSP/CRL revocation checks at the current time
});
```

If you receive Production and Sandbox notifications on the same endpoint, leave `Environment` unset and branch on
`payload.Data.Environment` yourself, or register two verifiers.

## 3. Handle the webhook

```csharp
using Talegen.Apple.Storekit.Client;
using Talegen.Apple.Storekit.Models.Api;

app.MapPost("/apple/notifications", async (AppleResponseBodyV2 body, ISignedDataVerifier verifier, ILogger<Program> log) =>
{
    AppleResponseBodyV2DecodedPayload notification;

    try
    {
        notification = verifier.VerifyAndDecodeNotification(body.SignedPayload);
    }
    catch (SignedDataVerificationException ex)
    {
        log.LogWarning(ex, "Rejected App Store notification: {Status}", ex.Status);
        return Results.BadRequest();
    }

    // Notifications can be delivered more than once; de-duplicate on NotificationIdentifier.
    var transaction = notification.Data?.SignedTransactionInfo is { Length: > 0 } signedTransaction
        ? verifier.VerifyAndDecodeTransaction(signedTransaction)
        : null;

    switch (notification.NotificationType)
    {
        case AppleStoreNotificationType.Subscribed:
        case AppleStoreNotificationType.Renewed:
            // grant / extend access until transaction.ExpiresDateUtc
            break;

        case AppleStoreNotificationType.Expired:
        case AppleStoreNotificationType.GracePeriodExpired:
        case AppleStoreNotificationType.Refund:
        case AppleStoreNotificationType.Revoke:
            // remove access
            break;

        case AppleStoreNotificationType.ConsumptionRequest:
            // respond within 12 hours — see consumption-requests.md
            break;

        case AppleStoreNotificationType.Unknown:
            log.LogInformation("Unrecognized notification type; library may need updating.");
            break;
    }

    // Return 200 quickly. Apple retries non-2xx responses (up to 5 times over ~3 days).
    return Results.Ok();
});
```

Notes:

- `SubNotificationType` is `null` when Apple sends no subtype — don't assume a value.
- Notification types and subtypes Apple adds after this library version deserialize to `Unknown` instead of failing.
- Exactly one of `Data`, `Summary` or `ExternalPurchaseToken` is populated.
- `notification.Data.SignedRenewalInfo` can be verified with `verifier.VerifyAndDecodeRenewalInfo(...)`.

## 4. Test your endpoint

```csharp
var sent = await apple.RequestTestNotificationAsync(EnvironmentType.Sandbox);
await Task.Delay(TimeSpan.FromSeconds(5));
var status = await apple.GetTestNotificationStatusAsync(sent.TestNotificationToken, EnvironmentType.Sandbox);

foreach (var attempt in status.SendAttempts)
{
    Console.WriteLine($"{attempt.AttemptDate}: {attempt.SendAttemptResult}");   // SUCCESS, TIMED_OUT, SSL_ISSUE, ...
}
```

## Verifying transactions sent by your app

StoreKit 2 gives your app a `jwsRepresentation` for each transaction. If the app sends it to your server, verify it the
same way before granting anything:

```csharp
var transaction = verifier.VerifyAndDecodeTransaction(jwsFromApp);
```

## What the verifier checks

1. The JWS header uses `ES256` and contains a three-certificate `x5c` chain.
2. The chain builds to one of your configured roots, and the leaf/intermediate carry Apple's marker OIDs
   (`1.2.840.113635.100.6.11.1` and `1.2.840.113635.100.6.2.1`).
3. The signature matches the leaf certificate's public key.
4. The bundle ID, app Apple ID and environment match the configured expectations (when set).

Failures throw `SignedDataVerificationException` with a `Status` of `InvalidFormat`, `InvalidCertificate`,
`InvalidSignature`, `InvalidAppIdentifier` or `InvalidEnvironment`.
