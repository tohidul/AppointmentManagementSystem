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
using AppointmentManagementSystem.API.Models.Entities;
using AppointmentManagementSystem.API.Exceptions;

namespace AppointmentManagementSystem.Test.Controllers
{
    [TestFixture]
    internal class DoctorsControllerTests
    {
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IDoctorService> _doctorServiceMock = null!;
        private DoctorsController _doctorsController = null!;

        [SetUp]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _doctorServiceMock = new Mock<IDoctorService>();
            _doctorsController = new DoctorsController(_mapperMock.Object, _doctorServiceMock.Object);
        }

        [Test]
        public async Task CreateDoctorAsync_ShouldReturnOk_WhenDoctorIsCreated()
        {
            // Arrange
            var doctorData = new CreateDoctorViewModel { DoctorName = "doctor1" };
            var doctorDto = new DoctorDto { DoctorName = "doctor1" };
            var doctorId = 1L;
            var doctorName = "doctor1";

            _mapperMock.Setup(mapper => mapper.Map<DoctorDto>(doctorData)).Returns(doctorDto);
            _doctorServiceMock.Setup(service => service.CreateDoctorAsync(doctorDto)).ReturnsAsync((doctorId, doctorName));

            // Act
            var result = await _doctorsController.CreateDoctorAsync(doctorData) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            if (result?.Value is DoctorResponseModel response)
            {
                Assert.That(response.DoctorId, Is.EqualTo(doctorId));
                Assert.That(response.DoctorName, Is.EqualTo(doctorName));
            }
        }

        [Test]
        public async Task GetAllDoctorsAsync_ShouldReturnOk_WithAllDoctors()
        {
            // Arrange
            var doctors = new List<DoctorDto>
            {
                new DoctorDto { DoctorId = 1, DoctorName = "doctor1" },
                new DoctorDto { DoctorId = 2, DoctorName = "doctor2" }
            };
            var doctorResponseModels = new List<DoctorResponseModel>
            {
                new DoctorResponseModel { DoctorId = 1, DoctorName = "doctor1" },
                new DoctorResponseModel { DoctorId = 2, DoctorName = "doctor2" }
            };

            _doctorServiceMock.Setup(service => service.GetAllDoctorsAsync()).ReturnsAsync(doctors);
            _mapperMock.Setup(mapper => mapper.Map<DoctorResponseModel[]>(doctors)).Returns(doctorResponseModels.ToArray());

            // Act
            var result = await _doctorsController.GetAllDoctorsAsync() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            if (result?.Value is DoctorResponseModel[] response)
            {
                Assert.That(response.Length, Is.EqualTo(2));
            }
        }

        [Test]
        public async Task GetDoctorByIdAsync_ShouldReturnOk_WhenDoctorExists()
        {
            // Arrange
            var doctorId = 1L;
            var doctor = new Doctor { DoctorId = doctorId, DoctorName = "doctor1" };
            var doctorDto = new DoctorDto { DoctorId = doctorId, DoctorName = "doctor1" };
            var doctorResponseModel = new DoctorResponseModel { DoctorId = doctorId, DoctorName = "doctor1" };

            _doctorServiceMock.Setup(service => service.GetDoctorByIdAsync(doctorId)).ReturnsAsync(doctor);
            _mapperMock.Setup(mapper => mapper.Map<DoctorDto>(doctor)).Returns(doctorDto);
            _mapperMock.Setup(mapper => mapper.Map<DoctorResponseModel>(doctorDto)).Returns(doctorResponseModel);

            // Act
            var result = await _doctorsController.GetDoctorByIdAsync(doctorId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            if (result?.Value is DoctorResponseModel response)
            {
                Assert.That(response.DoctorId, Is.EqualTo(doctorId));
                Assert.That(response.DoctorName, Is.EqualTo("doctor1"));
            }
        }

        [Test]
        public async Task GetDoctorByIdAsync_ShouldReturnNotFound_WhenDoctorDoesNotExist()
        {
            // Arrange
            var doctorId = 1L;
            _doctorServiceMock.Setup(service => service.GetDoctorByIdAsync(doctorId)).ReturnsAsync((Doctor)null);

            // Act
            var result = await _doctorsController.GetDoctorByIdAsync(doctorId) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(404));
            if (result?.Value is { } response)
            {
                Assert.That(response.GetType().GetProperty("message")?.GetValue(response, null), Is.EqualTo("Doctor not found"));
            }
        }

