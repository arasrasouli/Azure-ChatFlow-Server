using AzureChatFlow.Infrastructure.ConnectionMap;
using AzureChatFlow.Infrastructure.SignalR;
using AzureChatFlow.Service;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzureChatFlow.Functions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWebApplication()
                .ConfigureServices(async services =>
                {
                    services.AddSingleton<ConnectionMap>();

                    services.AddLogging(logging =>
                    {
                        logging.AddConsole();
                    });

                    var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                    var connectionString = configuration["AzureSignalRConnectionString"];
                    var serviceManager = new ServiceManagerBuilder()
                                        .WithOptions(option =>
                                        {
                                            option.ConnectionString = connectionString;
                                        })
                                        .BuildServiceManager();

                    var hubContext = await serviceManager.CreateHubContextAsync("chathub", new CancellationToken());
                    services.AddSingleton(hubContext);

                    services.AddSingleton<ISignalRClient, SignalRClient>();

                    services.AddSingleton<ChatMessageService>();
                })
                .Build();

            host.Run();
        }
    }
}