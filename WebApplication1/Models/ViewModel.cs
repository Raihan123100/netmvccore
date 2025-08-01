using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Step1ViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }

    public class Step2ViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, Phone]
        public string Phone { get; set; }
    }

    public class Step3ViewModel
    {
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string County { get; set; }
        [Required]
        public string Postcode { get; set; }
    }
}
