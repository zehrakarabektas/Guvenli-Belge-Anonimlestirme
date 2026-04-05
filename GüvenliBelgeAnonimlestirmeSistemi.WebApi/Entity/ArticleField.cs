using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity
{
    public class ArticleField
    {
        [Key]
        public int Id { get; set; }
        public int MakaleId { get; set; }
        public Article Makale { get; set; }

        public int FieldTopicId { get; set; }  
        public FieldTopic AltKonular { get; set; }
        public double Skor { get; set; }

    }
}
