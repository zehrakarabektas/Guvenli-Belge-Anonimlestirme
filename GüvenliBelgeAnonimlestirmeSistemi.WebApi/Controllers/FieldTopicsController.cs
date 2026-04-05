using AutoMapper;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Context;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.FieldTopicDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldTopicsController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;

        public FieldTopicsController(ApiContext context, IMapper mapper)
        {
            _context=context;
            _mapper=mapper;
        }

        [HttpGet]
        public IActionResult GetAllFieldTopics()
        {
            var values = _context.FieldTopics.ToList();
            return Ok(_mapper.Map<List<FieldTopicDto>>(values));
        }

        [HttpGet("GetFieldTopic")]
        public IActionResult GetFieldTopic(int id)
        {
            var value = _context.FieldTopics.Find(id);
            if (value == null)
            {
                return NotFound("Alan konusu bulunamadı.");
            }
            return Ok(_mapper.Map<FieldTopicDto>(value));
        }

        [HttpPost]
        public IActionResult CreateFieldTopic(CreateFieldTopicDto fieldTopic)
        {
            var value = _mapper.Map<FieldTopic>(fieldTopic);
            _context.FieldTopics.Add(value);
            _context.SaveChanges();
            return Ok("Alan konusu ekleme işlemi başarılı.");
        }

        [HttpPut]
        public IActionResult UpdateFieldTopic(FieldTopicDto fieldTopic)
        {
            var value = _mapper.Map<FieldTopic>(fieldTopic);
            _context.FieldTopics.Update(value);
            _context.SaveChanges();
            return Ok("Field başarılı bir şekilde güncellendi.");
        }

        [HttpDelete]
        public IActionResult DeleteFieldTopic(int id)
        {
            var value = _context.FieldTopics.Find(id);
            if (value == null)
            {
                return NotFound("Silmek istenilen alan konusu bulunamadı.");
            }
            _context.FieldTopics.Remove(value);
            _context.SaveChanges();
            return Ok("Alan konusu başarılı bir şekilde silindi.");
        }
        [HttpGet("GetByKonuAdiEn")]
        public IActionResult GetByKonuAdiEn(string name)
        {
            var topic = _context.FieldTopics.FirstOrDefault(x => x.KonuAdiEn == name);
            if (topic == null) return NotFound();

            return Ok(_mapper.Map<FieldTopicDto>(topic));
        }

    }
}
