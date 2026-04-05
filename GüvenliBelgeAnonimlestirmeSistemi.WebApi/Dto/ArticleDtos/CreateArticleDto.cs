using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.ArticleDtos
{
    public class CreateArticleDto
    {
        public string YazarEPosta { get; set; }
        public string PdfFilePath { get; set; }
        public string TakipNo { get; set; }
        public DateTime MakaleYuklemeTarihi { get; set; }
        public ArticleStatus MakaleDurumu { get; set; }
        public int? ReviewerId { get; set; }
    }
}
