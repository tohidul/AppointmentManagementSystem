using NUnit.Framework;
using Moq;
using AppointmentManagementSystem.API.Controllers;
using AppointmentManagementSystem.API.Models.DTOs;
using AppointmentManagementSystem.API.Models.ResponseModels;
using AppointmentManagementSystem.API.Models.ViewModels;
using AppointmentManagementSystem.API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppointmentManagementSystem.API.Exceptions;

namespace AppointmentManagementSystem.Test.Controllers
{
    [TestFixture]
    internal class AppointmentsControllerTests
    {
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IAppointmentService> _appointmentServiceMock = null!;
        private AppointmentsController _appointmentsController = null!;

        [SetUp]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _appointmentServiceMock = new Mock<IAppointmentService>();
            _appointmentsController = new AppointmentsController(_mapperMock.Object, _appointmentServiceMock.Object);
        }

        [Test]
        public async Task CreateAppointmentAsync_ShouldReturnOk_WhenAppointmentIsCreated()
        {
            // Arrange
            var appointmentData = new CreateAppointmentViewModel { PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now.AddDays(1), DoctorId = 1 };
            var appointmentDto = new AppointmentDto { PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now.AddDays(1), DoctorId = 1 };
            var createdAppointmentDto = new AppointmentDto { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now.AddDays(1), DoctorId = 1 };
            var responseModel = new CreateAppointmentResponseModel { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now.AddDays(1), Doctor = new DoctorResponseModel { DoctorId = 1, DoctorName = "doctor1" } };

            _mapperMock.Setup(mapper => mapper.Map<AppointmentDto>(appointmentData)).Returns(appointmentDto);
            _appointmentServiceMock.Setup(service => service.CreateAppointmentAsync(appointmentDto)).ReturnsAsync(createdAppointmentDto);
            _mapperMock.Setup(mapper => mapper.Map<CreateAppointmentResponseModel>(createdAppointmentDto)).Returns(responseModel);

            // Act
            var result = await _appointmentsController.CreateAppointmentAsync(appointmentData) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            if (result?.Value is CreateAppointmentResponseModel response)
            {
                Assert.That(response.AppointmentId, Is.EqualTo(1));
                Assert.That(response.PatientName, Is.EqualTo("patient1"));
                Assert.That(response.Doctor.DoctorId, Is.EqualTo(1));
            }
        }

