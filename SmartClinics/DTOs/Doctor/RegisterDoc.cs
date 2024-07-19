using SmartClinics.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;

namespace SmartClinics.DTOs.Doctor
{
    public class RegisterDoc
    {
        public string Name { get; set; }
        public string NationalID { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Image { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string? Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public List<string>? Roles { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public string Spicialty { get; set; }
        public List<string>? ClinicPhotos { get; set; }
        public string? SpecializeIn { get; set; }
        public string? AppointmentPrice { get; set; }
        public string? Description { get; set; }
        public string? AboutDoctor { get; set; }

    }
}
