# How to handle errors, retries and sandbox fallback

## AppleApiException

Every failed API call throws `AppleApiException`:

| Property | Meaning |
| --- | --- |
| `HttpStatusCode` | The HTTP status Apple returned (`RequestTimeout` for client timeouts, `ServiceUnavailable` for connection failures). |
| `ErrorCode` | Apple's error code (`AppleErrorCodes`), when Apple returned an error body. |
| `ApiErrorMessage` | Apple's error message, when present. |
| `IsRetryable` | `true` for 429, 5xx, and Apple's "retryable" error codes. |

Cancelling the `CancellationToken` throws `OperationCanceledException`, not `AppleApiException`.

```csharp
try
{
    var transaction = await apple.GetTransactionAsync(transactionId);
}
catch (AppleApiException ex) when (ex.ErrorCode == AppleErrorCodes.TransactionIdNotFound)
{
    // unknown transaction
}
catch (AppleApiException ex) when (ex.IsRetryable)
{
    // back off and retry
}
```

## Production first, then sandbox

App Review tests your production build against the **sandbox**. Apple's guidance is to call production first and fall
back to sandbox when the transaction isn't found:

```csharp
async Task<JwsTransactionDecodedPayload> GetTransactionAnyEnvironmentAsync(IAppStoreServerApiClient apple, string id, CancellationToken ct)
{
    try
    {
        return await apple.GetTransactionAsync(id, EnvironmentType.Production, ct);
    }
    catch (AppleApiException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
    {
        return await apple.GetTransactionAsync(id, EnvironmentType.Sandbox, ct);
    }
}
```

## Retries and timeouts

The client uses the named `HttpClient` `AppStoreServerApiClient.HttpClientName`. Configure it like any other
`IHttpClientFactory` client, for example with `Microsoft.Extensions.Http.Resilience`:

```csharp
builder.Services.AddAppleApiClient(o => builder.Configuration.GetSection("Apple").Bind(o));

builder.Services.AddHttpClient(AppStoreServerApiClient.HttpClientName)
    .ConfigureHttpClient(c => c.Timeout = TimeSpan.FromSeconds(30))
    .AddStandardResilienceHandler();
```

Only retry `PUT` calls such as `ExtendSubscriptionRenewalDateAsync` with the same `RequestIdentifier`, so Apple can
de-duplicate them.

## Authentication failures (401)

A `401` usually means:

- the Issuer ID, Key ID or bundle ID is wrong, or the key was revoked;
- the key is an App Store Connect API key rather than an **In-App Purchase** key;
- the server clock is off by more than a few minutes (tokens are valid for 5 minutes).

The bearer token is cached and reused until about a minute before it expires.
