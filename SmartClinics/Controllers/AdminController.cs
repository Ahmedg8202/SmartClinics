using SmartClinics.DTOs;
using SmartClinics.DTOs.Doctor;
using SmartClinics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmartClinics.Controllers
{
    //[Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;

        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateAdminAsync([FromBody] Admindto model)
        {
            if (!ModelState.IsValid)
                return BadRequest("ModelState");

            var result = await _adminService.CreateAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] LogIndto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.LogInAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("PendingRequests")]
        public async Task<IActionResult> PendingRequests()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.PendingRequestsAsync();
            if (result == null || !result.Any())
                return BadRequest("Error occurred when retrieving pending requests");

            return Ok(result);
        }


        //[Authorize(Roles = "Admin")]
        [HttpPost("AddDoctor")]
        public async Task<IActionResult> AddDoctorAsync([FromForm] RegisterDocdto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.AddDoctorAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        //[Authorize(Roles = "Admin")]
        [HttpDelete("DeleteDoctor")]
        public async Task<IActionResult> DeleteDoctorAsync(string doctorId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.DeleteDoctorAsync(doctorId);

            if(result.Message != null)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
