using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity
{
    public class Field
    {
        [Key]
        public int Id { get; set; }
        public string AlanAdi { get; set; }
        public List<FieldTopic> AlanAltBasliklari { get; set; }
    }
}
