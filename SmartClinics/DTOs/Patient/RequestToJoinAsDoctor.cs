using System.ComponentModel.DataAnnotations;

namespace SmartClinics.DTOs.Patient
{
    public class RequestToJoinAsDoctor
    {
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(14, MinimumLength = 14)]
        public string NationalID { get; set; }

        [RegularExpression(@"^(010|012|015|011)\d{8}$")]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Range(25, 100, ErrorMessage = "Age must be between 25 and 100")]
        public int Age { get; set; }

        [RegularExpression("^(Male|Female)$")]
        public string Gender { get; set; }

        public string Spicialty { get; set; }

        public string? Address { get; set; }
    }
}
