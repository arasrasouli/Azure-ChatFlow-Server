using AzureChatFlow.Common.Model;

namespace AzureChatFlow.Infrastructure.SignalR
{
    public interface ISignalRClient
    {
        Task SendToClientAsync(string connectionId, string methodName, MessageModel messageData);
        Task<ConnectionInfoModel> NegotiateConnectionAsync(string userId);
    }
}
