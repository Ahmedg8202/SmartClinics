using System.ComponentModel.DataAnnotations;

namespace SmartClinics.DTOs.Patient
{
    public class RegisterPatient
    {
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(14, MinimumLength = 14)]
        public string NationalID { get; set; }

        [RegularExpression(@"^(\010|\015|\011|\012)\d{8}$")]
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
        public string? Image { get; set; }
        public string Spicialty { get; internal set; }
        public List<string>? ClinicPhotos { get; set; }
        public string? SpecializeIn { get; set; }
        public string? AppointmentPrice { get; set; }
        public string? Description { get; set; }
        public string? AboutDoctor { get; set; }
        public List<string>? Roles { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public DateTime? ExpiresOn { get; set; }
    }
}
