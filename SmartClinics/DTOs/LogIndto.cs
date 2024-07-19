using System.ComponentModel.DataAnnotations;

namespace SmartClinics.DTOs
{
    public class LogIndto
    {
        //comment
       // [StringLength(14, MinimumLength = 14)]
        public string NationalID { get; set; }

        [Required, StringLength(256)]
        public string Password { get; set; }
    }
}
