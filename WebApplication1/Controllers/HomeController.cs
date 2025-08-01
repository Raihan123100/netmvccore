using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using WebApplication1.Models;
using WebApplication1.Utilities;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DatabaseHelper _db;

    public HomeController(ILogger<HomeController> logger, DatabaseHelper db)
    {
        _logger = logger;
        _db = db;
    }

    //public IActionResult Index()
    //{
    //    //var data = _db.ExecuteQuery("SELECT * FROM INCENTIVE_2023");
    //    ////return View(data);
    //    //return View();


    //var dataTable = _db.ExecuteQuery("SELECT EMPLOYEE_ID, BRANCH_CODE FROM INCENTIVE_2023");
    //var incentives = new List<Incentive>();

    //    foreach (DataRow row in dataTable.Rows)
    //    {
    //        incentives.Add(new Incentive
    //        {
    //            Id = Convert.ToInt32(row["EMPLOYEE_ID"]),
    //            Name = row["BRANCH_CODE"].ToString(),
    //           // Amount = Convert.ToDecimal(row["Amount"])
    //        });
    //    }

    //    return View(incentives);

    //}




    public IActionResult Index(string accountNo = null)
    {
        ViewBag.AccountNo = accountNo;
        var dataTable = _db.ExecuteQuery("SELECT EMPLOYEE_ID, BRANCH_CODE FROM INCENTIVE_2023");
        var incentives = new List<Incentive>();

        foreach (DataRow row in dataTable.Rows)
        {
            incentives.Add(new Incentive
            {
                Id = Convert.ToInt32(row["EMPLOYEE_ID"]),
                Name = row["BRANCH_CODE"].ToString(),
                // Amount = Convert.ToDecimal(row["Amount"])
            });
        }


        if (!string.IsNullOrEmpty(accountNo))
        {
            ViewBag.Balance = _db.GetAccountBalance(accountNo);
            ViewBag.History = _db.GetTransactionHistory(accountNo);
        }

        return View(incentives);
    }

    [HttpPost]
    public IActionResult Credit(string accountNo, decimal amount)
    {
        try
        {
            _db.CreditAccount(accountNo, amount);
            TempData["Message"] = $"Credited {amount:C} to account {accountNo}.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Credit failed: " + ex.Message;
        }

        return RedirectToAction("Index", new { accountNo });
    }

    [HttpPost]
    public IActionResult Debit(string accountNo, decimal amount)
    {
        try
        {
            _db.DebitAccount(accountNo, amount);
            TempData["Message"] = $"Debited {amount:C} from account {accountNo}.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Debit failed: " + ex.Message;
        }

        return RedirectToAction("Index", new { accountNo });
    }





    [HttpPost]
    public IActionResult Add(int id, string name, decimal amount)
    {
        _db.CallAddIncentive(id, name, amount);
        return RedirectToAction("Index");
    }





    //[HttpPost]
    //public IActionResult Credit(string accountNo, decimal amount)
    //{
    //    _db.CreditAccount(accountNo, amount);
    //    TempData["Message"] = "Account credited successfully.";
    //    return RedirectToAction("Index");
    //}

    //[HttpPost]
    //public IActionResult Debit(string accountNo, decimal amount)
    //{
    //    _db.DebitAccount(accountNo, amount);
    //    TempData["Message"] = "Account debited successfully.";
    //    return RedirectToAction("Index");
    //}



    public List<OwnBankTransferModel> ViewReportOwnBankTransfer(string year)
    {
        year = "2023";
        var eventList = new List<OwnBankTransferModel>();
        DbResponse response = new DbResponse { status = 0, message = "" };

        string sql = "PKG_REPORT_MGMNT.GET_OWN_BANK_TRANSFER_DATA";

        OracleParameter[] parameters = new OracleParameter[4];
        int index = 0;

        // Input parameter
        parameters[index++] = new OracleParameter("P_YEAR", OracleDbType.Int32, Convert.ToInt32(year), ParameterDirection.Input);

        // Output cursor for results
        parameters[index++] = new OracleParameter("P_RESULT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);

        // Output parameters for status/message
        parameters[index++] = new OracleParameter("O_STATUS", OracleDbType.Int32, 10, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, 0);
        parameters[index++] = new OracleParameter("O_MESSAGE", OracleDbType.Varchar2, 2000, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, "");

        try
        {
            using (var reader = _db.ExecuteReader(sql, CommandType.StoredProcedure, parameters))
            {
                while (reader.Read())
                {
                    var eventModel = new OwnBankTransferModel
                    {
                        BOAC = reader["BOAC"] != DBNull.Value ? reader["BOAC"].ToString() : null,
                        NAME = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : null,
                       // SHBAL = reader["SHBAL"] != DBNull.Value ? Convert.ToDecimal(reader["SHBAL"]) : 0m,
                       // NETPAY = reader["NETPAY"] != DBNull.Value ? Convert.ToDecimal(reader["NETPAY"]) : 0m,
                        PROPOSED_BNKNAME = reader["PROPOSED_BNKNAME"] != DBNull.Value ? reader["PROPOSED_BNKNAME"].ToString() : null,
                        PROPOSED_BRANCH = reader["PROPOSED_BRANCH"] != DBNull.Value ? reader["PROPOSED_BRANCH"].ToString() : "",
                        PROPOSED_ACNO = reader["PROPOSED_ACNO"] != DBNull.Value ? reader["PROPOSED_ACNO"].ToString() : "",
                        PROPOSED_ROUTINGNO = reader["PROPOSED_ROUTINGNO"] != DBNull.Value ? reader["PROPOSED_ROUTINGNO"].ToString() : ""
                    };
                    eventList.Add(eventModel);
                }
            }

            // Check status from output parameters
           // response.status = Convert.ToInt32(parameters[2].Value.ToString();
            response.message = parameters[3].Value?.ToString() ?? string.Empty;

            if (response.status != 0)
            {
                // Log error or handle as needed
                _logger.LogError($"Database error: {response.message}");
            }
        }
        catch (OracleException ex)
        {
            _logger.LogError(ex, "Oracle error in ViewReportOwnBankTransfer");
            throw new ApplicationException("Database error occurred while retrieving transfer data.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ViewReportOwnBankTransfer");
            throw;
        }

        return eventList;
    }




















    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


