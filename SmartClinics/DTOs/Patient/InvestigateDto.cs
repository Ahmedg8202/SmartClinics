namespace SmartClinics.DTOs.Patient
{
    public class InvestigateDto
    {
        public string PatientNationalID { get; set; }
        public string DoctorNationalID { get; set; }
        public IFormFile[] InvestigateImages { get; set; }
        public string? Comment { get; set; }
    }
}
