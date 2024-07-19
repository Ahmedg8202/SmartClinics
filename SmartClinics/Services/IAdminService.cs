using SmartClinics.DTOs;
using SmartClinics.DTOs.Doctor;
using SmartClinics.DTOs.Patient;
using SmartClinics.Models;

namespace SmartClinics.Services
{
    public interface IAdminService
    {
        public Task<RegisterAdmin> CreateAsync(Admindto model);
        public Task<RegisterPatient> LogInAsync(LogIndto model);
        public Task<List<DoctorRequestsToJoin>> PendingRequestsAsync();
        public Task<RegisterDoc> AddDoctorAsync(RegisterDocdto model);
        public Task<RegisterDoc> DeleteDoctorAsync(string doctorId);
    }
}
