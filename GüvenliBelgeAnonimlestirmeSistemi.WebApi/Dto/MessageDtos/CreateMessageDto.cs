using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.MessageDtos
{
    public class CreateMessageDto
    {
        public string MessageContent { get; set; }
        public int MakaleId { get; set; }
        public DateTime SendTime { get; set; } = DateTime.UtcNow;
        public SenderRole SendRol { get; set; }
    }
}
