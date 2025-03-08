using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureChatFlow.Functions
{
    public class ChatFunction
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
                    [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
                    [SignalRConnectionInfo(HubName = "chatHub", ConnectionStringSetting = "SignalRConnection")] SignalRConnectionInfo connectionInfo,
                    ILogger log)
        {
            log.LogInformation("Negotiate function called.");
            return connectionInfo;
        }

        [FunctionName("SendMessage")]
        public static async Task SendMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] MessageInput input,
            [SignalR(HubName = "chatHub", ConnectionStringSetting = "SignalRConnection")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation($"Message from {input?.User ?? "Unknown"}: {input?.Message ?? "Empty"}");
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "ReceiveMessage",
                    Arguments = new[] { input?.User ?? "Unknown", input?.Message ?? "Empty" }
                });
        }
    }

    public class MessageInput
    {
        public string User { get; set; }
        public string Message { get; set; }
    }
}