
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SmartClinics.Data
{
    public class ApplicationUser: IdentityUser
    {
        [StringLength(14, MinimumLength = 14)]
        public string NationalID { get; set; }
    }
}
