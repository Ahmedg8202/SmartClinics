using System.ComponentModel.DataAnnotations;

namespace SmartClinics.Models
{
    public class MedicalInvestigate
    {
        public int Id { get; set; }

        [Required]
        public string DoctorNationalID { get; set; }

        public Doctor? Doctor { get; set; }

        [Required]
        public string PatientNationalID { get; set; }

        public Patient? Patient { get; set; }

        [Required]
        public List<string> MedicalInves { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime SendTime { get; set; }
        public string? Comment { get; set; }
        public string Replay { get; set; }
        

    }
}
