using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApplication1.Utilities;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly IConfiguration _config;

        public PatientController(IConfiguration config)
        {
            _config = config;
            _db = new DatabaseHelper(_config);
        }

        public IActionResult BookAppointment() => View();

        [HttpPost]
        public IActionResult BookAppointment(string doctorName, DateTime appointmentTime)
        {
            string username = User.Identity.Name;
            var sql = @"INSERT INTO appointments (patient_username, doctor_name, appointment_time, status, payment_status) 
                    VALUES (:username, :doctor, :time, 'Pending', 'Unpaid')";

            var param = new Dictionary<string, object> {
            { "username", username },
            { "doctor", doctorName },
            { "time", appointmentTime }
        };

            _db.ExecuteNonQuery(sql, param);
            return RedirectToAction("PaymentGateway");
        }

        public IActionResult PaymentGateway()
        {
            // Redirect to SSLCommerz or display QR/code options
            return View();
        }

        public IActionResult RedirectToSslCommerz()
        {
            var postData = new Dictionary<string, string> {
        { "store_id", "your_store_id" },
        { "store_passwd", "your_password" },
        { "total_amount", "500" },
        { "currency", "BDT" },
        { "tran_id", Guid.NewGuid().ToString() },
        { "success_url", "https://yourdomain.com/Payment/Success" },
        { "fail_url", "https://yourdomain.com/Payment/Fail" },
        { "cancel_url", "https://yourdomain.com/Payment/Cancel" }
        // Add other fields...
    };

            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(postData);
            var response = client.PostAsync("https://sandbox.sslcommerz.com/gwprocess/v4/api.php", content).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            var json = JsonDocument.Parse(result);

            var gatewayPageUrl = json.RootElement.GetProperty("GatewayPageURL").GetString();
            return Redirect(gatewayPageUrl);
        }
    }

}
