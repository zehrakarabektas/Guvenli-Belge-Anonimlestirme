namespace GuvenliBelgeAnonimlestirmeSistemi.WebUI.Models.Message
{
    public class MessageDto
    {
        public int MessageId { get; set; }
        public string MessageContent { get; set; }
        public int MakaleId { get; set; }
        public DateTime SendTime { get; set; } = DateTime.UtcNow;
        public SenderRole SendRol { get; set; }
    }
}
