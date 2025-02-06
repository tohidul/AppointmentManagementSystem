using AppointmentManagementSystem.API.Models.Entities;
using AppointmentManagementSystem.API.Models.DTOs;
using AppointmentManagementSystem.API.Repositories;
using AppointmentManagementSystem.API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppointmentManagementSystem.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;


        public UserService(IUserRepository userRepository, IMapper mapper, IPasswordHasher<User> passwordHasher, IConfiguration configuration, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _logger = logger;

        }

        public async Task<(long UserId, string Username)> RegisterUserAsync(UserDto userDto)
        {
            var existingUser = await _userRepository.GetUserByUsernameAsync(userDto.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username already exists.");
            }
            var user = _mapper.Map<User>(userDto);
            user.Password = _passwordHasher.HashPassword(user, userDto.Password);
            await _userRepository.AddUserAsync(user);
            return (user.UserId, user.Username);
        }
        public async Task<User> LoginAsync(UserDto userDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(userDto.Username);
            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.Password, userDto.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return user;
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim("custom:username", user.Username));
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
