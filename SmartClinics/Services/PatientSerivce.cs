using Firebase.Auth;
using SmartClinics.Data;
using SmartClinics.DTOs;
using SmartClinics.DTOs.Doctor;
using SmartClinics.DTOs.Patient;
using SmartClinics.Helers;
using SmartClinics.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Numerics;

namespace SmartClinics.Services
{
    public class PatientSerivce: IPatientService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CreateJWTToken _createJWTToken;
        private readonly SaveToFirebase _saveToFirebase;
        private readonly IDoctorService _doctorService;

        public PatientSerivce(ApplicationDbContext context, UserManager<ApplicationUser> userManager, CreateJWTToken createJWTToken
            , IDoctorService doctorService, SaveToFirebase saveToFirebase)
        {
            _context = context;
            _userManager = userManager;
            _createJWTToken = createJWTToken;
            _doctorService = doctorService;
            _saveToFirebase = saveToFirebase;
        }

        public async Task<RegisterPatient> CreateAsync(RegisterPatientdto model)
                                                                                                                                                                                                                                                                                                                                                                         {
            if (await _userManager.Users.FirstOrDefaultAsync(u => u.NationalID == model.NationalID) != null)
                return new RegisterPatient { Message = "This National ID is already registered" };

            var user = new ApplicationUser
            {
                UserName = model.Name.Replace(" ",string.Empty),
                Email = model.Email,
                NationalID = model.NationalID,
                PhoneNumber = model.PhoneNumber
            };

            var ImgUrl = await _saveToFirebase.ImageToFirebase(model.Image);

            var patient = new Patient
            {
                NationalID = model.NationalID,
                Name = model.Name,
                Email = model.Email,
                Gender = model.Gender,
                Age = model.Age,
                PhoneNumber = model.PhoneNumber,
                Image = ImgUrl
            };

            await _context.Patients.AddAsync(patient);

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                return new RegisterPatient { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "Patient");

            await _context.SaveChangesAsync();

            var roleList = await _userManager.GetRolesAsync(user);
            var jwtSecurityToken = await _createJWTToken.CreateJwtToken(user);

            return new RegisterPatient
            {
                Name = model.Name,
                NationalID = model.NationalID,
                Email = model.Email,
                Image = ImgUrl,
                Gender = model.Gender,
                IsAuthenticated = true,
                Roles = roleList.ToList(),
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            };
        }
        public async Task<RegisterPatient> LogInAsync(LogIndto model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.NationalID == model.NationalID);
            if (user is null)
                return new RegisterPatient { Message = "User not found" };
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.FirstOrDefault() == "Doctor")
            {
                var result = await _doctorService.LogInAsync(model);

                if (!result.IsAuthenticated)
                    return new RegisterPatient { Message = "LogIn as a doctor failed" };
                var DocToPatient = new RegisterPatient
                {
                    Age = result.Age,
                    Email = result.Email,
                    Gender = result.Gender,
                    NationalID = result.NationalID,
                    Name = result.Name,
                    Image = result.Image,
                    PhoneNumber = result.PhoneNumber,
                    Spicialty = result.Spicialty,
                    Token = result.Token,
                    Roles = new List<string> { "Doctor" },
                    IsAuthenticated = result.IsAuthenticated,
                    ClinicPhotos = result.ClinicPhotos,
                    SpecializeIn = result.SpecializeIn,
                    AppointmentPrice = result.AppointmentPrice,
                    Description = result.Description,
                    AboutDoctor = result.AboutDoctor,
            };
                return DocToPatient;
            }
            
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new RegisterPatient { Message = "National ID or Password is incorrect" };

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.NationalID == user.NationalID);
            if (patient == null)
                return new RegisterPatient { Message = "Patient details not found in the database" };

            var jwtSecurityToken = await _createJWTToken.CreateJwtToken(user);

            var registerModel = new RegisterPatient();

            registerModel.Image = patient.Image;
            registerModel.IsAuthenticated = true;
            registerModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            registerModel.Email = patient.Email;
            registerModel.Name = patient.Name;
            registerModel.NationalID = user.NationalID;
            registerModel.Gender = patient.Gender;
            registerModel.Status = patient.Status;
            registerModel.PhoneNumber = patient.PhoneNumber;
            registerModel.Age = patient.Age;
            registerModel.ExpiresOn = jwtSecurityToken.ValidTo;
            registerModel.Roles = new List<string> {"Patient"};

            return registerModel;
        }
        public async Task<Patient> UpdateAsync(UpdatePatientdto model)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.NationalID == model.NationalID);

            if (patient == null)
                return new Patient { };

            var img = await _saveToFirebase.ImageToFirebase(model.Image);
            patient.Image = img;
            patient.BloodType = model.BloodType;
            patient.Gender = model.Gender;
            patient.Status = model.Status;
            patient.PhoneNumber = model.PhoneNumber;
            patient.Age = model.Age;
            patient.Email = model.Email;
            patient.Name = model.Name;

            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();

            return patient;
        }
        public async Task<bool> DeleteAsync(string PatientId)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.NationalID == PatientId);
            if (patient is null)
                return false;

             _context.Patients.Remove(patient);
            _context.SaveChanges();

            return true;
        }
        public IEnumerable<Doctor> GetAllDoctors()
        {
            var result = _context.Doctors.ToList();

            return result;
        }
        public async Task<IEnumerable<Doctor>> SearchAsync(SearchDto model)
        {
            var query = _context.Doctors.AsQueryable();

            if (!string.IsNullOrEmpty(model.DocName))
                query = query.Where(d => d.Name.Contains(model.DocName));

            if (!string.IsNullOrEmpty(model.DocAddress))
                query = query.Where(d => d.Address.Contains(model.DocAddress));

            if (!string.IsNullOrEmpty(model.DocSpecialty))
                query = query.Where(d => d.Spicialty.Contains(model.DocSpecialty));

            var doctors = await query.ToListAsync();

            return doctors;
        }
        public async Task<List<Appointment>> GetAvailableAppointmentsAsync(string doctorId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.DoctorNationalID == doctorId && a.IsAvaliable)
                .ToListAsync();

            return appointments;
        }
        public async Task<Appointment> BookAppointmentAsync(int appointmentId, string patientId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.IsAvaliable);

            if (appointment == null)
            {
                return null;
            }

            appointment.IsAvaliable = false;
            appointment.PatientNationalID = patientId;

            _context.Update(appointment);
            await _context.SaveChangesAsync();

            return appointment;
        }
        public async Task<bool> SendInvestigatesAsync(InvestigateDto model)
        {
            List<string> imageUrls = new List<string>();

            foreach (var file in model.InvestigateImages)
            {
                string downloadUrl = await _saveToFirebase.ImageToFirebase(file);

                imageUrls.Add(downloadUrl);

            }
            foreach (var imageUrl in imageUrls)
            {
                
            }
            var investigate = new MedicalInvestigate
            {
                PatientNationalID = model.PatientNationalID,
                DoctorNationalID = model.DoctorNationalID,
                MedicalInves = imageUrls, // Store URL
                SendTime = DateTime.Now,
                Comment = model.Comment,
                Replay = "Not yet"
            };

            _context.MedicalInvestigates.Add(investigate);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<InvestigateReplayDto> GetInvestigatesReplayAsync(int investigateId)
        {
            var medicalInvestigate = await _context.MedicalInvestigates
                .FirstOrDefaultAsync(mi => mi.Id == investigateId);

            if (medicalInvestigate == null)
                return null;

            return new InvestigateReplayDto
            {
                InvestigateId = medicalInvestigate.Id,
                Replay = medicalInvestigate.Replay
            };
        }
        public async Task<bool> RequestToJoinAsDoctor(RequestToJoinAsDoctor model)
        {
            var info = await _context.doctorRequestsToJoins.FirstOrDefaultAsync(i => i.NationalID == model.NationalID);
            if (info is not null)
                return false;

            DoctorRequestsToJoin request = new DoctorRequestsToJoin
            {
                NationalID = model.NationalID,
                Address = model.Address,
                Age = model.Age,
                Email = model.Email,
                Gender = model.Gender,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                Spicialty = model.Spicialty
            };
            await _context.doctorRequestsToJoins.AddAsync(request);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> RateDoctorAsync(RateDoctorDto model)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.NationalID == model.DoctorNationalID);
            var patient = await _context.Patients.FirstOrDefaultAsync(d => d.NationalID == model.PatientNationalID);

            if (doctor is null || patient is null)
                return false;

            var doctorRating = new DoctorRating
            {
                DoctorNationalID = model.DoctorNationalID,
                PatientNationalID = model.PatientNationalID,
                Rating = model.Rating,
                Comments = model.Comments,
                RateTime = DateTime.Now
            };

            _context.DoctorRatings.Add(doctorRating);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}