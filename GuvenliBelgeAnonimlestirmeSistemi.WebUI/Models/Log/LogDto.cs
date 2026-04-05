namespace GuvenliBelgeAnonimlestirmeSistemi.WebUI.Models.Log
{
    public class LogDto
    {
        public int LogId { get; set; }
        public string LogDetayi { get; set; }
        public DateTime islemZamani { get; set; } = DateTime.UtcNow;
        public int? MakaleId { get; set; }
    }
}
