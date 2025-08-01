namespace WebApplication1.Models
{
    public class MultiStepFormModel
    {
        // Step 1 fields
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Step 2 fields
        public string Email { get; set; }
        public string Phone { get; set; }

        // Step 3 fields
        public string Address { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
    }
}
