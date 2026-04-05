using AutoMapper;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Context;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.ArticleDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;

        public ArticlesController(ApiContext context,IMapper mapper)
        {
            _context=context;
            _mapper=mapper;
        }
        [HttpGet]
        public IActionResult ArticleList()
        {
            var values = _context.Articles.ToList();
            return Ok(_mapper.Map<List<ArticleDto>>(values));
        }
        //[HttpPost]
        //public IActionResult CreateArticle(Article article)
        //{
        //    _context.Articles.Add(article);
        //    _context.SaveChanges();
        //    return Ok("Makale başarılı bir şekilde eklendi.");
        //}
        [HttpPost]
        public IActionResult CreateArticleDto(CreateArticleDto article)
        {
            var value = _mapper.Map<Article>(article);
            _context.Articles.Add(value);
            _context.SaveChanges();
            var articleDto = _mapper.Map<ArticleDto>(value);

            return Ok(articleDto);
        }
        [HttpDelete]
        public IActionResult DeleteArticle(int id)
        {
            var value = _context.Articles.Find(id);
            _context.Articles.Remove(value);
            _context.SaveChanges();
            return Ok("Makale başarılı bir şekilde silindi.");
        }
        [HttpGet("GetArticleById")]
        public IActionResult GetArticle(int id)
        {
            var value = _context.Articles.Find(id);
            return Ok(_mapper.Map<ArticleDto>(value));
        }
        [HttpGet("GetArticleByReviewerId")]
        public IActionResult GetArticleByReviewerId(int id)
        {
            var values = _context.Articles.Where(x=>x.ReviewerId==id).ToList();
            return Ok(_mapper.Map<List<ArticleDto>>(values));
        }
        [HttpPut]
        public IActionResult UpdateArticle(ArticleDto article)
        {
            var value = _mapper.Map<Article>(article);
            _context.Articles.Update(value);
            _context.SaveChanges();
            return Ok("Makale başarılı bir şekilde güncellendi.");
        }
        [HttpGet("GetArticleByTrackingNumberEmail")]
        public ActionResult<bool> GetArticleByTrackingNumberEmail([FromQuery] string makaleTakipNo, [FromQuery] string email)
        {
            var makale = _context.Articles.FirstOrDefault(m => m.TakipNo == makaleTakipNo && m.YazarEPosta == email);
            return makale != null;
        }

        [HttpGet("GetArticleByTrackingNumber")]
        public IActionResult GetArticleByTrackingNumber(string takipno)
        {
            var value = _context.Articles.FirstOrDefault(x => x.TakipNo==takipno);
            if (value == null)
            {
                return NotFound("Makale bulunamadı.");
            }
            return Ok(_mapper.Map<ArticleDto>(value));
        }
        [HttpGet("GetArticleByStatus")]
        public IActionResult GetArticleByStatus(ArticleStatus durum)
        {
            var value = _context.Articles.FirstOrDefault(x => x.MakaleDurumu==durum);
            if (value == null)
            {
                return NotFound("Belirlenen durumda olan makale bulunamadı.");
            }
            return Ok(_mapper.Map<ArticleDto>(value));
        }
      

    }
}
