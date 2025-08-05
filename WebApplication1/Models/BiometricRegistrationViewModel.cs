namespace WebApplication1.Models
{
    public class BiometricRegistrationViewModel
    {
        public string BrCode { get; set; }
        public string ActCode { get; set; }
        public string AcNo { get; set; }
        public string AccountTitle { get; set; }
        public string CustomerId { get; set; }

        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string NID { get; set; }
        public string SmartNID { get; set; }
        public string DOB { get; set; }
        public string TIN { get; set; }
        public string Address { get; set; }
        public string District { get; set; }
        public string Upazila { get; set; }
        public string Country { get; set; }

        public List<string> SelectedAccountNumbers { get; set; }
        public string FingerprintStatus { get; set; }
        public string OTP { get; set; }
    }

}
