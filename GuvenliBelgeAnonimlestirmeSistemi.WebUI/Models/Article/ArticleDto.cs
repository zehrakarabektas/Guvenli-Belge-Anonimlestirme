namespace GuvenliBelgeAnonimlestirmeSistemi.WebUI.Models.Article
{
    public class ArticleDto
    {
        public int MakaleId { get; set; }
        public string YazarEPosta { get; set; }
        public string PdfFilePath { get; set; }
        public string TakipNo { get; set; }
        public string? AnonimPdfFilePath { get; set; }
        public string? SonucPdfFilePath { get; set; }
        public string? HakemDegerlendirmesi { get; set; }
        public string? EncryptedInfoJson { get; set; }
        public DateTime MakaleYuklemeTarihi { get; set; }
        public DateTime? EnSonYapilanIsleminTarihi { get; set; }
        public ArticleStatus MakaleDurumu { get; set; }
        public int? ReviewerId { get; set; }
    }
}
