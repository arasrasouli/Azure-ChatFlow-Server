namespace AzureChatFlow.Functions.DTO
{
    public class MessageDTO
    {
        public string RowKey { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime SendAt { get; set; }
        public int ReadStatus { get; set; } = 0;
    }
}
