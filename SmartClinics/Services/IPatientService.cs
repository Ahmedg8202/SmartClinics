using SmartClinics.Data;
using SmartClinics.DTOs;
using SmartClinics.DTOs.Patient;
using SmartClinics.Models;

namespace SmartClinics.Services
{
    public interface IPatientService
    {
        public Task<RegisterPatient> CreateAsync(RegisterPatientdto model);
        public Task<RegisterPatient> LogInAsync(LogIndto model);
        public Task<Patient> UpdateAsync(UpdatePatientdto model);
        public Task<bool> DeleteAsync(string PatientId);
        public IEnumerable<Doctor> GetAllDoctors();
        public Task<IEnumerable<Doctor>> SearchAsync(SearchDto model);
        public Task<List<Appointment>> GetAvailableAppointmentsAsync(string doctorId);
        public Task<Appointment> BookAppointmentAsync(int appointmentId, string patientId);
        Task<bool> SendInvestigatesAsync(InvestigateDto model);
        Task<bool> RateDoctorAsync(RateDoctorDto model);
        Task<InvestigateReplayDto> GetInvestigatesReplayAsync(int investigateId);
        Task<bool> RequestToJoinAsDoctor(RequestToJoinAsDoctor model);

    }
}
