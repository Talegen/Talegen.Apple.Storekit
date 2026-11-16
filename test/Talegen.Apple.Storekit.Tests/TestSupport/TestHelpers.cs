namespace Talegen.Apple.Storekit.Tests.TestSupport
{
    using System.Net;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Text.Json;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Talegen.Apple.Storekit.Client;
    using Talegen.Apple.Storekit.Models;
    using Talegen.Apple.Storekit.Models.Settings;

    /// <summary>
    /// An HTTP handler that records requests and returns queued responses.
    /// </summary>
    internal sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<Func<HttpRequestMessage, HttpResponseMessage>> responses = new();

        public List<(HttpRequestMessage Request, string? Body)> Requests { get; } = new();

        public FakeHttpMessageHandler Enqueue(HttpStatusCode status, string? json = null)
        {
            this.responses.Enqueue(_ => new HttpResponseMessage(status)
            {
                Content = json == null ? new ByteArrayContent(Array.Empty<byte>()) : new StringContent(json, Encoding.UTF8, "application/json"),
            });
            return this;
        }

        public FakeHttpMessageHandler Enqueue(Func<HttpRequestMessage, HttpResponseMessage> responder)
        {
            this.responses.Enqueue(responder);
            return this;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string? body = request.Content == null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            this.Requests.Add((request, body));

            if (this.responses.Count == 0)
            {
                throw new InvalidOperationException($"No response queued for {request.Method} {request.RequestUri}.");
            }

            return this.responses.Dequeue()(request);
        }
    }

    /// <summary>
    /// An <see cref="IHttpClientFactory"/> backed by a single handler.
    /// </summary>
    internal sealed class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler handler;

        public FakeHttpClientFactory(HttpMessageHandler handler) => this.handler = handler;

        public string? LastName { get; private set; }

        public HttpClient CreateClient(string name)
        {
            this.LastName = name;
            return new HttpClient(this.handler, disposeHandler: false);
        }
    }

    /// <summary>
    /// A token authenticator that returns a fixed token.
    /// </summary>
    internal sealed class StaticTokenAuthenticator : IBearerTokenAuthenticator
    {
        public const string Token = "test-token";

        public string GenerateToken() => Token;
    }

    /// <summary>
    /// Builds clients wired to a fake handler.
    /// </summary>
    internal static class ClientFactory
    {
        public static AppStoreServerApiClient Create(FakeHttpMessageHandler handler, EnvironmentType environment = EnvironmentType.Production)
        {
            var settings = Options.Create(new AppleApiSettings { Environment = environment });
            return new AppStoreServerApiClient(new FakeHttpClientFactory(handler), new StaticTokenAuthenticator(), settings);
        }
    }

    /// <summary>
    /// Creates unsigned and signed JWS strings for tests.
    /// </summary>
    internal static class Jws
    {
        /// <summary>
        /// Creates a JWS with a dummy signature, suitable for code paths that only decode.
        /// </summary>
        public static string Unsigned(object payload)
        {
            string header = Base64UrlEncoder.Encode("{\"alg\":\"ES256\"}");
            return $"{header}.{Base64UrlEncoder.Encode(JsonSerializer.SerializeToUtf8Bytes(payload))}.c2ln";
        }

        /// <summary>
        /// Creates a JWS signed by the chain's leaf key with an x5c header.
        /// </summary>
        public static string Signed(object payload, TestCertificateChain chain, string algorithm = "ES256", IEnumerable<X509Certificate2>? x5c = null, ECDsa? signingKey = null)
        {
            var certificates = (x5c ?? new[] { chain.Leaf, chain.Intermediate, chain.Root }).Select(c => Convert.ToBase64String(c.RawData)).ToArray();
            string header = Base64UrlEncoder.Encode(JsonSerializer.SerializeToUtf8Bytes(new { alg = algorithm, x5c = certificates }));
            string body = Base64UrlEncoder.Encode(payload is string s ? Encoding.UTF8.GetBytes(s) : JsonSerializer.SerializeToUtf8Bytes(payload));
            byte[] signature = (signingKey ?? chain.LeafKey).SignData(Encoding.ASCII.GetBytes(header + "." + body), HashAlgorithmName.SHA256, DSASignatureFormat.IeeeP1363FixedFieldConcatenation);
            return $"{header}.{body}.{Base64UrlEncoder.Encode(signature)}";
        }
    }

    /// <summary>
    /// A generated root → intermediate → leaf chain shaped like Apple's App Store signing chain.
    /// </summary>
    internal sealed class TestCertificateChain : IDisposable
    {
        private TestCertificateChain(X509Certificate2 root, X509Certificate2 intermediate, X509Certificate2 leaf, ECDsa leafKey)
        {
            this.Root = root;
            this.Intermediate = intermediate;
            this.Leaf = leaf;
            this.LeafKey = leafKey;
        }

        public X509Certificate2 Root { get; }

        public X509Certificate2 Intermediate { get; }

        public X509Certificate2 Leaf { get; }

        public ECDsa LeafKey { get; }

        public static TestCertificateChain Create(bool includeLeafOid = true, bool includeIntermediateOid = true)
        {
            var notBefore = DateTimeOffset.UtcNow.AddDays(-1);
            var notAfter = DateTimeOffset.UtcNow.AddYears(1);

            using var rootKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var rootRequest = new CertificateRequest("CN=Test Apple Root CA - G3", rootKey, HashAlgorithmName.SHA256);
            rootRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
            rootRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, true));
            rootRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(rootRequest.PublicKey, false));
            var root = rootRequest.CreateSelfSigned(notBefore, notAfter.AddYears(1));

            using var intermediateKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var intermediateRequest = new CertificateRequest("CN=Test Apple WWDR", intermediateKey, HashAlgorithmName.SHA256);
            intermediateRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, true, 0, true));
            intermediateRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, true));
            intermediateRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(intermediateRequest.PublicKey, false));
            if (includeIntermediateOid)
            {
                intermediateRequest.CertificateExtensions.Add(new X509Extension(SignedDataVerifier.IntermediateCertificateOid, new byte[] { 0x05, 0x00 }, false));
            }

            var intermediatePublic = intermediateRequest.Create(root, notBefore, notAfter.AddMonths(6), new byte[] { 1, 2, 3, 4 });
            var intermediate = intermediatePublic.CopyWithPrivateKey(intermediateKey);

            var leafKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var leafRequest = new CertificateRequest("CN=Test Prod ECC Mac App Store and iTunes Store Receipt Signing", leafKey, HashAlgorithmName.SHA256);
            leafRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
            leafRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));
            if (includeLeafOid)
            {
                leafRequest.CertificateExtensions.Add(new X509Extension(SignedDataVerifier.LeafCertificateOid, new byte[] { 0x05, 0x00 }, false));
            }

            var leaf = leafRequest.Create(intermediate, notBefore, notAfter, new byte[] { 5, 6, 7, 8 });

            return new TestCertificateChain(
                new X509Certificate2(root.Export(X509ContentType.Cert)),
                new X509Certificate2(intermediate.Export(X509ContentType.Cert)),
                new X509Certificate2(leaf.RawData),
                leafKey);
        }

        public void Dispose()
        {
            this.Root.Dispose();
            this.Intermediate.Dispose();
            this.Leaf.Dispose();
            this.LeafKey.Dispose();
        }
    }
}
