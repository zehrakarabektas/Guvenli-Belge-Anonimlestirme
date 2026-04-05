using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.ArticleFieldsDto
{
    public class ArticleFieldsDto
    {
        public int Id { get; set; }
        public int MakaleId { get; set; }
        public int FieldTopicId { get; set; }
        public double Skor { get; set; }
    }
}
