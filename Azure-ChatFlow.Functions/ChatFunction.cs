using AzureChatFlow.Functions.DTO;
using AzureChatFlow.Common.Model;
using AzureChatFlow.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace AzureChatFlow.Functions
{
    public class ChatFunction
    {
        private readonly ILogger<ChatFunction> _logger;
        private readonly ChatMessageService _chatMessageService;

        public ChatFunction(ILogger<ChatFunction> logger, ChatMessageService chatMessageService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _chatMessageService = chatMessageService ?? throw new ArgumentNullException(nameof(chatMessageService));
        }

        [Function("Negotiate")]
        public async Task<HttpResponseData> Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            FunctionContext context)
        {
            try
            {
                _logger.LogInformation("Negotiate function called.");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var negotiateData = JsonSerializer.Deserialize<NegotiateRequestDto>(requestBody);
                if (negotiateData == null)
                {
                    var negotiateResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await negotiateResponse.WriteStringAsync("Invalid or missing negotiation data.");
                    return negotiateResponse;
                }

                var connectionInfo = await _chatMessageService.NegotiateAsync(negotiateData.UserId);

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync(JsonSerializer.Serialize(connectionInfo));
                return response;
            }
            catch (InvalidOperationException ex)
            {
                var response = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await response.WriteStringAsync(ex.Message);
                return response;
            }
        }

        [Function("SendMessage")]
        public async Task<HttpResponseData> SendMessage(
                    [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
                    FunctionContext context)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            MessageDto messageData = JsonSerializer.Deserialize<MessageDto>(requestBody) ?? new MessageDto();
            bool success = await _chatMessageService.SendMessageAsync(new MessageModel()
            {
                Message = messageData.Message,
                ReceiverId = messageData.ReceiverId,
                SendAt = messageData.SendAt,
                SenderId = messageData.SenderId
            });

            if (!success)
            {
                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                await response.WriteStringAsync("Receiver is not online.");
                return response;
            }

            return req.CreateResponse(HttpStatusCode.OK);
        }

        [Function("RegisterConnection")]
        public async Task<HttpResponseData> RegisterConnection(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            FunctionContext context)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<ConnectionRegistrationDto>(requestBody);

            if (data == null || string.IsNullOrEmpty(data.UserId) || string.IsNullOrEmpty(data.ConnectionId))
            {
                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                await response.WriteStringAsync("Invalid or missing connection data.");
                return response;
            }

            await _chatMessageService.RegisterConnectionAsync(new ConnectionRegistrationModel()
            {
                ConnectionId = data.ConnectionId,
                UserId = data.UserId
            });

            _logger.LogInformation($"Registered UserId={data.UserId} with ConnectionId={data.ConnectionId}");
            return req.CreateResponse(HttpStatusCode.OK);
        }

        [Function("UnregisterConnection")]
        public async Task<HttpResponseData> UnregisterConnection(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            FunctionContext context)
        {
            var data = JsonSerializer.Deserialize<ConnectionRegistrationDto>(await new StreamReader(req.Body).ReadToEndAsync());
            if (data == null || string.IsNullOrEmpty(data.UserId))
            {
                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                await response.WriteStringAsync("Invalid or missing connection data.");
                return response;
            }

            await _chatMessageService.UnregisterConnectionAsync(data.UserId);
            return req.CreateResponse(HttpStatusCode.OK);
        }

        [Function("Ping")]
        public Task<IActionResult> Ping(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            _logger.LogInformation("Ping request received.");
            return Task.FromResult<IActionResult>(new OkObjectResult(new { status = "online", timestamp = DateTime.UtcNow }));
        }
    }
}
