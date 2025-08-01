using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalBalance = 15432000.50m,
                TotalCustomers = 12500,
                ActiveLoans = 3500,
                PendingTransactions = 87
            };
            return View(model);
        }
    }
}
