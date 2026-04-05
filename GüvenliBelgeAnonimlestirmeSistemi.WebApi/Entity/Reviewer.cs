using System.ComponentModel.DataAnnotations;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity
{
    public class Reviewer
    {
        [Key]
        public int ReviewerId { get; set; }
        public string EPosta { get; set; }
        public List<ReviewerFieldTopic> HakemIlgiAlanlari { get; set; }
        public List<Article> Makaleler { get; set; }
        
    }
}
