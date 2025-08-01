using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication1.Models;
using WebApplication1.Utilities;

namespace WebApplication1.Controllers
{
    public class IncentiveController : Controller
    {
        private readonly DatabaseHelper _db;

        public IncentiveController(DatabaseHelper db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetIncentives()
        {
            var dt = _db.ExecuteQuery("SELECT EMPLOYEE_ID, BRANCH_CODE, TEAM_KPI FROM INCENTIVE_2023");

            var list = dt.AsEnumerable().Select(row => new
            {
                Id = row.Field<decimal>("EMPLOYEE_ID"),
                Name = row.Field<string>("BRANCH_CODE"),
                Amount = row.Field<decimal>("TEAM_KPI")
            }).ToList();

            return Json(new { data = list });
        }






        [HttpPost]
        public IActionResult SaveSelected([FromBody] List<IncentiveModel> selectedRows)
        {
            List<int> skippedIds = new();
            List<int> insertedIds = new();

            foreach (var row in selectedRows)
            {
                // Check if ID already exists
                var exists = _db.Exists("SELECT 1 FROM INCENTIVE2025 WHERE ID = :id", new Dictionary<string, object>
        {
            { "id", row.Id }
        });

                if (exists)
                {
                    skippedIds.Add(row.Id);
                    continue; // skip insert
                }

                // Insert only if not exists
                var query = "INSERT INTO INCENTIVE2025 (ID, NAME, AMOUNT) VALUES (:id, :name, :amount)";
                _db.ExecuteNonQuery(query, new Dictionary<string, object>
        {
            { "id", row.Id },
            { "name", row.Name },
            { "amount", row.Amount }
        });

                insertedIds.Add(row.Id);
            }

            return Json(new
            {
                success = true,
                inserted = insertedIds,
                skipped = skippedIds,
                message = $"{insertedIds.Count} inserted, {skippedIds.Count} skipped (duplicate IDs)"
            });
        }







    }

}
