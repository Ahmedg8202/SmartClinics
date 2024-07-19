using System.ComponentModel.DataAnnotations;

namespace SmartClinics.DTOs.Patient
{
    public class RegisterPatientdto
    {
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(14, MinimumLength = 14)]
        public string NationalID { get; set; }

        [RegularExpression(@"^(010|012|015|011)\d{8}$")]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
        public int Age { get; set; }

        [RegularExpression("^(Male|Female)$")]
        public string Gender { get; set; }

        [RegularExpression("^(Single|Married|Divorced|Widowed)$")]
        public string? Status { get; set; }

        [Required, StringLength(256)]
        public string Password { get; set; }
        public IFormFile? Image { get; set; }
    }
}
