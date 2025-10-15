namespace TestAppKit
{
    using Talegen.Apple.Storekit.Models.Settings;
    using Talegen.Apple.Storekit.Client;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Talegen.Apple.Storekit.Extensions;
    using static System.Net.Mime.MediaTypeNames;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Apple Kit Tester!");
            var builder = new HostBuilder()
               .ConfigureServices((hostContext, services) =>
               {
                   services.AddHttpClient();
                   services.AddAppleApiClient(config =>
                   {


                       config.IssuerId = "9e59125a-eae7-4bfb-a8b6-d650b52c2849";
                       config.KeyId = "4QZCKM7669";
                       config.BundleId = "com.sweptdate.app";
                       config.Environment = Talegen.Apple.Storekit.Models.EnvironmentType.Production;
                       config.PrivateKey = "-----BEGIN PRIVATE KEY-----\nMIGTAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBHkwdwIBAQQg/ZnBax74oADZaoUF\nIcQ4FSzKBatMlsmoqsf9jauHFD+gCgYIKoZIzj0DAQehRANCAARXJlU8o/JxTd7M\nZWlCdB6+eO/iXm3ZRcRgXh/9pm8Dtz5b/sUxiQiuvf4cNxuwS3aPt+BvaHB34ViN\n2cRtrx2n\n-----END PRIVATE KEY-----";

                   });
                   services.AddTransient<TestApp>();
               }).UseConsoleLifetime();
            var host = builder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    string testTransactionId = "1000000826908824";
                    var myService = services.GetRequiredService<TestApp>();
                    await myService.Run(testTransactionId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Occured");
                }
            }
        }
    }
}
