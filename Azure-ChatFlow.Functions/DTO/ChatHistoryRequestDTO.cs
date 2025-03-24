namespace AzureChatFlow.Functions.DTO
{
    public class ChatHistoryRequestDTO
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public int? MaxResults { get; set; } = 0;
    }
}
