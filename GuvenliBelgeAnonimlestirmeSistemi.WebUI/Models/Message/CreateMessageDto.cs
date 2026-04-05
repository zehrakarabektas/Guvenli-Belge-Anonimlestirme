namespace GuvenliBelgeAnonimlestirmeSistemi.WebUI.Models.Message
{
    public class CreateMessageDto
    {
        public string MessageContent { get; set; }
        public int MakaleId { get; set; }
        public DateTime SendTime { get; set; } = DateTime.UtcNow;
        public SenderRole SendRol { get; set; }
    }

}
