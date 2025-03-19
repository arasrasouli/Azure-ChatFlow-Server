using AzureChatFlow.Common.Model;
using AzureChatFlow.Infrastructure.ConnectionMap;
using AzureChatFlow.Infrastructure.SignalR;
using Microsoft.Extensions.Logging;

namespace AzureChatFlow.Service
{
    public class ChatMessageService: IChatMessageService
    {
        private readonly ConnectionMap _connectionMap;
        private readonly ISignalRClient _signalRClient;
        private readonly ILogger<ChatMessageService> _logger;

        public ChatMessageService(ConnectionMap connectionMap, ISignalRClient signalRClient, ILogger<ChatMessageService> logger)
        {
            _connectionMap = connectionMap ?? throw new ArgumentNullException(nameof(connectionMap));
            _signalRClient = signalRClient ?? throw new ArgumentNullException(nameof(signalRClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> SendMessageAsync(MessageModel messageData)
        {
            _logger.LogInformation($"Message from SenderId={messageData.SenderId} to ReceiverId={messageData.ReceiverId}: {messageData.Message}: {messageData.SendAt}");

            string connectionId = _connectionMap.Get(messageData.ReceiverId) ?? string.Empty;
            if (string.IsNullOrEmpty(connectionId))
            {
                _logger.LogWarning($"No ConnectionId found for ReceiverId={messageData.ReceiverId}");
                return false;
            }

            await _signalRClient.SendToClientAsync(
                connectionId,
                "ReceiveMessage",
                messageData);
            _logger.LogInformation($"Message sent to ConnectionId={connectionId} with data={messageData}");
            return true;
        }

        public async Task<ConnectionInfoModel> NegotiateAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("No UserId provided in negotiate request.");
                throw new InvalidOperationException("UserId is required for negotiation.");
            }

            _logger.LogInformation($"Negotiating connection for UserId={userId}");
            return await _signalRClient.NegotiateConnectionAsync(userId);
        }

        public Task RegisterConnectionAsync(ConnectionRegistrationModel connectionData)
        {
            _connectionMap.Set(connectionData.UserId, connectionData.ConnectionId);
            return Task.CompletedTask;
        }

        public Task<bool> UnregisterConnectionAsync(string userId)
        {
            string? existingConnectionId = _connectionMap.Get(userId);
            if (string.IsNullOrEmpty(existingConnectionId))
            {
                return Task.FromResult(false);
            }

            _connectionMap.Delete(userId);
            return Task.FromResult(true);
        }
    }
}