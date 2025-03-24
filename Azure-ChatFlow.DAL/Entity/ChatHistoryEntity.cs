using Azure.Data.Tables;
using AzureChatFlow.Common.Helper;
using AzureChatFlow.Common.Model;

namespace AzureChatFlow.DAL.Entities
{
    public class ChatHistoryEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public Azure.ETag ETag { get; set; }

        public string Message { get; set; } = string.Empty;
        public int ReadStatus { get; set; } = 0;
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }

        public ChatHistoryEntity() { }

        public ChatHistoryEntity(MessageModel messageData)
        {
            PartitionKey = StringHelper.CreatePartitionKey(messageData.SenderId, messageData.ReceiverId);
            RowKey = messageData.SendAt.Ticks.ToString();
            Message = messageData.Message;
            SenderId = messageData.SenderId;
            ReceiverId = messageData.ReceiverId;
            Timestamp = messageData.SendAt;
            ReadStatus = (int)messageData.ReadStatus;
        }
    }
}