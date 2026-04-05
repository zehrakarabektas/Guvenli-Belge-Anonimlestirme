using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity
{
    public class FieldTopic
    {
        [Key]
        public int FieldTopicId { get; set; }
        public string KonuAdi { get; set; }
        public string KonuAdiEn { get; set; }
        public int FieldId { get; set; }
        public Field Alan { get; set; }
    }
}
