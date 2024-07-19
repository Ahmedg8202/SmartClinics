namespace SmartClinics.DTOs.Doctor
{
    public class BookedAppointmentDto
    {
        public DateTime AppointmentDate { get; set; }
        public string PatientNationalID { get; set; }
        public string PatientName { get; set; }
    }
}
