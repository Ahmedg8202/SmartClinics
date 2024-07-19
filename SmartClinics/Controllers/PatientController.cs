using SmartClinics.DTOs;
using SmartClinics.DTOs.Patient;
using SmartClinics.Models;
using SmartClinics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmartClinics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogInAsync([FromBody] LogIndto model)
        {
            if (!ModelState.IsValid)
                return BadRequest("ModelState");

            var result = await _patientService.LogInAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreatePatientAsync([FromForm] RegisterPatientdto model)
        {
            if (!ModelState.IsValid)
                return BadRequest("ModelState");

            var result = await _patientService.CreateAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        //[Authorize(Roles ="Patient")]
        [HttpPut("EditProfile")]
        public async Task<IActionResult> Edit([FromForm] UpdatePatientdto model)
        {
            if (!ModelState.IsValid)
                return BadRequest("ModelState");

            var result = await _patientService.UpdateAsync(model);

            if(result.NationalID is null)
                return BadRequest("Patient not found");

            return Ok(result);
        }

       // [Authorize(Roles = "Patient")]
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string PatientId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _patientService.DeleteAsync(PatientId);
            if (!result)
                return BadRequest();

            return Ok();
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(SearchDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _patientService.SearchAsync(model);

            if (result == null || !result.Any())
                return NotFound("No doctors found with the given criteria");

            return Ok(result);
        }

        [HttpGet("AvailableAppointments/{doctorId}")]
        public async Task<IActionResult> GetAvailableAppointments(string doctorId)
        {
            var appointments = await _patientService.GetAvailableAppointmentsAsync(doctorId);
            if (appointments == null || !appointments.Any())
            {
                return NotFound("No available appointments found for the given doctor.");
            }

            return Ok(appointments);
        }

        //[Authorize(Roles ="Patient")]
        [HttpPost("BookAppointment")]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDto dto)
        {
            var appointment = await _patientService.BookAppointmentAsync(dto.AppointmentId, dto.PatientId);
            if (appointment == null)
            {
                return BadRequest(new { Message = "Failed to book appointment. The appointment may not be available or already booked." });
            }

            return Ok(new { Message = "Appointment booked successfully.", Appointment = appointment });
        }

        //[Authorize(Roles = "Patient")]
        [HttpPost("SendInvestigates")]
        public async Task<IActionResult> SendInvestigates([FromForm] InvestigateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _patientService.SendInvestigatesAsync(model);

            if (!result)
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while sending investigates.");

            return Ok("Investigates sent successfully.");
        }

        //[Authorize(Roles = "Patient")]
        [HttpGet("ViewInvestigatesReplay")]
        public async Task<IActionResult> ViewInvestigatesReplay(int investigateId)
        {
            var replay = await _patientService.GetInvestigatesReplayAsync(investigateId);

            if (replay == null)
                return NotFound("Investigates replay not found.");

            return Ok(replay);
        }

        //[Authorize(Roles ="Patient")]
        [HttpPost("RateDoctor")]
        public async Task<IActionResult> RateDoctor([FromBody] RateDoctorDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _patientService.RateDoctorAsync(model);

            if (!result)
                return BadRequest("Rating failed.");

            return Ok("Doctor rated successfully.");
        }

        [HttpPost("JoinAsADoctor")]
        public async Task<IActionResult> RequestToJoinAsADoctorAsync(RequestToJoinAsDoctor model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _patientService.RequestToJoinAsDoctor(model);
            if (result is false)
                return BadRequest("this info already registered or failed to send request");

            return Ok("Request has sent successfully");
        }

        [HttpPost("GetAllDoctors")]
        public async Task<IActionResult> GetAllDoctors()
        {
            var result = _patientService.GetAllDoctors();
            return Ok(result);
        }
    }
}
