using AzureChatFlow.Functions.DTO;
using AzureChatFlow.Infrastructure.ConnectionMap;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureChatFlow.Functions
{
    public class ChatFunction
    {
        private readonly ConnectionMap _connectionMap;
        private readonly ILogger<ChatFunction> _logger;

        public ChatFunction(ConnectionMap connectionMap, ILogger<ChatFunction> logger)
        {
            _connectionMap = connectionMap ?? throw new ArgumentNullException(nameof(connectionMap));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName("negotiate")]
        public async Task<IActionResult> Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "chathub", ConnectionStringSetting = "AzureSignalRConnectionString")] SignalRConnectionInfo connectionInfo)
        {
            _logger.LogInformation("Negotiate function called.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<NegotiateRequestDto>(requestBody);
            string userId = data?.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("No UserId provided in negotiate request.");
                return new BadRequestObjectResult("UserId is required for negotiation.");
            }

            _logger.LogInformation($"Negotiating connection for UserId={userId}");
            string connectionUrlWithUserId = $"{connectionInfo.Url}&userId={Uri.EscapeDataString(userId)}";
            var customConnectionInfo = new
            {
                url = connectionUrlWithUserId,
                accessToken = connectionInfo.AccessToken
            };

            return new OkObjectResult(customConnectionInfo);
        }

        [FunctionName("SendMessage")]
        public async Task<IActionResult> SendMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalR(HubName = "chathub")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            MessageDto messageData = JsonSerializer.Deserialize<MessageDto>(requestBody);
            _logger.LogInformation($"Message from SenderId={messageData.SenderId} to ReceiverId={messageData.ReceiverId}: {messageData.Message}: {messageData.SendAt}");

            string connectionId = _connectionMap.Get(messageData.ReceiverId);
            if (string.IsNullOrEmpty(connectionId))
            {
                _logger.LogWarning($"No ConnectionId found for ReceiverId={messageData.ReceiverId}");
                return new BadRequestObjectResult("Receiver is not online.");
            }

            await signalRMessages.AddAsync(new SignalRMessage
            {
                ConnectionId = connectionId,
                Target = "ReceiveMessage",
                Arguments = new[] { messageData.SenderId, messageData.ReceiverId, messageData.Message, messageData.SendAt.ToString() }
            });
            _logger.LogInformation($"Message sent to ConnectionId={connectionId}");
            _logger.LogInformation($"Message sent={messageData}");
            return new OkResult();
        }

        [FunctionName("RegisterConnection")]
        public async Task<IActionResult> RegisterConnection(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<ConnectionRegistrationDto>(requestBody);
            _connectionMap.Set(data.UserId, data.ConnectionId);
            _logger.LogInformation($"Registered UserId={data.UserId} with ConnectionId={data.ConnectionId}");
            return new OkResult();
        }

        [FunctionName("UnregisterConnection")]
        public async Task<IActionResult> UnregisterConnection(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, ILogger log)
        {
            var data = JsonSerializer.Deserialize<ConnectionRegistrationDto>(await new StreamReader(req.Body).ReadToEndAsync());
            _connectionMap.Delete(data.UserId);
            _logger.LogInformation($"Unregistered UserId={data.UserId}");
            return new OkResult();
        }

        [FunctionName("Ping")]
        public async Task<IActionResult> Ping(
                    [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("Ping request received.");
            return new OkObjectResult(new { status = "online", timestamp = DateTime.UtcNow });
        }
    }
}