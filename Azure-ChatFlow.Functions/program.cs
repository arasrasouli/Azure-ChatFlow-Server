using AzureChatFlow.Infrastructure.ConnectionMap;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: WebJobsStartup(typeof(AzureChatFlow.Functions.Program.Startup))]

namespace AzureChatFlow.Functions
{
    public class Program
    {
        public static void Main()
        {
        }

        public class Startup : IWebJobsStartup
        {
            public void Configure(IWebJobsBuilder builder)
            {
                builder.Services.AddSingleton<ConnectionMap>();
                builder.Services.AddLogging(logging =>
                {
                    logging.AddConsole();
                });
            }
        }
    }
}