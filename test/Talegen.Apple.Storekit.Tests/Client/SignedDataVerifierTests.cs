namespace Talegen.Apple.Storekit.Tests.Client
{
    using System.Security.Cryptography;
    using Talegen.Apple.Storekit.Client;
    using Talegen.Apple.Storekit.Models;
    using Talegen.Apple.Storekit.Models.Api;
    using Talegen.Apple.Storekit.Tests.TestSupport;

    public sealed class SignedDataVerifierTests : IDisposable
    {
        private readonly TestCertificateChain chain = TestCertificateChain.Create();

        private static object Notification(string bundleId = "com.example.app", string environment = "Production", long appAppleId = 1234) => new
        {
            notificationType = "DID_RENEW",
            notificationUUID = Guid.NewGuid(),
            version = "2.0",
            signedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            data = new { bundleId, environment, appAppleId, status = 1, signedTransactionInfo = "a.b.c" },
        };

        private SignedDataVerifier CreateVerifier(Action<SignedDataVerifierOptions>? configure = null)
        {
            var options = new SignedDataVerifierOptions { BundleId = "com.example.app", Environment = EnvironmentType.Production, AppAppleId = 1234 };
            options.RootCertificates.Add(this.chain.Root);
            configure?.Invoke(options);
            return new SignedDataVerifier(options);
        }

        public void Dispose() => this.chain.Dispose();

        [Fact]
        public void VerifyAndDecodeNotification_ValidSignature_ReturnsPayload()
        {
            var payload = this.CreateVerifier().VerifyAndDecodeNotification(Jws.Signed(Notification(), this.chain));

            Assert.Equal(AppleStoreNotificationType.Renewed, payload.NotificationType);
            Assert.Equal("com.example.app", payload.Data!.BundleId);
        }

        [Fact]
        public void VerifyAndDecodeTransaction_ValidSignature_ReturnsPayload()
        {
            string jws = Jws.Signed(new { transactionId = "t1", bundleId = "com.example.app", environment = "Production", signedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, this.chain);
            Assert.Equal("t1", this.CreateVerifier().VerifyAndDecodeTransaction(jws).TransactionId);
        }

        [Fact]
        public void VerifyAndDecodeRenewalInfo_ValidSignature_ReturnsPayload()
        {
            string jws = Jws.Signed(new { autoRenewStatus = 1, environment = "Production", signedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, this.chain);
            Assert.Equal(AppleAutoRenewStatuses.AutomaticRenewalOn, this.CreateVerifier().VerifyAndDecodeRenewalInfo(jws).AutoRenewStatus);
        }

        [Fact]
        public void TamperedPayload_IsRejected()
        {
            string[] parts = Jws.Signed(Notification(), this.chain).Split('.');
            string forgedPayload = Jws.Unsigned(Notification(bundleId: "com.example.app")).Split('.')[1];

            var ex = Assert.Throws<SignedDataVerificationException>(() => this.CreateVerifier().VerifyAndDecodeNotification($"{parts[0]}.{forgedPayload}.{parts[2]}"));
            Assert.Equal(VerificationStatus.InvalidSignature, ex.Status);
        }

        [Fact]
        public void SignedByDifferentKey_IsRejected()
        {
            using var otherKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var ex = Assert.Throws<SignedDataVerificationException>(() => this.CreateVerifier().VerifyAndDecodeNotification(Jws.Signed(Notification(), this.chain, signingKey: otherKey)));
            Assert.Equal(VerificationStatus.InvalidSignature, ex.Status);
        }

        [Fact]
        public void ChainFromUntrustedRoot_IsRejected()
        {
            using var attacker = TestCertificateChain.Create();
            var ex = Assert.Throws<SignedDataVerificationException>(() => this.CreateVerifier().VerifyAndDecodeNotification(Jws.Signed(Notification(), attacker)));
            Assert.Equal(VerificationStatus.InvalidCertificate, ex.Status);
        }

        [Fact]
        public void LeafWithoutAppleOid_IsRejected()
        {
            using var noOid = TestCertificateChain.Create(includeLeafOid: false);
            var verifier = this.CreateVerifier(o => { o.RootCertificates.Clear(); o.RootCertificates.Add(noOid.Root); });

            var ex = Assert.Throws<SignedDataVerificationException>(() => verifier.VerifyAndDecodeNotification(Jws.Signed(Notification(), noOid)));
            Assert.Equal(VerificationStatus.InvalidCertificate, ex.Status);
        }

        [Fact]
        public void IntermediateWithoutAppleOid_IsRejected()
        {
            using var noOid = TestCertificateChain.Create(includeIntermediateOid: false);
            var verifier = this.CreateVerifier(o => { o.RootCertificates.Clear(); o.RootCertificates.Add(noOid.Root); });

            var ex = Assert.Throws<SignedDataVerificationException>(() => verifier.VerifyAndDecodeNotification(Jws.Signed(Notification(), noOid)));
            Assert.Equal(VerificationStatus.InvalidCertificate, ex.Status);
        }

        [Fact]
        public void ShortCertificateChain_IsRejected()
        {
            string jws = Jws.Signed(Notification(), this.chain, x5c: new[] { this.chain.Leaf, this.chain.Intermediate });
            var ex = Assert.Throws<SignedDataVerificationException>(() => this.CreateVerifier().VerifyAndDecodeNotification(jws));
            Assert.Equal(VerificationStatus.InvalidCertificate, ex.Status);
        }

        [Fact]
        public void UnsupportedAlgorithm_IsRejected()
        {
            var ex = Assert.Throws<SignedDataVerificationException>(() => this.CreateVerifier().VerifyAndDecodeNotification(Jws.Signed(Notification(), this.chain, algorithm: "none")));
            Assert.Equal(VerificationStatus.InvalidFormat, ex.Status);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("a.b.c")]
        public void MalformedInput_IsRejected(string value)
        {
            var ex = Assert.Throws<SignedDataVerificationException>(() => this.CreateVerifier().VerifyAndDecodeNotification(value));
            Assert.Equal(VerificationStatus.InvalidFormat, ex.Status);
        }

        [Fact]
        public void WrongBundleId_IsRejected()
        {
            var ex = Assert.Throws<SignedDataVerificationException>(() => this.CreateVerifier().VerifyAndDecodeNotification(Jws.Signed(Notification(bundleId: "com.other.app"), this.chain)));
            Assert.Equal(VerificationStatus.InvalidAppIdentifier, ex.Status);
        }

        [Fact]
        public void WrongAppAppleId_InProduction_IsRejected()
        {
            var ex = Assert.Throws<SignedDataVerificationException>(() => this.CreateVerifier().VerifyAndDecodeNotification(Jws.Signed(Notification(appAppleId: 999), this.chain)));
            Assert.Equal(VerificationStatus.InvalidAppIdentifier, ex.Status);
        }

        [Fact]
        public void WrongEnvironment_IsRejected()
        {
            var ex = Assert.Throws<SignedDataVerificationException>(() => this.CreateVerifier().VerifyAndDecodeNotification(Jws.Signed(Notification(environment: "Sandbox"), this.chain)));
            Assert.Equal(VerificationStatus.InvalidEnvironment, ex.Status);
        }

        [Fact]
        public void NoExpectations_AcceptsAnyAppAndEnvironment()
        {
            var verifier = this.CreateVerifier(o => { o.BundleId = null; o.Environment = null; o.AppAppleId = null; });
            var payload = verifier.VerifyAndDecodeNotification(Jws.Signed(Notification(bundleId: "x", environment: "Sandbox"), this.chain));
            Assert.Equal("x", payload.Data!.BundleId);
        }

        [Fact]
        public void Constructor_RequiresRootCertificate()
        {
            Assert.Throws<ArgumentException>(() => new SignedDataVerifier(new SignedDataVerifierOptions()));
        }
    }
}
