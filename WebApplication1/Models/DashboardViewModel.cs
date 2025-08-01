namespace WebApplication1.Models
{
    public class DashboardViewModel
    {
        public decimal TotalBalance { get; set; }
        public int TotalCustomers { get; set; }
        public int ActiveLoans { get; set; }
        public int PendingTransactions { get; set; }
    }

}
