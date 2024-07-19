using System.ComponentModel.DataAnnotations;

namespace SmartClinics.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public string DoctorNationalID { get; set; }

        public Doctor? Doctor { get; set; }

        public string? PatientNationalID { get; set; }

        public Patient? Patient { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }
        public bool IsAvaliable { get; set; }   
    }
}
