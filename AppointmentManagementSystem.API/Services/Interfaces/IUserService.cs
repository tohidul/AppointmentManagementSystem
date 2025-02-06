using AppointmentManagementSystem.API.Models.DTOs;
using AppointmentManagementSystem.API.Models.Entities;

namespace AppointmentManagementSystem.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<(long UserId, string Username)> RegisterUserAsync(UserDto userDto);
        Task<User> LoginAsync(UserDto userDto);
        string GenerateToken(User user);
    }
}
