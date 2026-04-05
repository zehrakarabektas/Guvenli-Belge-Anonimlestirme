using AutoMapper;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Context;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.LogDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;

        public LogsController(ApiContext context,IMapper mapper)
        {
            _context=context;
            _mapper=mapper;
        }
        [HttpGet]
        public IActionResult LogList()
        {
            var values = _context.Logs.ToList();
            return Ok(_mapper.Map<LogDto>(values));
        }
        [HttpPost]
        public IActionResult CreateLog(CreateLogDto log)
        {
            var value = _mapper.Map<Log>(log);
            _context.Logs.Add(value);
            _context.SaveChanges();
            return Ok("Log ekleme işlemi başarılı.");
        }
        [HttpDelete]
        public IActionResult DeleteLog(int id)
        {
            var value = _context.Logs.Find(id);
            _context.Logs.Remove(value);
            _context.SaveChanges();
            return Ok("Log kaydı başarılı bir şekilde silindi.");
        }
        [HttpGet("GetLog")]
        public IActionResult GetLog(int id)
        {
            var value = _context.Logs.Find(id);
            return Ok(_mapper.Map<LogDto>(value));
        }
        [HttpGet("GetLogsByMakaleId")]
        public IActionResult GetLogsByMakaleId(int makaleId)
        {
            var logs = _context.Logs
                .Where(l => l.MakaleId == makaleId)
                .OrderBy(l => l.islemZamani)
                .ToList();

            var logDtos = _mapper.Map<List<LogDto>>(logs);
            return Ok(logDtos);
        }

        [HttpPut]
        public IActionResult UpdateLog(LogDto log)
        {
            var value = _mapper.Map<Log>(log);
            _context.Logs.Update(value);
            _context.SaveChanges();
            return Ok("Log başarılı bir şekilde güncellendi.");
        }

    }
}
