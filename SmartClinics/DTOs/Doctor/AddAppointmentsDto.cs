namespace SmartClinics.DTOs.Doctor
{
    public class AddAppointmentsDto
    {
        public string DoctorId { get; set; }
        public List<AppointmentDto> Appointments { get; set; }
    }
}
