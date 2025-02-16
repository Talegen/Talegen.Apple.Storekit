namespace TestAppKit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Talegen.Apple.Storekit.Client;
    using Talegen.Apple.Storekit.Extensions;

    internal class TestApp
    {
        private readonly ILogger logger;

        private readonly IAppStoreServerApiClient client;

        public TestApp(ILogger<TestApp> logger, IAppStoreServerApiClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        public async Task Run(string testTransactionId)
        {
            this.logger.LogInformation("Application {applicationEvent} at {dateTime}", "Started", DateTime.UtcNow);
            try
            {
                var response = await this.client.GetTransactionAsync(testTransactionId);
            }
            catch (AppleApiException aex)
            {
                if (aex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    this.logger.LogWarning("The specified resource was not found.");

                    try
                    {
                        // try sandbox
                        var response = await this.client.GetTransactionAsync(testTransactionId, environment: Talegen.Apple.Storekit.Models.EnvironmentType.Sandbox);

                    }
                    catch (AppleApiException aex2)
                    {
                        this.logger.LogError(aex2, "An error occured while processing your request.");
                    }
                }
                else
                {
                    this.logger.LogError(aex, "An error occured while processing your request.");

                }
            }
        }
    }
}
