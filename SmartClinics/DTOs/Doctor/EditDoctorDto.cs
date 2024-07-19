namespace SmartClinics.DTOs.Doctor
{
    public class EditDoctorDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Spicialty { get; set; }
        public string Address { get; set; }
        public IFormFile Image { get; set; }
        public string? AboutDoctor { get; set; }
        public string? Description { get; set; }
        public string? AppointmentPrice { get; set; }
        public string? SpecializeIn { get; set; }
        public List<IFormFile>? ClinicPhotos { get; set; }
    }

}
