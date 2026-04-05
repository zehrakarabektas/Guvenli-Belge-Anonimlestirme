using AutoMapper;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Context;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.ReviewerDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewersController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;

        public ReviewersController(ApiContext context, IMapper mapper)
        {
            _context=context;
            _mapper=mapper;
        }

        [HttpGet]
        public IActionResult ReviewerList()
        {
            var values = _context.Reviewers.Include(r => r.HakemIlgiAlanlari).ToList();
            return Ok(_mapper.Map<List<ReviewerDto>>(values));
        }
        [HttpPost]
        public IActionResult CreateReviewers(CreateReviewerDto reviewer)
        {
            var value = _mapper.Map<Reviewer>(reviewer);
            _context.Reviewers.Add(value);
            _context.SaveChanges();
            return Ok("Hakem başarılı bir şekilde eklendi.");
        }
        [HttpDelete]
        public IActionResult DeleteReviewer(int id)
        {
            var value = _context.Reviewers.Find(id);
            _context.Reviewers.Remove(value);
            _context.SaveChanges();
            return Ok("Hakem başarılı bir şekilde silindi.");
        }
        [HttpGet("GetReviewer")]
        public IActionResult GetReviewer(int id)
        {
            var value = _context.Reviewers.Find(id);
            return Ok(_mapper.Map<ReviewerDto>(value));
        }
        [HttpPut]
        public IActionResult UpdateReviewer(ReviewerDto reviewer)
        {
            var value = _mapper.Map<Reviewer>(reviewer);
            _context.Reviewers.Update(value);
            _context.SaveChanges();
            return Ok("Hakem başarılı bir şekilde güncellendi.");
        }
    }
}
