using Microsoft.AspNetCore.Mvc;

namespace ShiftGenius.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
