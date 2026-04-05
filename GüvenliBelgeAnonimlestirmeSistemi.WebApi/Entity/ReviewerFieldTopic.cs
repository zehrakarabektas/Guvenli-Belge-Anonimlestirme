namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity
{
    public class ReviewerFieldTopic
    {
        public int Id { get; set; }
        public int ReviewerId { get; set; }
        public Reviewer Reviewer { get; set; }

        public int FieldTopicId { get; set; }
        public FieldTopic AltKonular { get; set; }
    }
}
