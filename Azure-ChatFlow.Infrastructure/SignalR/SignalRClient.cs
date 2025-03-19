using AzureChatFlow.Common.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.SignalR.Management;

namespace AzureChatFlow.Infrastructure.SignalR
{
    public class SignalRClient : ISignalRClient
    {
        private readonly ServiceHubContext _hubContext;

        public SignalRClient(ServiceHubContext hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public async Task SendToClientAsync(string connectionId, string methodName, MessageModel messageData)
        {
            await _hubContext.Clients.Client(connectionId).SendAsync(methodName, messageData); ;
        }

        public async Task<ConnectionInfoModel> NegotiateConnectionAsync(string userId)
        {
            var connectionInfo = await _hubContext.NegotiateAsync(new NegotiationOptions
            {
                UserId = userId
            });
            return new ConnectionInfoModel()
            {
                Url = connectionInfo.Url,
                AccessToken = connectionInfo.AccessToken
            };
        }
    }
}
