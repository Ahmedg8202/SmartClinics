namespace SmartClinics.DTOs.Patient
{
    public class AvailableTimesDto
    {
        public DateTime Day { get; set; }
        public List<string> AvailableTimes { get; set; }
    }
}
