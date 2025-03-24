using System.Xml;
using static AzureChatFlow.Common.Enum.ChatEnum;

namespace AzureChatFlow.Common.Model
{
    public class MessageModel
    {
        public string RowKey { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime SendAt { get; set; }
        public ReadStatus ReadStatus { get; set; } = ReadStatus.NotSent;
    }
}
