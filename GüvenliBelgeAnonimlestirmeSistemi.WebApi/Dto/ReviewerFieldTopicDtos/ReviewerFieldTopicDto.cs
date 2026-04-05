using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.ReviewerFieldTopicDtos
{
    public class ReviewerFieldTopicDto
    {
        public int Id { get; set; }
        public int ReviewerId { get; set; }
        public int FieldTopicId { get; set; }
    }
}
