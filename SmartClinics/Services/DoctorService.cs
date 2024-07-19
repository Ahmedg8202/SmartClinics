using SmartClinics.Data;
using SmartClinics.DTOs;
using SmartClinics.DTOs.Doctor;
using SmartClinics.DTOs.Patient;
using SmartClinics.Helers;
using SmartClinics.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace SmartClinics.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly CreateJWTToken _createJWTToken;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SaveToFirebase _saveToFirebase;
        public DoctorService(ApplicationDbContext context, CreateJWTToken createJWTToken, 
            UserManager<ApplicationUser> userManager, SaveToFirebase saveToFirebase)
        {
            _context = context;
            _createJWTToken = createJWTToken;
            _userManager = userManager;
            _saveToFirebase = saveToFirebase;
        }
        public async Task<RegisterDoc> LogInAsync(LogIndto model)
        {
            var exustUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NationalID == model.NationalID);

            if (exustUser == null || !await _userManager.CheckPasswordAsync(exustUser, model.Password))
                return new RegisterDoc { Message = "National ID or Password is incorrect" };

            var doctor = await _context.Doctors.FirstOrDefaultAsync(p => p.NationalID == model.NationalID);
            if (doctor == null)
                return new RegisterDoc { Message = "Doctor details not found in the database" };

            var user = new ApplicationUser
            {
                NationalID = model.NationalID,
                UserName = doctor.Name,
                Email = doctor.Email,
                PhoneNumber = doctor.PhoneNumber
            };
            var jwtSecurityToken = await _createJWTToken.CreateJwtToken(exustUser);

            var registerModel = new RegisterDoc();
            registerModel.IsAuthenticated = true;
            registerModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            registerModel.Email = doctor.Email;
            registerModel.Name = doctor.Name;
            registerModel.NationalID = doctor.NationalID;
            registerModel.Image = doctor.Image;
            registerModel.Gender = doctor.Gender;
            registerModel.Spicialty = doctor.Spicialty;
            registerModel.PhoneNumber = doctor.PhoneNumber;
            registerModel.Age = doctor.Age;
            registerModel.ExpiresOn = jwtSecurityToken.ValidTo;
            registerModel.SpecializeIn = doctor.specializeIn;
            registerModel.AppointmentPrice = doctor.AppointmentPrice;
            registerModel.Description = doctor.Description;
            registerModel.AboutDoctor = doctor.AboutDoctor;
            registerModel.ClinicPhotos = doctor.ClinicPhotos;
            registerModel.Roles = new List<string> { "Docotr" };

            return registerModel;
        }
        public async Task<bool> EditProfileAsync(string doctorId, EditDoctorDto model)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.NationalID == doctorId);

            if (doctor == null)
                return false;

            string ImgUrl = await _saveToFirebase.ImageToFirebase(model.Image);

            doctor.Image = ImgUrl;

            List<string> ClinicPhotos = new List<string>();

            if (model.ClinicPhotos.Count > 0)
            {
                foreach (var file in model.ClinicPhotos)
                {
                    string downloadUrl = await _saveToFirebase.ImageToFirebase(file);

                    ClinicPhotos.Add(downloadUrl);

                }
                doctor.ClinicPhotos = ClinicPhotos;
            }

            doctor.Name = model.Name;
            doctor.PhoneNumber = model.PhoneNumber;
            doctor.Email = model.Email;
            doctor.Age = model.Age;
            doctor.Gender = model.Gender;
            doctor.Spicialty = model.Spicialty;
            doctor.Address = model.Address;
            doctor.AboutDoctor = model.AboutDoctor;
            doctor.Description = model.Description;
            doctor.AppointmentPrice = model.AppointmentPrice;
            doctor.specializeIn = model.SpecializeIn;

            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<MedicalInvestigate>> GetInvestigatesAsync(string doctorNationalID)
        {
             var Inves = await _context.MedicalInvestigates
                .Where(i => i.DoctorNationalID == doctorNationalID)
                .ToListAsync();
            return Inves;
        }
        public async Task<bool> SendInvestigateReplyAsync(InvestigateReplayDto model)
        {
            var investigate = await _context.MedicalInvestigates.FindAsync(model.InvestigateId);

            if (investigate == null)
                return false;

            investigate.Replay = model.Replay;

            _context.MedicalInvestigates.Update(investigate);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<string> AddAppointments(AddAppointmentsDto dto)
        {
            if (await _context.Doctors.FirstOrDefaultAsync(d => d.NationalID == dto.DoctorId) is null)
                return "Doctor Not Found";

            foreach (var appointmentDto in dto.Appointments)
            {
                var appointment = new Appointment
                {
                    DoctorNationalID = dto.DoctorId,
                    AppointmentDate = appointmentDto.Date,
                    IsAvaliable = true,
                    PatientNationalID = null
                };
                    _context.Appointments.Add(appointment);
            }

            await _context.SaveChangesAsync();

            return "Appointments added successfully.";
            
        }
        public async Task<List<Appointment>> ViewAppointments(string doctorId)
        {
            if (await _context.Doctors.FirstOrDefaultAsync(d => d.NationalID == doctorId) is null)
                return new List<Appointment> { };

            var doctorAppointments = await _context.Appointments
                    .Include(a => a.Doctor)
                    .Include(a => a.Patient)
                    .Where(a => a.DoctorNationalID == doctorId && a.PatientNationalID != null && a.IsAvaliable == false)
                    .ToListAsync();

            return doctorAppointments;
        }
        public async Task<List<BookedAppointmentDto>> GetBookedAppointmentsAsync(string doctorNationalId)
        {
            var bookedAppointments = await _context.Appointments
                .Where(a => a.DoctorNationalID == doctorNationalId && a.IsAvaliable)
                .Select(a => new BookedAppointmentDto
                {
                    AppointmentDate = a.AppointmentDate,
                    PatientNationalID = a.PatientNationalID,
                    PatientName = a.Patient.Name

                })
                .ToListAsync();

            return bookedAppointments;
        }
        public async Task<List<RateDoctorDto>> ALlRates(string DoctorNationalID)
        {
            var doctor = await _context.DoctorRatings.FirstOrDefaultAsync(d => d.DoctorNationalID == DoctorNationalID);

            if (doctor == null)
                return new List<RateDoctorDto>();
            var rates = _context.DoctorRatings.Where(r=>r.DoctorNationalID==DoctorNationalID)
                .OrderByDescending(r=>r.Rating).ToList();

            var result = new List<RateDoctorDto> ();
            foreach(var rate in rates)
            {
                var rateDoctor = new RateDoctorDto
                {
                    DoctorNationalID = rate.DoctorNationalID,
                    DoctorName = _context.Doctors.FirstOrDefault(d => d.NationalID == rate.DoctorNationalID).Name,
                    PatientNationalID = rate.PatientNationalID,
                    PatientName = _context.Patients.FirstOrDefault(p => p.NationalID == rate.PatientNationalID).Name,
                    Rating = rate.Rating,
                    Comments = rate.Comments
                };
                result.Add(rateDoctor);
            }

            return result;
        }
        public IEnumerable<Doctor> GetAllAsync()
        {
            var result = _context.Doctors.ToList();

            return result;
        }
    }
}
