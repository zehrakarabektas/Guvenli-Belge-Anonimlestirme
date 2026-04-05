using System.ComponentModel.DataAnnotations;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity
{
    public enum ArticleStatus
    {
        MakaleBeklemede = 0,
        RevizeEdildi = 1,
        EditorIncelemede = 2,
        HakemeAtandi = 3,
        HakemDegerlendirilmesinde = 4,
        HakemDegerlendirdi = 5,
        EditorSonuclandirmada = 6,
        MakaleSonuclandirildi = 7,
        MakaleSonucuYazaraIletildi = 8
    }
    public class Article
    {
        [Key]
        public int MakaleId { get; set; }
        public string YazarEPosta { get; set; }
        public string PdfFilePath { get; set; }
        public string TakipNo { get; set; }
        public string? AnonimPdfFilePath { get; set; }
        public string? SonucPdfFilePath { get; set; }
        public DateTime MakaleYuklemeTarihi { get; set; }
        public DateTime? EnSonYapilanIsleminTarihi { get; set; }
        public ArticleStatus MakaleDurumu { get; set; }
        public string? HakemDegerlendirmesi { get; set; }
        public string? EncryptedInfoJson { get; set; }
        public int? ReviewerId { get; set; }
        public Reviewer Hakem { get; set; }
        public List<ArticleField> ArticleFields { get; set; }
        public List<Message> Messages { get; set; } 

    }
}
