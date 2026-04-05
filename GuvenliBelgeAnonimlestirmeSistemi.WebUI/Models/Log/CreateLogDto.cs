namespace GuvenliBelgeAnonimlestirmeSistemi.WebUI.Models.Log
{
    public class CreateLogDto
    {
        public string LogDetayi { get; set; }
        public DateTime islemZamani { get; set; } = DateTime.UtcNow;
        public int? MakaleId { get; set; }
    }
}
