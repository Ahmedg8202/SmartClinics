using System.ComponentModel.DataAnnotations;

namespace SmartClinics.Models
{
    
    public class DoctorRating
    {
        public int Id { get; set; }

        [Required]
        public string DoctorNationalID { get; set; }

        public Doctor? Doctor { get; set; }

        [Required]
        public string PatientNationalID { get; set; }

        public Patient? Patient { get; set; }

        [Required]
        public string Rating { get; set; }

        public string? Comments { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime RateTime { get; set; }
    }

}
