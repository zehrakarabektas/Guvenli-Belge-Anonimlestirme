using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.ArticleFieldsDto
{
    public class CreateArticleFieldsDto
    {
        public int MakaleId { get; set; }

        public int FieldTopicId { get; set; }
        public double Skor { get; set; }
    }
}
