using SmartClinics.DTOs;
using SmartClinics.DTOs.Doctor;
using SmartClinics.DTOs.Patient;
using SmartClinics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SmartClinics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] LogIndto model)
        {
            if (!ModelState.IsValid)
                return BadRequest("ModelState");

            var result = await _doctorService.LogInAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        //[Authorize(Roles = "Doctor")]
        [HttpPut("EditProfile/{doctorId}")]
        public async Task<IActionResult> EditProfile(string doctorId, [FromForm] EditDoctorDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _doctorService.EditProfileAsync(doctorId, model);

            if (!result)
                return NotFound("Doctor not found or unable to update profile");

            return Ok("Profile updated successfully");
        }

        [HttpPost("AddAppointments")]
        public async Task<IActionResult> AddAppointments([FromBody] AddAppointmentsDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("ModelState");

            var result = await _doctorService.AddAppointments(dto);

            if (result == "Doctor Not Found")
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("ViewAppointments/{doctorId}")]
        public async Task<IActionResult> ViewAppointments(string doctorId)
        {
            if (!ModelState.IsValid)
                return BadRequest("ModelState"); 

            var result = await _doctorService.ViewAppointments(doctorId);

            if (result == null || !result.Any())
                return BadRequest("doctor not found");

            return Ok(result);
        }

        //[Authorize(Roles = "Doctor")]
        [HttpGet("ViewInvestigates")]
        public async Task<IActionResult> ViewInvestigates(string doctorNationalID)
        {
            var investigates = await _doctorService.GetInvestigatesAsync(doctorNationalID);

            var investigateDtos = investigates.Select(i => new
            {
                i.Id,
                i.PatientNationalID,
                i.DoctorNationalID,
                i.MedicalInves,
                i.SendTime,
                i.Comment,
                i.Replay
            }).ToList();

            return Ok(investigateDtos);
        }

       // [Authorize(Roles = "Doctor")]
        [HttpPost("ReplayInvestigates")]
        public async Task<IActionResult> ReplayInvestigates([FromBody] InvestigateReplayDto model)
        {
            var result = await _doctorService.SendInvestigateReplyAsync(model);

            if (!result)
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while replying to investigates.");

            return Ok("Reply sent successfully.");
        }

        [HttpGet("Rates")]
        public async Task<IActionResult> GetRates(string DoctorNationalID)
        {
            var result = await _doctorService.ALlRates(DoctorNationalID);
            if (!result.Any() || result == null)
                return BadRequest("Error in getting rates");

            return Ok(result);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = _doctorService.GetAllAsync();

            return Ok(result);
        }
    }
}
