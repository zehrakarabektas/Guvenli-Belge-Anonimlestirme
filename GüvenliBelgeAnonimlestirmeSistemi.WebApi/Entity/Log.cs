using System.ComponentModel.DataAnnotations;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity
{
    public class Log
    {
        [Key]
        public int LogId { get; set; }
        public string LogDetayi { get; set; }
        public DateTime islemZamani { get; set; } = DateTime.UtcNow;
        public int? MakaleId { get; set; } 
        public Article Makale { get; set; }
    }
}
