using System.ComponentModel.DataAnnotations;

namespace SmartClinics.Models
{
    public class Patient: Human
    {
        [RegularExpression("^(Single|Married|Divorced|Widowed)$")]
        public string? Status { get; set; }
    }
}
