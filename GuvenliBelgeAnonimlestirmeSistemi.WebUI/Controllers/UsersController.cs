using Microsoft.AspNetCore.Mvc;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebUI.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult UserHome()
        {
            return View();
        }
        public IActionResult MessageEditor()
        {
            return View();
        }
      
    }
}
