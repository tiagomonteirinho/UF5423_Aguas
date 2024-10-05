using Microsoft.AspNetCore.Mvc;

namespace UF5423_Aguas.Controllers
{
    public class ErrorsController : Controller
    {
        public IActionResult Unauthorized401()
        {
            return View();
        }

        [Route("/NotFound404")]
        public IActionResult NotFound404(string entityName) // Not found error view for unknown pages or specific entities.
        {
            return View("NotFound404", entityName);
        }
    }
}
