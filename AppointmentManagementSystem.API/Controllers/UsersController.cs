using AppointmentManagementSystem.API.CustomActionFilters;
using AppointmentManagementSystem.API.Models.DTOs;
using AppointmentManagementSystem.API.Models.ResponseModels;
using AppointmentManagementSystem.API.Models.ViewModels;
using AppointmentManagementSystem.API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UsersController(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        [HttpPost]
        [Route("Register")]
        [ValidateModelAttribute]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserViewModel userRegistrationData)
        {
            var userDto = _mapper.Map<UserDto>(userRegistrationData);
            try {
                var (userId, username) = await _userService.RegisterUserAsync(userDto);
                var response = new RegisterUserResponseModel
                {
                    UserId = userId,
                    Username = username
                };
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }


        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserViewModel loginData)
        {
            var userDto = _mapper.Map<UserDto>(loginData);
            var user = await _userService.LoginAsync(userDto);
            if (user == null)
            {
                return Unauthorized(new { message = "Username or Password incorrect" });
            }
            var token = _userService.GenerateToken(user);
            var response = new LoginUserResponseModel
            {
                Username = user.Username,
                JwtToken = token
            };
            return Ok(response);
        }   
    }


}
