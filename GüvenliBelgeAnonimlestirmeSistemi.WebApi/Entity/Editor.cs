using System.ComponentModel.DataAnnotations;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity
{
    public class Editor
    {
        [Key]
        public int EditorId { get; set; }
        public string EPosta { get; set; }
    }
}
