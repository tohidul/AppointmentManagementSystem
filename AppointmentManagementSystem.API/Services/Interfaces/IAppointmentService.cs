using AppointmentManagementSystem.API.Models.DTOs;


namespace AppointmentManagementSystem.API.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> CreateAppointmentAsync(AppointmentDto appointmentDto);
        Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync();
        Task<AppointmentDto> GetAppointmentByIdAsync(long appointmentId);
        Task<AppointmentDto> UpdateAppointmentByIdAsync(long appointmentId, AppointmentDto appointmentDto);
        Task DeleteAppointmentByIdAsync(long appointmentId);
    }
}

