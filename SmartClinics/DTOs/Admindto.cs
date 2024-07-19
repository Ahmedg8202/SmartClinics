using System.ComponentModel.DataAnnotations;

namespace SmartClinics.DTOs
{
    public class Admindto
    {
        [Required]
        public string UserName { get; set; }

        [Required, StringLength(256)]
        public string Password { get; set; }

    }
}
