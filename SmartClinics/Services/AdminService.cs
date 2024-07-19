using SmartClinics.Data;
using SmartClinics.DTOs;
using SmartClinics.DTOs.Doctor;
using SmartClinics.DTOs.Patient;
using SmartClinics.Helers;
using SmartClinics.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Admindto = SmartClinics.DTOs.Admindto;

namespace SmartClinics.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPatientService _patientService;
        private readonly JWT _jwt;
        private readonly CreateJWTToken _createJWTToken;

        public AdminService(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IOptions<JWT> jwt, IPatientService patientService, CreateJWTToken createJWTToken)
        {
            _context = context;
            _userManager = userManager;
            _jwt = jwt.Value;
            _patientService = patientService;
            _createJWTToken = createJWTToken;
        }

    
        public async Task<RegisterAdmin> CreateAsync(Admindto model)
        {
            if (await _userManager.FindByNameAsync(model.UserName) is not null)
                return new RegisterAdmin { Message = "This userName is already Taken" };

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                //PasswordHash = model.Password,
                NationalID = "00000000000000"
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                return new RegisterAdmin { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "Admin");

            var roleList = await _userManager.GetRolesAsync(user);
            var jwtSecurityToken = await _createJWTToken.CreateJwtToken(user);

            return new RegisterAdmin
            {
                UserName = model.UserName,
                IsAuthenticated = true,
                Roles = roleList.ToList(),
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            };
        }

        public async Task<RegisterPatient> LogInAsync(LogIndto model)
        {
            var user = await _userManager.FindByNameAsync(model.NationalID);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new RegisterPatient { Message = "UserName or Password is incorrect" };

            /*var patient = await _context.Admins.FirstOrDefaultAsync(p => p.UserName == user.UserName);
            if (patient == null)
                return new RegisterAdmin { Message = "Admin details not found in the database" };
            */

            var jwtSecurityToken = await _createJWTToken.CreateJwtToken(user);

            var registerModel = new RegisterPatient();
            registerModel.IsAuthenticated = true;
            registerModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            registerModel.NationalID = user.UserName;
            registerModel.ExpiresOn = jwtSecurityToken.ValidTo;
            registerModel.Roles = new List<string> { "Admin" };
            //_log.Information('user dfdsfdlogg
            return registerModel;
        }

        public async Task<RegisterDoc> AddDoctorAsync(RegisterDocdto model)
        {
            if (await _userManager.Users.FirstOrDefaultAsync(u => u.NationalID == model.NationalID) is not null)
                return new RegisterDoc { Message = "This National ID is already registered" };

            var user = new ApplicationUser
            {
                NationalID = model.NationalID,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                UserName = model.Name.Replace(" ", string.Empty)
            };

            var doctor = new Doctor
            {
                Name = model.Name,
                Age = model.Age,
                Email = model.Email,
                Gender = model.Gender,
                NationalID = model.NationalID,
                PhoneNumber = model.PhoneNumber,
                Spicialty = model.Spicialty,
                Address = model.Address
            };

            await _context.Doctors.AddAsync(doctor);

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                return new RegisterDoc { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "Doctor");

            await _context.SaveChangesAsync();

            var roleList = await _userManager.GetRolesAsync(user);
            var jwtSecurityToken = await _createJWTToken.CreateJwtToken(user);

            var existDoctor = await _context.doctorRequestsToJoins.FirstOrDefaultAsync(d => d.NationalID == doctor.NationalID);
            if (existDoctor is not null)
            {
                _context.doctorRequestsToJoins.Remove(existDoctor);
                _context.SaveChanges();
            }

            return new RegisterDoc
            {
                Name = model.Name,
                Age = model.Age,
                Email = model.Email,
                Gender = model.Gender,
                NationalID = model.NationalID,
                PhoneNumber = model.PhoneNumber,
                Spicialty = model.Spicialty,
                IsAuthenticated = true,
                Roles = roleList.ToList(),
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            };

        }

        public async Task<RegisterDoc> DeleteDoctorAsync(string doctorId)
        {
            var model = await _userManager.Users.FirstOrDefaultAsync(u => u.NationalID == doctorId);
            if (model is null)
                return new RegisterDoc { Message = "Doctor with this ID not exists" };

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.NationalID == doctorId);

            _context.Doctors.Remove(doctor);
            _context.SaveChanges();

            return new RegisterDoc
            {
                Name = doctor.Name,
                NationalID = doctor.NationalID,
                Age = doctor.Age,
                Email = doctor.Email,
                Spicialty = doctor.Spicialty,
                PhoneNumber = doctor.PhoneNumber
            };
        }

        public async Task<List<DoctorRequestsToJoin>> PendingRequestsAsync()
        {
            var requests = _context.doctorRequestsToJoins.Where(r => r.Viewed == false);
            return await requests.ToListAsync();
        }

        /*public async Task<RegisterPatient> AddDoctorAsync(RegisterPatientdto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return new RegisterPatient { Message = "This Email is already registered" };

            var user = new ApplicationUser
            {
                UserName = model.Name,
                Email = model.Email,
                NationalID = model.NationalID
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                return new RegisterPatient { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "Doctor");

            var patient = new Doctor
            {
                NationalID = model.NationalID,
                Name = model.Name,
                Email = model.Email,
                Gender = model.Gender,
                DOfB = model.DOfB,
                
            };

            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();

            var roleList = await _userManager.GetRolesAsync(user);
            var jwtSecurityToken = await _createJWTToken.CreateJwtToken(user);

            return new RegisterPatient
            {
                Name = model.Name,
                NationalID = model.NationalID,
                Email = model.Email,
                Gender = model.Gender,
                DOfB = model.DOfB,
                IsAuthenticated = true,
                Roles = roleList.ToList(),
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            };
            throw new NotImplementedException();
        }*/
    }
}
