using AzureChatFlow.Common.Enum;
using AzureChatFlow.Common.Helper;
using AzureChatFlow.Common.Model;
using AzureChatFlow.DAL.Entities;
using AzureChatFlow.DAL.Repositories;
using AzureChatFlow.Infrastructure.ConnectionMap;
using AzureChatFlow.Infrastructure.SignalR;
using Microsoft.Extensions.Logging;
using static AzureChatFlow.Common.Enum.ChatEnum;

namespace AzureChatFlow.Service
{
    public class ChatMessageService: IChatMessageService
    {
        private readonly ConnectionMap _connectionMap;
        private readonly ISignalRClient _signalRClient;
        private readonly ILogger<ChatMessageService> _logger;
        private readonly IChatHistoryRepository _chatHistoryRepository; 

        public ChatMessageService(ConnectionMap connectionMap, 
            ISignalRClient signalRClient,
            ILogger<ChatMessageService> logger,
            IChatHistoryRepository chatHistoryRepository)
        {
            _connectionMap = connectionMap ?? throw new ArgumentNullException(nameof(connectionMap));
            _signalRClient = signalRClient ?? throw new ArgumentNullException(nameof(signalRClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _chatHistoryRepository = chatHistoryRepository ?? throw new ArgumentNullException(nameof(chatHistoryRepository));
        }

        public async Task<bool> SendMessageAsync(MessageModel messageData)
        {
            _logger.LogInformation($"Message from SenderId={messageData.SenderId} to ReceiverId={messageData.ReceiverId}: {messageData.Message}: {messageData.SendAt}");

            string connectionId = _connectionMap.Get(messageData.ReceiverId) ?? string.Empty;

            try
            {
                messageData.ReadStatus = string.IsNullOrEmpty(connectionId) ? ChatEnum.ReadStatus.NotSent : ChatEnum.ReadStatus.Sent;
                ChatHistoryEntity chatData = new ChatHistoryEntity(messageData);
                await _chatHistoryRepository.AddEntityAsync(chatData);

                messageData.RowKey = chatData.RowKey;

                if (!string.IsNullOrEmpty(connectionId))
                {
                    await _signalRClient.SendToClientAsync(connectionId, "ReceiveMessage", messageData);
                    _logger.LogInformation($"Message sent to ConnectionId={connectionId} with data={messageData}");
                }
                else
                {
                    _logger.LogInformation($"ReceiverId={messageData.ReceiverId} is offline; message stored but not sent.");
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message from {SenderId} to {ReceiverId}", messageData.SenderId, messageData.ReceiverId);
                return false;
            }
        }

        public async Task<List<MessageModel>> GetChatHistoryAsync(string senderId, string receiverId, int? maxResults)
        {
            string partitionKey = StringHelper.CreatePartitionKey(senderId, receiverId);

            _logger.LogInformation($"Fetching chat history for PartitionKey={partitionKey}, MaxResults={maxResults ?? 0}");

            List<ChatHistoryEntity> orderedHistory = await _chatHistoryRepository.GetEntitiesByPartitionAsync(partitionKey, maxResults);

            return orderedHistory.Select(msg => new MessageModel
            {
                RowKey = msg.RowKey,
                SenderId = msg.SenderId,
                ReceiverId = msg.ReceiverId,
                Message = msg.Message,
                SendAt = msg.Timestamp.HasValue ? msg.Timestamp.Value.DateTime : DateTime.UtcNow,
                ReadStatus = (ReadStatus) msg.ReadStatus
            }).OrderBy(msg=> msg.SendAt).ToList();
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