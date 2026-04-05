using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Context;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditorsController : ControllerBase
    {
        private readonly ApiContext _context;

        public EditorsController(ApiContext context)
        {
            _context=context;
        }

        [HttpGet]
        public IActionResult EditorList()
        {
            var values = _context.Editors.ToList();
            return Ok(values);
        }
        [HttpPost]
        public IActionResult CreateEditor(Editor editor)
        {
            _context.Editors.Add(editor);
            _context.SaveChanges();
            return Ok("Editor başarılı bir şekilde eklendi.");
        }
        [HttpDelete]
        public IActionResult DeleteEditor(int id)
        {
            var value = _context.Editors.Find(id);
            _context.Editors.Remove(value);
            _context.SaveChanges();
            return Ok("Editör başarılı bir şekilde silindi.");
        }
        [HttpGet("GetEditor")]
        public IActionResult GetEditor(int id)
        {
            var value = _context.Editors.Find(id);
            return Ok(value);
        }
        [HttpPut]
        public IActionResult UpdateEditor(Editor editor)
        {
            _context.Editors.Update(editor);
            _context.SaveChanges();
            return Ok("Editör başarılı bir şekilde güncellendi.");
        }
    }
}
