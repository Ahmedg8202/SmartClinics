using System.ComponentModel.DataAnnotations;

namespace SmartClinics.DTOs
{
    public class RegisterAdmin
    {
        [Required]
        public string UserName { get; set; }

        [Required, StringLength(256)]
        public string Password { get; set; }
        public string? Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public List<string>? Roles { get; set; }
        public string? Token { get; set; }

        public DateTime? ExpiresOn { get; set; }
    }
}
