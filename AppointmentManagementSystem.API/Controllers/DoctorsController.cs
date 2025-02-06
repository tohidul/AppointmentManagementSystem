using AppointmentManagementSystem.API.CustomActionFilters;
using AppointmentManagementSystem.API.Exceptions;
using AppointmentManagementSystem.API.Models.DTOs;
using AppointmentManagementSystem.API.Models.ResponseModels;
using AppointmentManagementSystem.API.Models.ViewModels;
using AppointmentManagementSystem.API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDoctorService _doctorService;

        public DoctorsController(IMapper mapper, IDoctorService doctorService)
        {
            _mapper = mapper;
            _doctorService = doctorService;
        }
        [HttpPost]
        [Route("Create")]
        [Authorize]
        public async Task<IActionResult> CreateDoctorAsync([FromBody] CreateDoctorViewModel doctorData)
        {
            var doctorDto = _mapper.Map<DoctorDto>(doctorData);
            var (doctorId, doctorName) = await _doctorService.CreateDoctorAsync(doctorDto);
            var response = new DoctorResponseModel { DoctorId = doctorId, DoctorName = doctorName };
            return Ok(response);
        }

        [HttpGet]
        [Route("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAllDoctorsAsync()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            var response = _mapper.Map<DoctorResponseModel[]>(doctors);
            return Ok(response);
        }

        [HttpGet]
        [Route("GetById/{doctorId}")]
        [Authorize]
        public async Task<IActionResult> GetDoctorByIdAsync(long doctorId)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
            {
                return NotFound(new { message = "Doctor not found" });
            }
            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            var response = _mapper.Map<DoctorResponseModel>(doctorDto);
            return Ok(response);
        }

        [HttpPut]
        [Route("Update/{doctorId}")]
        [Authorize]
        public async Task<IActionResult> UpdateDoctorByIdAsync(long doctorId, [FromBody] CreateDoctorViewModel doctorData)
        {
            try 
            { 
                var doctorDto = _mapper.Map<DoctorDto>(doctorData);
                var updatedDoctor = await _doctorService.UpdateDoctorByIdAsync(doctorId, doctorDto);
                var response = _mapper.Map<DoctorResponseModel>(updatedDoctor);
                return Ok(response);
            }
            catch (DoctorNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteDoctorByIdAsync(long id)
        {
            try
            {
                await _doctorService.DeleteDoctorByIdAsync(id);
                return Ok(new { message = $"Doctor with Id: {id} deleted successfully." });
            }
            catch (DoctorNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
