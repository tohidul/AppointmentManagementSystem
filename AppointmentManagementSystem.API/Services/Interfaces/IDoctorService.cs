using AppointmentManagementSystem.API.Models.DTOs;
using AppointmentManagementSystem.API.Models.Entities;

namespace AppointmentManagementSystem.API.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<(long DoctorId, string DoctorName)> CreateDoctorAsync(DoctorDto doctorDto);
        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();
        Task<Doctor> GetDoctorByIdAsync(long doctorId);
        Task<DoctorDto> UpdateDoctorByIdAsync(long doctorId, DoctorDto doctorDto);
        Task DeleteDoctorByIdAsync(long doctorId);
        
    }
}
