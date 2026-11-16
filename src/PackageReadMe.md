# Talegen.Apple.Storekit

A .NET client (targets .NET 8 and .NET 10) for the Apple App Store Server API and App Store Server Notifications V2, including verification of
Apple-signed data (JWS).

## Register

```csharp
builder.Services.AddAppleApiClient(options =>
{
    options.IssuerId = "<issuer id>";
    options.KeyId = "<key id>";
    options.BundleId = "com.example.app";
    options.PrivateKey = File.ReadAllText("AuthKey_XXXXXXXXXX.p8");
    options.Environment = EnvironmentType.Production;
});

builder.Services.AddAppleSignedDataVerifier(options =>
{
    options.RootCertificates.Add(new X509Certificate2("AppleRootCA-G3.cer"));
    options.BundleId = "com.example.app";
    options.Environment = EnvironmentType.Production;
});
```

## Use

```csharp
var transaction = await apple.GetTransactionAsync(transactionId);
var status = await apple.GetSubscriptionStatusAsync(transactionId);

await foreach (var t in apple.GetAllTransactionHistoryAsync(transactionId)) { /* ... */ }

var notification = verifier.VerifyAndDecodeNotification(body.SignedPayload);
```

Also available: refund history, order lookup, renewal-date extension, consumption information and test notifications.

Full documentation and how-to guides: https://github.com/Talegen/Talegen.Apple.Storekit
