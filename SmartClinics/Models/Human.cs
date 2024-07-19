using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.ComponentModel.DataAnnotations;

namespace SmartClinics.Models
{
    public class Human
    {
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(14, MinimumLength = 14)]
        public string NationalID { get; set; }

        [RegularExpression(@"^(010|012|015|011)\d{8}$")]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
        public int Age { get; set; }

        [RegularExpression("^(Male|Female)$")]
        public string Gender { get; set; }

        [RegularExpression("^(A|B|AB|O)[+-]$")]
        public string? BloodType { get; set; }
        public string? Image { get; set; }
    }
}
