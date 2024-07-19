using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SmartClinics.Models
{
    public class Doctor : Human
    {
        public string? Address { get; set; }
        public string? Cirtificates { get; set; }
        public string? Rate { get;set; }
        public string Spicialty { get; set; }
        public string? AboutDoctor { get; set; }
        public string? Description { get; set; }
        public string? AppointmentPrice { get; set; }
        public string? specializeIn { get; set; }
        public List<string>? ClinicPhotos { get; set; }
    }
}