        [Test]
        public async Task CreateAppointmentAsync_ShouldReturnNotFound_WhenDoctorDoesNotExist()
        {
            // Arrange
            var appointmentData = new CreateAppointmentViewModel { PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now.AddDays(1), DoctorId = 1 };
            var appointmentDto = new AppointmentDto { PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now.AddDays(1), DoctorId = 1 };

            _mapperMock.Setup(mapper => mapper.Map<AppointmentDto>(appointmentData)).Returns(appointmentDto);
            _appointmentServiceMock.Setup(service => service.CreateAppointmentAsync(appointmentDto)).ThrowsAsync(new DoctorNotFoundException("Doctor with ID 1 not found."));

            // Act
            var result = await _appointmentsController.CreateAppointmentAsync(appointmentData) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(404));
            if (result?.Value is { } response)
            {
                Assert.That(response.GetType().GetProperty("message")?.GetValue(response, null), Is.EqualTo("Doctor with ID 1 not found."));
            }
        }

        [Test]
        public async Task GetAllAppointmentsAsync_ShouldReturnOk_WithAllAppointments()
        {
            // Arrange
            var appointments = new List<AppointmentDto>
            {
                new AppointmentDto { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now.AddDays(1), DoctorId = 1 },
                new AppointmentDto { AppointmentId = 2, PatientName = "patient2", PatientContactInformation = "contact2", AppointmentDateAndTime = DateTime.Now.AddDays(1), DoctorId = 2 }
            };
            var responseModels = new List<CreateAppointmentResponseModel>
            {
                new CreateAppointmentResponseModel { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now.AddDays(1), Doctor = new DoctorResponseModel { DoctorId = 1, DoctorName = "doctor1" } },
                new CreateAppointmentResponseModel { AppointmentId = 2, PatientName = "patient2", PatientContactInformation = "contact2", AppointmentDateAndTime = DateTime.Now.AddDays(1), Doctor = new DoctorResponseModel { DoctorId = 2, DoctorName = "doctor2" } }
            };

            _appointmentServiceMock.Setup(service => service.GetAllAppointmentsAsync()).ReturnsAsync(appointments);
            _mapperMock.Setup(mapper => mapper.Map<CreateAppointmentResponseModel[]>(appointments)).Returns(responseModels.ToArray());

            // Act
            var result = await _appointmentsController.GetAllAppoiintmentsAsync() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            if (result?.Value is CreateAppointmentResponseModel[] response)
            {
                Assert.That(response.Length, Is.EqualTo(2));
            }
        }

        [Test]
        public async Task GetAppointmentByIdAsync_ShouldReturnOk_WhenAppointmentExists()
        {
            // Arrange
            var appointmentId = 1L;
            var appointmentDto = new AppointmentDto { AppointmentId = appointmentId, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now.AddDays(1), DoctorId = 1 };
            var responseModel = new CreateAppointmentResponseModel { AppointmentId = appointmentId, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now.AddDays(1), Doctor = new DoctorResponseModel { DoctorId = 1, DoctorName = "doctor1" } };

            _appointmentServiceMock.Setup(service => service.GetAppointmentByIdAsync(appointmentId)).ReturnsAsync(appointmentDto);
            _mapperMock.Setup(mapper => mapper.Map<CreateAppointmentResponseModel>(appointmentDto)).Returns(responseModel);

            // Act
            var result = await _appointmentsController.GetAppointmentByIdAsync(appointmentId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            if (result?.Value is CreateAppointmentResponseModel response)
            {
                Assert.That(response.AppointmentId, Is.EqualTo(appointmentId));
                Assert.That(response.PatientName, Is.EqualTo("patient1"));
                Assert.That(response.Doctor.DoctorId, Is.EqualTo(1));
            }
        }

        [Test]
        public async Task GetAppointmentByIdAsync_ShouldReturnNotFound_WhenAppointmentDoesNotExist()
        {
            // Arrange
            var appointmentId = 1L;
            _appointmentServiceMock.Setup(service => service.GetAppointmentByIdAsync(appointmentId)).ReturnsAsync((AppointmentDto)null);

            // Act
            var result = await _appointmentsController.GetAppointmentByIdAsync(appointmentId) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(404));
            if (result?.Value is { } response)
            {
                Assert.That(response.GetType().GetProperty("message")?.GetValue(response, null), Is.EqualTo($"Appointment with Id: {appointmentId} not found."));
            }
        }

        [Test]
        public async Task UpdateAppointmentByIdAsync_ShouldReturnOk_WhenAppointmentIsUpdated()
        {
            // Arrange
            var appointmentId = 1L;
            var appointmentData = new CreateAppointmentViewModel { PatientName = "newpatientname", PatientContactInformation = "newcontact", AppointmentDateAndTime = DateTime.Now.AddDays(1), DoctorId = 1 };
            var appointmentDto = new AppointmentDto { PatientName = "newpatientname", PatientContactInformation = "newcontact", AppointmentDateAndTime = DateTime.Now.AddDays(1), DoctorId = 1 };
            var updatedAppointmentDto = new AppointmentDto { AppointmentId = appointmentId, PatientName = "newpatientname", PatientContactInformation = "newcontact", AppointmentDateAndTime = DateTime.Now.AddDays(1), DoctorId = 1 };
            var responseModel = new CreateAppointmentResponseModel { AppointmentId = appointmentId, PatientName = "newpatientname", PatientContactInformation = "newcontact", AppointmentDateAndTime = DateTime.Now.AddDays(1), Doctor = new DoctorResponseModel { DoctorId = 1, DoctorName = "doctor1" } };

            _mapperMock.Setup(mapper => mapper.Map<AppointmentDto>(appointmentData)).Returns(appointmentDto);
            _appointmentServiceMock.Setup(service => service.UpdateAppointmentByIdAsync(appointmentId, appointmentDto)).ReturnsAsync(updatedAppointmentDto);
            _mapperMock.Setup(mapper => mapper.Map<CreateAppointmentResponseModel>(updatedAppointmentDto)).Returns(responseModel);

            // Act
            var result = await _appointmentsController.UpdateAppointmentByIdAsync(appointmentId, appointmentData) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            if (result?.Value is CreateAppointmentResponseModel response)
            {
                Assert.That(response.AppointmentId, Is.EqualTo(appointmentId));
                Assert.That(response.PatientName, Is.EqualTo("newpatientname"));
                Assert.That(response.Doctor.DoctorId, Is.EqualTo(1));
            }
        }

        [Test]
        public async Task UpdateAppointmentByIdAsync_ShouldReturnNotFound_WhenAppointmentDoesNotExist()
        {
            // Arrange
            var appointmentId = 1L;
            var appointmentData = new CreateAppointmentViewModel { PatientName = "newpatientname", PatientContactInformation = "newcontact", AppointmentDateAndTime = DateTime.Now.AddDays(1), DoctorId = 1 };
            var appointmentDto = new AppointmentDto { PatientName = "newpatientname", PatientContactInformation = "newcontact", AppointmentDateAndTime = DateTime.Now.AddDays(1), DoctorId = 1 };

            _mapperMock.Setup(mapper => mapper.Map<AppointmentDto>(appointmentData)).Returns(appointmentDto);
            _appointmentServiceMock.Setup(service => service.UpdateAppointmentByIdAsync(appointmentId, appointmentDto)).ThrowsAsync(new AppointmentNotFoundException("Appointment with ID 1 not found."));

            // Act
            var result = await _appointmentsController.UpdateAppointmentByIdAsync(appointmentId, appointmentData) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(404));
            if (result?.Value is { } response)
            {
                Assert.That(response.GetType().GetProperty("message")?.GetValue(response, null), Is.EqualTo("Appointment with ID 1 not found."));
            }
        }

        [Test]
        public async Task UpdateAppointmentByIdAsync_ShouldReturnNotFound_WhenDoctorDoesNotExist()
        {
            // Arrange
            var appointmentId = 1L;
            var appointmentData = new CreateAppointmentViewModel { PatientName = "newpatientname", PatientContactInformation = "newcontact", AppointmentDateAndTime = DateTime.Now.AddDays(1), DoctorId = 1 };
            var appointmentDto = new AppointmentDto { PatientName = "newpatientname", PatientContactInformation = "newcontact", AppointmentDateAndTime = DateTime.Now.AddDays(1), DoctorId = 1 };

            _mapperMock.Setup(mapper => mapper.Map<AppointmentDto>(appointmentData)).Returns(appointmentDto);
            _appointmentServiceMock.Setup(service => service.UpdateAppointmentByIdAsync(appointmentId, appointmentDto)).ThrowsAsync(new DoctorNotFoundException("Doctor with ID 1 not found."));

            // Act
            var result = await _appointmentsController.UpdateAppointmentByIdAsync(appointmentId, appointmentData) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(404));
            if (result?.Value is { } response)
            {
                Assert.That(response.GetType().GetProperty("message")?.GetValue(response, null), Is.EqualTo("Doctor with ID 1 not found."));
            }
        }

        [Test]
        public async Task DeleteAppointmentByIdAsync_ShouldReturnOk_WhenAppointmentIsDeleted()
        {
            // Arrange
            var appointmentId = 1L;

            // Act
            var result = await _appointmentsController.DeleteAppointmentByIdAsync(appointmentId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            if (result?.Value is { } response)
            {
                Assert.That(response.GetType().GetProperty("message")?.GetValue(response, null), Is.EqualTo($"Appointment with Id: {appointmentId} deleted successfully."));
            }
        }

        [Test]
        public async Task DeleteAppointmentByIdAsync_ShouldReturnNotFound_WhenAppointmentDoesNotExist()
        {
            // Arrange
            var appointmentId = 1L;
            _appointmentServiceMock.Setup(service => service.DeleteAppointmentByIdAsync(appointmentId)).ThrowsAsync(new AppointmentNotFoundException("Appointment with ID 1 not found."));

            // Act
            var result = await _appointmentsController.DeleteAppointmentByIdAsync(appointmentId) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(404));
            if (result?.Value is { } response)
            {
                Assert.That(response.GetType().GetProperty("message")?.GetValue(response, null), Is.EqualTo("Appointment with ID 1 not found."));
            }
        }
    }
}




