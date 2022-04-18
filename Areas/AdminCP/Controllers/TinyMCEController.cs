using Microsoft.AspNetCore.Mvc;

namespace CMS.Areas.AdminCP.Controllers
{
    [Area("AdminCP")]
    [Route("/AdminCp/tiny-mce")]
    public class TinyMCEController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("browse")]
        public IActionResult Browse()
        {
            return View();
        }
    }
}