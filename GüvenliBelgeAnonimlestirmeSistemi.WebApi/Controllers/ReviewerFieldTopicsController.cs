using AutoMapper;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Context;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.ReviewerDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.ReviewerFieldTopicDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerFieldTopicsController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;

        public ReviewerFieldTopicsController(ApiContext context, IMapper mapper)
        {
            _context=context;
            _mapper=mapper;
        }

        [HttpGet]
        public IActionResult ReviewerFieldTopicList()
        {
            var values = _context.ReviewerFieldTopics.ToList();
            return Ok(_mapper.Map<List<ReviewerFieldTopicDto>>(values));
        }
        [HttpPost]
        public IActionResult CreateReviewerFieldTopic(CreateReviewerFieldTopicDto field)
        {
            var value = _mapper.Map<ReviewerFieldTopic>(field);
            _context.ReviewerFieldTopics.Add(value);
            _context.SaveChanges();
            return Ok("Hakem ilgi alanı ekleme işlemi başarılı.");
        }
        [HttpDelete]
        public IActionResult DeleteReviewerFieldTopic(int id)
        {
            var value = _context.ReviewerFieldTopics.Find(id);
            _context.ReviewerFieldTopics.Remove(value);
            _context.SaveChanges();
            return Ok("Hakem ilgi alanı başarılı bir şekilde silindi.");
        }
        [HttpGet("GetReviewerFieldTopic")]
        public IActionResult GetReviewerFieldTopic(int id)
        {
            var value = _context.ReviewerFieldTopics.Find(id);
            return Ok(_mapper.Map<ReviewerFieldTopicDto>(value));
        }
        [HttpPut]
        public IActionResult UpdateReviewerFieldTopic(ReviewerFieldTopicDto field)
        {
            var value = _mapper.Map<ReviewerFieldTopic>(field);
            _context.ReviewerFieldTopics.Update(value);
            _context.SaveChanges();
            return Ok("Hakem ilgi alanı başarılı bir şekilde güncellendi.");
        }
        [HttpGet("GetFieldsByReviewerId")]
        public IActionResult GetFieldsByReviewerId(int reviewerId)
        {
            var reviewerFields = _context.ReviewerFieldTopics.Where(f => f.ReviewerId == reviewerId).ToList();

            if (reviewerFields == null || !reviewerFields.Any())
            {
                return NotFound("Bu hakeme ait uzmanlık alanı bulunamadı.");
            }

            return Ok(_mapper.Map<List<ReviewerFieldTopicDto>>(reviewerFields));
        }
        [HttpGet("GetFieldsByFieldTopicId")]
        public IActionResult GetFieldsByFieldTopicId(int fieldTopicId)
        {
            var values = _context.ReviewerFieldTopics.Where(f => f.FieldTopicId == fieldTopicId).ToList();

            if (values == null || !values.Any())
            {
                return NotFound("Bu hakeme ait uzmanlık alanı bulunamadı.");
            }

            return Ok(_mapper.Map<List<ReviewerFieldTopicDto>>(values));
        }
        [HttpGet("GetReviewersByFieldId")]
        public IActionResult GetReviewersByFieldId(int fieldId)
        {
            var reviewers = _context.ReviewerFieldTopics
                .Where(rft => rft.FieldTopicId == fieldId) 
                .Include(rft => rft.Reviewer) 
                .Select(rft => new ReviewerDto
                {
                    ReviewerId = rft.Reviewer.ReviewerId,
                    EPosta = rft.Reviewer.EPosta
                })
                .Distinct() 
                .ToList();

            if (!reviewers.Any())
            {
                return NotFound("Bu alana sahip hakem bulunamadı.");
            }

            return Ok(reviewers);
        }

    }
}
