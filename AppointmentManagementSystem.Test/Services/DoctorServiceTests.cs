using NUnit.Framework;
using Moq;
using AppointmentManagementSystem.API.Models.Entities;
using AppointmentManagementSystem.API.Models.DTOs;
using AppointmentManagementSystem.API.Repositories.Interfaces;
using AppointmentManagementSystem.API.Services;
using AppointmentManagementSystem.API.Services.Interfaces;
using AppointmentManagementSystem.API.Exceptions;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentManagementSystem.Test.Services
{
    [TestFixture]
    internal class DoctorServiceTests
    {
        private Mock<IDoctorRepository> _doctorRepositoryMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private DoctorService _doctorService = null!;

        [SetUp]
        public void Setup()
        {
            _doctorRepositoryMock = new Mock<IDoctorRepository>();
            _mapperMock = new Mock<IMapper>();
            _doctorService = new DoctorService(_doctorRepositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task CreateDoctorAsync_ShouldCreateDoctor()
        {
            // Arrange
            var doctorDto = new DoctorDto { DoctorName = "doctor1" };
            var doctor = new Doctor { DoctorId = 1, DoctorName = "doctor1" };

            _mapperMock.Setup(mapper => mapper.Map<Doctor>(doctorDto)).Returns(doctor);

            // Act
            var result = await _doctorService.CreateDoctorAsync(doctorDto);

            // Assert
            _doctorRepositoryMock.Verify(repo => repo.AddDoctorAsync(doctor), Times.Once);
            Assert.That(result.DoctorId, Is.EqualTo(doctor.DoctorId));
            Assert.That(result.DoctorName, Is.EqualTo(doctor.DoctorName));
        }

        [Test]
        public async Task GetAllDoctorsAsync_ShouldReturnAllDoctors()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor { DoctorId = 1, DoctorName = "doctor1" },
                new Doctor { DoctorId = 2, DoctorName = "doctor2" }
            };
            var doctorDtos = new List<DoctorDto>
            {
                new DoctorDto { DoctorId = 1, DoctorName = "doctor1" },
                new DoctorDto { DoctorId = 2, DoctorName = "doctor2" }
            };

            _doctorRepositoryMock.Setup(repo => repo.GetAllDoctorsAsync()).ReturnsAsync(doctors);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<DoctorDto>>(doctors)).Returns(doctorDtos);

            // Act
            var result = await _doctorService.GetAllDoctorsAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetDoctorByIdAsync_ShouldReturnDoctor_WhenDoctorExists()
        {
            // Arrange
            var doctor = new Doctor { DoctorId = 1, DoctorName = "doctor1" };
            _doctorRepositoryMock.Setup(repo => repo.GetDoctorByIdAsync(1L)).ReturnsAsync(doctor);

            // Act
            var result = await _doctorService.GetDoctorByIdAsync(1L);

            // Assert
            Assert.That(result, Is.EqualTo(doctor));
        }

        [Test]
        public async Task UpdateDoctorByIdAsync_ShouldUpdateDoctor_WhenDoctorExists()
        {
            // Arrange
            var doctorDto = new DoctorDto { DoctorName = "newdoctorname" };
            var doctor = new Doctor { DoctorId = 1, DoctorName = "doctor1" };

            _doctorRepositoryMock.Setup(repo => repo.GetDoctorByIdAsync(1L)).ReturnsAsync(doctor);
            _mapperMock.Setup(mapper => mapper.Map<DoctorDto>(doctor)).Returns(doctorDto);

            // Act
            var result = await _doctorService.UpdateDoctorByIdAsync(1L, doctorDto);

            // Assert
            _doctorRepositoryMock.Verify(repo => repo.UpdateDoctorAsync(doctor), Times.Once);
            Assert.That(result.DoctorName, Is.EqualTo("newdoctorname"));
        }

        [Test]
        public void UpdateDoctorByIdAsync_ShouldThrowDoctorNotFoundException_WhenDoctorDoesNotExist()
        {
            // Arrange
            var doctorDto = new DoctorDto { DoctorName = "newdoctorname" };
            _doctorRepositoryMock.Setup(repo => repo.GetDoctorByIdAsync(1L)).ReturnsAsync((Doctor)null);

            // Act & Assert
            Assert.ThrowsAsync<DoctorNotFoundException>(async () => await _doctorService.UpdateDoctorByIdAsync(1L, doctorDto));
        }

        [Test]
        public async Task DeleteDoctorByIdAsync_ShouldDeleteDoctor_WhenDoctorExists()
        {
            // Arrange
            var doctor = new Doctor { DoctorId = 1, DoctorName = "doctor1" };
            _doctorRepositoryMock.Setup(repo => repo.GetDoctorByIdAsync(1L)).ReturnsAsync(doctor);

            // Act
            await _doctorService.DeleteDoctorByIdAsync(1L);

            // Assert
            _doctorRepositoryMock.Verify(repo => repo.DeleteDoctorAsync(1L), Times.Once);
        }

        [Test]
        public void DeleteDoctorByIdAsync_ShouldThrowDoctorNotFoundException_WhenDoctorDoesNotExist()
        {
            // Arrange
            _doctorRepositoryMock.Setup(repo => repo.GetDoctorByIdAsync(1L)).ReturnsAsync((Doctor)null);

            // Act & Assert
            Assert.ThrowsAsync<DoctorNotFoundException>(async () => await _doctorService.DeleteDoctorByIdAsync(1L));
        }
    }
}



