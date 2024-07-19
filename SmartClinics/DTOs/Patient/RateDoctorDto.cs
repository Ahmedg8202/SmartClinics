using System.ComponentModel.DataAnnotations;

namespace SmartClinics.DTOs.Patient
{
    public class RateDoctorDto
    {
        [Required]
        public string DoctorNationalID { get; set; }

        [Required]
        public string PatientNationalID { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }

        [Required]
        public string Rating { get; set; }

        public string? Comments { get; set; }
    }
}