        [Test]
        public async Task UpdateDoctorByIdAsync_ShouldReturnOk_WhenDoctorIsUpdated()
        {
            // Arrange
            var doctorId = 1L;
            var doctorData = new CreateDoctorViewModel { DoctorName = "newdoctorname" };
            var doctorDto = new DoctorDto { DoctorName = "newdoctorname" };
            var updatedDoctorDto = new DoctorDto { DoctorId = doctorId, DoctorName = "newdoctorname" };
            var doctorResponseModel = new DoctorResponseModel { DoctorId = doctorId, DoctorName = "newdoctorname" };

            _mapperMock.Setup(mapper => mapper.Map<DoctorDto>(doctorData)).Returns(doctorDto);
            _doctorServiceMock.Setup(service => service.UpdateDoctorByIdAsync(doctorId, doctorDto)).ReturnsAsync(updatedDoctorDto);
            _mapperMock.Setup(mapper => mapper.Map<DoctorResponseModel>(updatedDoctorDto)).Returns(doctorResponseModel);

            // Act
            var result = await _doctorsController.UpdateDoctorByIdAsync(doctorId, doctorData) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            if (result?.Value is DoctorResponseModel response)
            {
                Assert.That(response.DoctorId, Is.EqualTo(doctorId));
                Assert.That(response.DoctorName, Is.EqualTo("newdoctorname"));
            }
        }

        [Test]
        public async Task UpdateDoctorByIdAsync_ShouldReturnNotFound_WhenDoctorDoesNotExist()
        {
            // Arrange
            var doctorId = 1L;
            var doctorData = new CreateDoctorViewModel { DoctorName = "newdoctorname" };
            var doctorDto = new DoctorDto { DoctorName = "newdoctorname" };

            _mapperMock.Setup(mapper => mapper.Map<DoctorDto>(doctorData)).Returns(doctorDto);
            _doctorServiceMock.Setup(service => service.UpdateDoctorByIdAsync(doctorId, doctorDto)).ThrowsAsync(new DoctorNotFoundException("Doctor with ID 1 not found."));

            // Act
            var result = await _doctorsController.UpdateDoctorByIdAsync(doctorId, doctorData) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(404));
            if (result?.Value is { } response)
            {
                Assert.That(response.GetType().GetProperty("message")?.GetValue(response, null), Is.EqualTo("Doctor with ID 1 not found."));
            }
        }

        [Test]
        public async Task DeleteDoctorByIdAsync_ShouldReturnOk_WhenDoctorIsDeleted()
        {
            // Arrange
            var doctorId = 1L;

            // Act
            var result = await _doctorsController.DeleteDoctorByIdAsync(doctorId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            if (result?.Value is { } response)
            {
                Assert.That(response.GetType().GetProperty("message")?.GetValue(response, null), Is.EqualTo($"Doctor with Id: {doctorId} deleted successfully."));
            }
        }

        [Test]
        public async Task DeleteDoctorByIdAsync_ShouldReturnNotFound_WhenDoctorDoesNotExist()
        {
            // Arrange
            var doctorId = 1L;
            _doctorServiceMock.Setup(service => service.DeleteDoctorByIdAsync(doctorId)).ThrowsAsync(new DoctorNotFoundException("Doctor with ID 1 not found."));

            // Act
            var result = await _doctorsController.DeleteDoctorByIdAsync(doctorId) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(404));
            if (result?.Value is { } response)
            {
                Assert.That(response.GetType().GetProperty("message")?.GetValue(response, null), Is.EqualTo("Doctor with ID 1 not found."));
            }
        }
    }
}



