namespace Talegen.Apple.Storekit.Tests.Client
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Cryptography;
    using Microsoft.Extensions.Time.Testing;
    using Microsoft.IdentityModel.Tokens;
    using Talegen.Apple.Storekit.Client;
    using Talegen.Apple.Storekit.Models.Settings;

    public class BearerTokenAuthenticatorTests
    {
        private static (string Pem, ECDsa Key) CreateKey()
        {
            var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            return (key.ExportPkcs8PrivateKeyPem(), key);
        }

        [Fact]
        public void GenerateToken_ProducesAppleCompliantEs256Jwt()
        {
            var (pem, key) = CreateKey();
            using var keyScope = key;
            var time = new FakeTimeProvider(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
            var authenticator = new BearerTokenAuthenticator("KEY123", "issuer-guid", "com.example.app", pem, null, time);

            string token = authenticator.GenerateToken();
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            Assert.Equal("ES256", jwt.Header.Alg);
            Assert.Equal("KEY123", jwt.Header.Kid);
            Assert.Equal("issuer-guid", jwt.Payload.Iss);
            Assert.Equal("appstoreconnect-v1", Assert.Single(jwt.Payload.Aud));
            Assert.Equal("com.example.app", jwt.Payload["bid"]);
            Assert.Equal(time.GetUtcNow().ToUnixTimeSeconds(), jwt.Payload.IssuedAt.Subtract(DateTime.UnixEpoch).TotalSeconds);
            Assert.Equal(time.GetUtcNow().AddMinutes(5).ToUnixTimeSeconds(), (long)jwt.Payload.Expiration!);

            // the signature must verify with the matching public key.
            var parameters = new TokenValidationParameters
            {
                IssuerSigningKey = new ECDsaSecurityKey(ECDsa.Create(key.ExportParameters(false))),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
            };
            new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _);
        }

        [Fact]
        public void GenerateToken_IsCached_UntilNearExpiry()
        {
            var (pem, key) = CreateKey();
            using var keyScope = key;
            var time = new FakeTimeProvider(DateTimeOffset.UtcNow);
            var authenticator = new BearerTokenAuthenticator("KEY123", "issuer", "bundle", pem, null, time);

            string first = authenticator.GenerateToken();
            time.Advance(TimeSpan.FromMinutes(3));
            Assert.Same(first, authenticator.GenerateToken());

            time.Advance(TimeSpan.FromMinutes(1.5));
            Assert.NotEqual(first, authenticator.GenerateToken());
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Constructor_AcceptsPemOrBareBase64(bool stripHeaders)
        {
            var (pem, key) = CreateKey();
            using var keyScope = key;
            string text = stripHeaders
                ? pem.Replace("-----BEGIN PRIVATE KEY-----", string.Empty).Replace("-----END PRIVATE KEY-----", string.Empty)
                : pem.Replace("\n", "\r\n");

            var authenticator = new BearerTokenAuthenticator(new AppleApiSettings { KeyId = "k", IssuerId = "i", BundleId = "b", PrivateKey = text });

            Assert.False(string.IsNullOrEmpty(authenticator.GenerateToken()));
        }

        [Fact]
        public void Constructor_InvalidKey_FailsFast()
        {
            Assert.Throws<ArgumentException>(() => new BearerTokenAuthenticator("k", "i", "b", "not base64 !!"));
            Assert.ThrowsAny<CryptographicException>(() => new BearerTokenAuthenticator("k", "i", "b", Convert.ToBase64String(new byte[] { 1, 2, 3 })));
        }

        [Theory]
        [InlineData(null, "i", "b")]
        [InlineData("k", "", "b")]
        [InlineData("k", "i", " ")]
        public void Constructor_MissingSettings_Throw(string? keyId, string issuerId, string bundleId)
        {
            var (pem, key) = CreateKey();
            using var keyScope = key;
            Assert.ThrowsAny<ArgumentException>(() => new BearerTokenAuthenticator(keyId!, issuerId, bundleId, pem));
        }
    }
}
