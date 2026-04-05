using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.ReviewerFieldTopicDtos
{
    public class CreateReviewerFieldTopicDto
    {
        public int ReviewerId { get; set; }
        public int FieldTopicId { get; set; }
    }
}
