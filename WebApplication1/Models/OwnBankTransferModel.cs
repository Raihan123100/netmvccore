namespace WebApplication1.Models
{
    public class OwnBankTransferModel
    {
        // BOAC (BO Account) from BOSHRHLD table
        public string BOAC { get; set; }

        // Shareholder name from BOSHRHLD table
        public string NAME { get; set; }

        // Share balance from BOSHRHLD table
        public string SHBAL { get; set; }

        // Calculated NETPAY (CDAMT + FSAMT - DDAMT) from SHR_SALE_BO
        public string NETPAY { get; set; }

        // Bank name for transfer from BOSHRHLD
        public string PROPOSED_BNKNAME { get; set; }

        // Branch name for transfer from BOSHRHLD
        public string PROPOSED_BRANCH { get; set; }

        // Account number for transfer from BOSHRHLD
        public string PROPOSED_ACNO { get; set; }

        // Routing number for transfer from BOSHRHLD
        public string PROPOSED_ROUTINGNO { get; set; }

        // Constructor to initialize default values

    }
}
