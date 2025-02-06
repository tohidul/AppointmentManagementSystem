using NUnit.Framework;
using Moq;
using AppointmentManagementSystem.API.Controllers;
using AppointmentManagementSystem.API.Models.DTOs;
using AppointmentManagementSystem.API.Models.ResponseModels;
using AppointmentManagementSystem.API.Models.ViewModels;
using AppointmentManagementSystem.API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AppointmentManagementSystem.API.Models.Entities;

namespace AppointmentManagementSystem.Test.Controllers
{
    [TestFixture]
    internal class UsersControllerTests
    {
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IUserService> _userServiceMock = null!;
        private UsersController _usersController = null!;

        [SetUp]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _userServiceMock = new Mock<IUserService>();
            _usersController = new UsersController(_mapperMock.Object, _userServiceMock.Object);
        }

        [Test]
        public async Task RegisterUserAsync_ShouldReturnOk_WhenUserIsRegistered()
        {
            // Arrange
            var userRegistrationData = new RegisterUserViewModel { Username = "user1", Password = "Test@1234" };
            var userDto = new UserDto { Username = "user1", Password = "Test@1234" };
            var userId = 1L;
            var username = "user1";

            _mapperMock.Setup(mapper => mapper.Map<UserDto>(userRegistrationData)).Returns(userDto);
            _userServiceMock.Setup(service => service.RegisterUserAsync(userDto)).ReturnsAsync((userId, username));

            // Act
            var result = await _usersController.RegisterUserAsync(userRegistrationData) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            if (result?.Value is RegisterUserResponseModel response)
            {
                Assert.That(response.UserId, Is.EqualTo(userId));
                Assert.That(response.Username, Is.EqualTo(username));
            }
        }

        [Test]
        public async Task RegisterUserAsync_ShouldReturnConflict_WhenUsernameAlreadyExists()
        {
            // Arrange
            var userRegistrationData = new RegisterUserViewModel { Username = "user1", Password = "Test@1234" };
            var userDto = new UserDto { Username = "user1", Password = "Test@1234" };

            _mapperMock.Setup(mapper => mapper.Map<UserDto>(userRegistrationData)).Returns(userDto);
            _userServiceMock.Setup(service => service.RegisterUserAsync(userDto)).ThrowsAsync(new InvalidOperationException("Username already exists."));

            // Act
            var result = await _usersController.RegisterUserAsync(userRegistrationData) as ConflictObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(409));
            if (result?.Value is { } response)
            {
                Assert.That(response.GetType().GetProperty("message")?.GetValue(response, null), Is.EqualTo("Username already exists."));
            }
        }

        [Test]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var loginData = new LoginUserViewModel { Username = "user1", Password = "Test@1234" };
            var userDto = new UserDto { Username = "user1", Password = "Test@1234" };
            var user = new User { UserId = 1, Username = "user1", Password = "hashedpassword" };
            var token = "jwt_token";

            _mapperMock.Setup(mapper => mapper.Map<UserDto>(loginData)).Returns(userDto);
            _userServiceMock.Setup(service => service.LoginAsync(userDto)).ReturnsAsync(user);
            _userServiceMock.Setup(service => service.GenerateToken(user)).Returns(token);

            // Act
            var result = await _usersController.Login(loginData) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            if (result?.Value is LoginUserResponseModel response)
            {
                Assert.That(response.Username, Is.EqualTo(user.Username));
                Assert.That(response.JwtToken, Is.EqualTo(token));
            }
        }

        [Test]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginData = new LoginUserViewModel { Username = "user1", Password = "Test@1234" };
            var userDto = new UserDto { Username = "user1", Password = "Test@1234" };

            _mapperMock.Setup(mapper => mapper.Map<UserDto>(loginData)).Returns(userDto);
            _userServiceMock.Setup(service => service.LoginAsync(userDto)).ReturnsAsync((User)null);

            // Act
            var result = await _usersController.Login(loginData) as UnauthorizedObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(401));
            if (result?.Value is { } response)
            {
                Assert.That(response.GetType().GetProperty("message")?.GetValue(response, null), Is.EqualTo("Username or Password incorrect"));
            }
        }
    }
}




