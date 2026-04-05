using AutoMapper;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Context;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.FieldDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldsController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;

        public FieldsController(ApiContext context, IMapper mapper)
        {
            _context=context;
            _mapper=mapper;
        }

        [HttpGet]
        public IActionResult FieldList()
        {
            var values = _context.Fields.ToList();
            return Ok(_mapper.Map<List<FieldDto>>(values));
        }
        [HttpPost]
        public IActionResult CreateField(CreateFieldDto field)
        {
            var value = _mapper.Map<Field>(field);
            _context.Fields.Add(value);
            _context.SaveChanges();
            return Ok("Field ekleme işlemi başarılı.");
        }
        [HttpDelete]
        public IActionResult DeleteField(int id)
        {
            var value = _context.Fields.Find(id);
            _context.Fields.Remove(value);
            _context.SaveChanges();
            return Ok("Field başarılı bir şekilde silindi.");
        }
        [HttpGet("GetField")]
        public IActionResult GetField(int id)
        {
            var value = _context.Fields.Find(id);
            return Ok(_mapper.Map<FieldDto>(value));
        }
        [HttpPut]
        public IActionResult UpdateField(FieldDto field)
        {
            var value = _mapper.Map<Field>(field);
            _context.Fields.Update(value);
            _context.SaveChanges();
            return Ok("Field başarılı bir şekilde güncellendi.");
        }
    }
}
