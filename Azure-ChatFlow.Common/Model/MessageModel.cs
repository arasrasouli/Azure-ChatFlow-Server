namespace AzureChatFlow.Common.Model
{
    public class MessageModel
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime SendAt { get; set; }
    }
}
