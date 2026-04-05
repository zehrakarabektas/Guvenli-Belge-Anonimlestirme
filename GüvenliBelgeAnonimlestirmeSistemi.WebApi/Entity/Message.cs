using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity
{
    public enum SenderRole
    {
        Editor=0,
        Author=1
    }
    public class Message
    {
        public int MessageId { get; set; }
        public string MessageContent { get; set; }
        [ForeignKey("Article")]
        public int MakaleId { get; set; }
        public Article Makale { get; set; }
        public DateTime SendTime { get;set; } = DateTime.UtcNow;
        public SenderRole SendRol { get; set; }
    }
}
