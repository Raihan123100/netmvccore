using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class viewdatatempdata : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "Hello from ViewData!";
            ViewBag.Greeting = "Hello from ViewBag!";
            TempData["Message"] = "Your data has been saved!";

            return View();
           
        }




        public IActionResult Submit()
        {
            TempData["Message"] = "Your data has been saved!";
            return RedirectToAction("Index");
        }

    }
}
