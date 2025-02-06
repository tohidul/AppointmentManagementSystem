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
    public class AppointmentsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IMapper mapper, IAppointmentService appointmentService)
        {
            _mapper = mapper;
            _appointmentService = appointmentService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAppointmentAsync([FromBody] CreateAppointmentViewModel appointmentData)
        {
            var appointmentDto = _mapper.Map<AppointmentDto>(appointmentData);

            try
            {
                var createdAppointmentDto = await _appointmentService.CreateAppointmentAsync(appointmentDto);
                var response = _mapper.Map<CreateAppointmentResponseModel>(createdAppointmentDto);
                return Ok(response);
            }
            catch (DoctorNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllAppoiintmentsAsync()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            var response = _mapper.Map<CreateAppointmentResponseModel[]>(appointments);
            return Ok(response);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> GetAppointmentByIdAsync(long id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound(new { message = $"Appointment with Id: {id} not found." });
            }
            var response = _mapper.Map<CreateAppointmentResponseModel>(appointment);
            return Ok(response);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAppointmentByIdAsync(long id, [FromBody] CreateAppointmentViewModel appointmentData)
        {
            var appointmentDto = _mapper.Map<AppointmentDto>(appointmentData);
            try
            {
                var updatedAppointment = await _appointmentService.UpdateAppointmentByIdAsync(id, appointmentDto);
                var response = _mapper.Map<CreateAppointmentResponseModel>(updatedAppointment);
                return Ok(response);
            }
            catch (AppointmentNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DoctorNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAppointmentByIdAsync(long id)
        {
            try
            {
                await _appointmentService.DeleteAppointmentByIdAsync(id);
                return Ok(new { message = $"Appointment with Id: {id} deleted successfully." });
            }
            catch (AppointmentNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
