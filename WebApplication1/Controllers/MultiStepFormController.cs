using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication1.Models;
using WebApplication1.Utilities;

namespace WebApplication1.Controllers
{
    public class MultiStepFormController : Controller
    {
        private const string SessionKey = "MultiStepForm";

        [HttpGet]
        public IActionResult Step1()
        {
            var model = new Step1ViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Step1(Step1ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sessionModel = HttpContext.Session.Get<MultiStepFormModel>(SessionKey) ?? new MultiStepFormModel();
            sessionModel.FirstName = model.FirstName;
            sessionModel.LastName = model.LastName;
            HttpContext.Session.Set(SessionKey, sessionModel);

            return RedirectToAction("Step2");
        }

        [HttpGet]
        public IActionResult Step2()
        {
            var model = new Step2ViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Step2(Step2ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sessionModel = HttpContext.Session.Get<MultiStepFormModel>(SessionKey);
            sessionModel.Email = model.Email;
            sessionModel.Phone = model.Phone;
            HttpContext.Session.Set(SessionKey, sessionModel);

            return RedirectToAction("Step3");
        }

        [HttpGet]
        public IActionResult Step3()
        {
            var model = new Step3ViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Step3(Step3ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sessionModel = HttpContext.Session.Get<MultiStepFormModel>(SessionKey);
            sessionModel.Address = model.Address;
            sessionModel.City = model.City;
            sessionModel.County = model.County;
            sessionModel.Postcode = model.Postcode;

            // Process the form data (e.g., save to database)
            HttpContext.Session.Remove(SessionKey);

            return RedirectToAction("Confirmation");
        }

        public IActionResult Confirmation()
        {
            return View("~/Views/Shared/Confirmation.cshtml");
        }
    }

}
