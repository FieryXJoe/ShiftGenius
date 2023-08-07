using Microsoft.AspNetCore.Mvc;

namespace ShiftGenius.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
