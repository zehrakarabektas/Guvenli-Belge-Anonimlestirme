using Microsoft.AspNetCore.Mvc;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebUI.Controllers
{
    public class MessageController : Controller
    {
        public IActionResult EditorMessagePage()
        {
            return View();
        }
    }
}
