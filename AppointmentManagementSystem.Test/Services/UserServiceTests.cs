using NUnit.Framework;
using Moq;
using AppointmentManagementSystem.API.Models.Entities;
using AppointmentManagementSystem.API.Models.DTOs;
using AppointmentManagementSystem.API.Repositories;
using AppointmentManagementSystem.API.Services;
using AppointmentManagementSystem.API.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentManagementSystem.Test.Services
{
    [TestFixture]
    internal class UserServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IPasswordHasher<User>> _passwordHasherMock = null!;
        private Mock<IConfiguration> _configurationMock = null!;
        private Mock<ILogger<UserService>> _loggerMock = null!;
        private UserService _userService = null!;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _userService = new UserService(_userRepositoryMock.Object, _mapperMock.Object, _passwordHasherMock.Object, _configurationMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task RegisterUserAsync_ShouldRegisterUser_WhenUsernameDoesNotExist()
        {
            // Arrange
            var userDto = new UserDto { Username = "user1", Password = "test" };
            var user = new User { UserId = 1, Username = "user1", Password = "hashedpassword" };

            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(userDto.Username)).ReturnsAsync((User)null);
            _mapperMock.Setup(mapper => mapper.Map<User>(userDto)).Returns(user);
            _passwordHasherMock.Setup(hasher => hasher.HashPassword(user, userDto.Password)).Returns("hashedpassword");

            // Act
            var result = await _userService.RegisterUserAsync(userDto);

            // Assert
            _userRepositoryMock.Verify(repo => repo.AddUserAsync(user), Times.Once);
            Assert.That(result.UserId, Is.EqualTo(user.UserId));
            Assert.That(result.Username, Is.EqualTo(user.Username));
        }

        [Test]
        public async Task LoginAsync_ShouldReturnUser_WhenCredentialsAreValid()
        {
            // Arrange
            var userDto = new UserDto { Username = "user1", Password = "test" };
            var user = new User { UserId = 1, Username = "user1", Password = "hashedpassword" };

            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(userDto.Username)).ReturnsAsync(user);
            _passwordHasherMock.Setup(hasher => hasher.VerifyHashedPassword(user, user.Password, userDto.Password)).Returns(PasswordVerificationResult.Success);

            // Act
            var result = await _userService.LoginAsync(userDto);

            // Assert
            Assert.That(result, Is.EqualTo(user));
        }

        [Test]
        public void GenerateToken_ShouldReturnToken_WhenUserIsValid()
        {
            // Arrange
            var user = new User { UserId = 1, Username = "user1" };
            _configurationMock.Setup(config => config["Jwt:Key"]).Returns("supersecretkey12345678901234567890");
            _configurationMock.Setup(config => config["Jwt:Issuer"]).Returns("issuer");
            _configurationMock.Setup(config => config["Jwt:Audience"]).Returns("audience");

            // Act
            var token = _userService.GenerateToken(user);

            // Assert
            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.Not.Empty);
        }





    }
}



