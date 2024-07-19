using SmartClinics.Data;
using SmartClinics.DTOs;
using SmartClinics.DTOs.Doctor;
using SmartClinics.DTOs.Patient;
using SmartClinics.Models;

namespace SmartClinics.Services
{
    public interface IDoctorService
    {
        public Task<RegisterDoc> LogInAsync(LogIndto model);
        Task<bool> EditProfileAsync(string doctorId, EditDoctorDto model);
        Task<List<MedicalInvestigate>> GetInvestigatesAsync(string doctorNationalID);
        Task<bool> SendInvestigateReplyAsync(InvestigateReplayDto model);
        public Task<string> AddAppointments(AddAppointmentsDto dto);
        public Task<List<Appointment>> ViewAppointments(string doctorId);
        Task<List<BookedAppointmentDto>> GetBookedAppointmentsAsync(string doctorNationalId);
        public IEnumerable<Doctor> GetAllAsync();
        Task<List<RateDoctorDto>> ALlRates(string DoctorNationalID);
    }
}
