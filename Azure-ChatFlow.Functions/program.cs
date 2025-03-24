using Azure.Data.Tables;
using AzureChatFlow.DAL.Repositories;
using AzureChatFlow.Infrastructure.ConnectionMap;
using AzureChatFlow.Infrastructure.SignalR;
using AzureChatFlow.Service;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzureChatFlow.Functions
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = FunctionsApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

            builder.Services.AddSingleton<ConnectionMap>();

            builder.Services.AddLogging(logging =>
            {
                        logging.AddConsole();
            });

            var signalRConnectionString = builder.Configuration[Literals.AzureSignalRConnectionString];
            var serviceManager = new ServiceManagerBuilder()
                .WithOptions(option =>
                {
                    option.ConnectionString = signalRConnectionString;
                }).BuildServiceManager();

            var hubContext = await serviceManager.CreateHubContextAsync("chathub", new CancellationToken());

            builder.Services.AddSingleton(hubContext);


            builder.Services.AddSingleton<ISignalRClient, SignalRClient>();


            builder.Services.AddSingleton<ChatMessageService>();


            builder.Services.AddScoped(typeof(ITableStorageRepository<>), typeof(TableStorageRepository<>));

            string tableStorageConnectionString = builder.Configuration.GetValue<string>(Literals.TableStorageConnectionString);


            builder.Services.AddScoped<IChatHistoryRepository>(sp =>
                    {
                        string tableName = builder.Configuration.GetValue<string>(Literals.ChatTableName);
                        TableClient tableClient = new TableClient(tableStorageConnectionString, tableName);
                        return new ChatHistoryRepository(tableClient);
                    });

            builder.ConfigureFunctionsWebApplication();

            builder.Build().Run();
        }
    }
}