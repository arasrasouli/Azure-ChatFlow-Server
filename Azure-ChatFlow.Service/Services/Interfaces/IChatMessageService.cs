using AzureChatFlow.Common.Model;

namespace AzureChatFlow.Service
{
    public interface IChatMessageService
    {
        Task<bool> SendMessageAsync(MessageModel messageData);
        Task<ConnectionInfoModel> NegotiateAsync(string requestBody);
        Task RegisterConnectionAsync(ConnectionRegistrationModel connectionData);
        Task<bool> UnregisterConnectionAsync(string userId);
        Task<List<MessageModel>> GetChatHistoryAsync(string senderId, string receiverId, int? maxResults);
    }
}
