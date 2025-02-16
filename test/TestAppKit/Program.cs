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
                       config.IssuerId = "<Your IssuerId Guid>";
                       config.KeyId = "<Your Key Id>";
                       config.BundleId = "<Your Bundle Id>";
                       config.Environment = Talegen.Apple.Storekit.Models.EnvironmentType.Production;
                       config.PrivateKey = "-----Your Private Key-----";

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
