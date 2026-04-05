namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.LogDtos
{
    public class CreateLogDto
    {
        public string LogDetayi { get; set; }
        public DateTime islemZamani { get; set; } = DateTime.UtcNow;
        public int? MakaleId { get; set; }
    }
}
