using System.ComponentModel.DataAnnotations;

namespace SmartClinics.Models
{
    public class MedicalHistory
    {
        public int Id { get; set; }

        [Required]
        public string NationalID { get; set; }

        public Patient? Patient { get; set; }

        public string Diseases { get; set; }

        public string Surgeries { get; set; }
    }
}
