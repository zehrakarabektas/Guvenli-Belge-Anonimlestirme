using AutoMapper;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Context;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.MessageDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;

        public MessagesController(ApiContext context, IMapper mapper)
        {
            _context=context;
            _mapper=mapper;
        }

        [HttpGet]
        public IActionResult MessageList()
        {
            var values = _context.Messages.ToList();
            return Ok(_mapper.Map<List<MessageDto>>(values));
        }
        [HttpPost]
        public IActionResult CreateMessage(CreateMessageDto message)
        {
            var value=_mapper.Map<Message>(message);
            _context.Messages.Add(value);
            _context.SaveChanges();
            return Ok("Mesaj başarılı bir şekilde eklendi.");
        }
        [HttpDelete]
        public IActionResult DeleteMessage(int id)
        {
            var value = _context.Messages.Find(id);
            _context.Messages.Remove(value);
            _context.SaveChanges();
            return Ok("Mesaj başarılı bir şekilde silindi.");
        }
       
        [HttpGet("GetMessagesByArticle")]
        public IActionResult GetMessagesByArticle(int articleId)
        {
            var values= _context.Messages.Where(m => m.MakaleId == articleId).OrderBy(m => m.SendTime).ToList();
            return Ok(_mapper.Map<List<MessageDto>>(values));
        }
        [HttpPut]
        public IActionResult UpdateMessage(MessageDto message)
        {
            var value = _mapper.Map<Message>(message);
            _context.Messages.Update(value);
            _context.SaveChanges();
            return Ok("Mesaj başarılı bir şekilde güncellendi.");
        }
    }
}
