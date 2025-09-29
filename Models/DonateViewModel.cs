using System.ComponentModel.DataAnnotations;

namespace Will_Website.Models
{
    public class DonationViewModel
    {
        [Display(Name = "Your Name")]
        public string? DonorName { get; set; }

        [EmailAddress, Display(Name = "Email (for receipt)")]
        public string? Email { get; set; }

        [Display(Name = "Message (optional)")]
        public string? Message { get; set; }

        [Display(Name = "Amount (ZAR)")]
        [Range(10, 100000, ErrorMessage = "Minimum R10")]
        public decimal? Amount { get; set; }
    }
}
