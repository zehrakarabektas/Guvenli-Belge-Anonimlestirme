using AutoMapper;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Context;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.ArticleFieldsDto;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleFieldsController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;

        public ArticleFieldsController(ApiContext context, IMapper mapper)
        {
            _context=context;
            _mapper=mapper;
        }

        [HttpGet]
        public IActionResult ArticleFieldList()
        {
            var values = _context.ArticleFields.ToList();
            return Ok(_mapper.Map<List<ArticleFieldsDto>>(values));
        }
        [HttpPost]
        public IActionResult CreateArticleField(CreateArticleFieldsDto articleField)
        {
            var value = _mapper.Map<ArticleField>(articleField);
            _context.ArticleFields.Add(value);
            _context.SaveChanges();
            return Ok("ArticleField ekleme işlemi başarılı.");
        }
        [HttpDelete]
        public IActionResult DeleteArticleField(int id)
        {
            var value = _context.ArticleFields.Find(id);
            _context.ArticleFields.Remove(value);
            _context.SaveChanges();
            return Ok("ArticleField başarılı bir şekilde silindi.");
        }
        [HttpGet("GetField")]
        public IActionResult GetArticleField(int id)
        {
            var value = _context.ArticleFields.Find(id);
            return Ok(_mapper.Map<ArticleFieldsDto>(value));
        }
        [HttpPut]
        public IActionResult UpdateArticleField(ArticleFieldsDto articleField)
        {
            var value = _mapper.Map<ArticleField>(articleField);
            _context.ArticleFields.Update(value);
            _context.SaveChanges();
            return Ok("ArticleField başarılı bir şekilde güncellendi.");
        }
        [HttpGet("GetFieldsByArticleId")]
        public IActionResult GetFieldsByArticleId(int articleId)
        {
            var fields = _context.ArticleFields.Where(f => f.MakaleId == articleId).ToList();

            if (fields == null || !fields.Any())
            {
                return NotFound("Bu makaleye ait alan bulunamadı.");
            }

            return Ok(_mapper.Map<List<ArticleFieldsDto>>(fields));
        }
        [HttpDelete("DeleteFieldsByArticleId")]
        public IActionResult DeleteFieldsByArticleId(int articleId)
        {
            var fields = _context.ArticleFields.Where(f => f.MakaleId == articleId).ToList();

            if (fields == null || !fields.Any())
            {
                return Ok("Bu makaleye ait silinecek alan bulunamadı.");
            }

            _context.ArticleFields.RemoveRange(fields);
            _context.SaveChanges();

            return Ok("Alanlar başarıyla silindi.");
        }
        [HttpGet("GetFieldsBilgiByArticleId")]
        public IActionResult GetFieldsBilgiByArticleId(int articleId)
        {
            var fields = _context.ArticleFields
        .Include(f => f.AltKonular) 
        .Where(f => f.MakaleId == articleId)
        .ToList();

            if (!fields.Any())
            {
                return NotFound("Bu makaleye ait alan bulunamadı.");
            }

            return Ok(_mapper.Map<List<GetArticleTopicDto>>(fields));
        }


    }
}
