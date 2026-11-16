namespace Talegen.Apple.Storekit.Tests.Extensions
{
    using System.Security.Cryptography;
    using Microsoft.Extensions.DependencyInjection;
    using Talegen.Apple.Storekit.Client;
    using Talegen.Apple.Storekit.Extensions;
    using Talegen.Apple.Storekit.Tests.TestSupport;

    public class ServiceExtensionsTests
    {
        [Fact]
        public void AddAppleApiClient_ResolvesClient_WithoutCallerRegisteringHttpClient()
        {
            using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var services = new ServiceCollection();
            services.AddAppleApiClient(o =>
            {
                o.KeyId = "k";
                o.IssuerId = "i";
                o.BundleId = "b";
                o.PrivateKey = key.ExportPkcs8PrivateKeyPem();
            });

            using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
            using var scope = provider.CreateScope();

            Assert.IsType<AppStoreServerApiClient>(scope.ServiceProvider.GetRequiredService<IAppStoreServerApiClient>());
            Assert.Same(provider.GetRequiredService<IBearerTokenAuthenticator>(), scope.ServiceProvider.GetRequiredService<IBearerTokenAuthenticator>());
        }

        [Fact]
        public void AddAppleSignedDataVerifier_ResolvesVerifier()
        {
            using var chain = TestCertificateChain.Create();
            var services = new ServiceCollection();
            services.AddAppleSignedDataVerifier(o => o.RootCertificates.Add(chain.Root));

            using var provider = services.BuildServiceProvider();

            Assert.IsType<SignedDataVerifier>(provider.GetRequiredService<ISignedDataVerifier>());
        }

        [Fact]
        public void Registration_RejectsNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => ServiceExtensions.AddAppleApiClient(null!, _ => { }));
            Assert.Throws<ArgumentNullException>(() => new ServiceCollection().AddAppleApiClient(null!));
        }
    }
}
