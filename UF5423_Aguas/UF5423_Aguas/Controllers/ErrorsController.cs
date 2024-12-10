using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UF5423_Aguas.Models;

namespace UF5423_Aguas.Controllers
{
    public class ErrorsController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string title, string message) // Error view for deletion, update or unhandled exceptions.
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Title = title,
                Message = message
            });
        }

        public IActionResult Unauthorized401() // Error view for unauthorized or forbidden access.
        {
            return View();
        }

        [Route("/NotFound404")]
        public IActionResult NotFound404(string entityName) // Error view for entities or unknown URLs.
        {
            return View("NotFound404", entityName);
        }
    }
}
