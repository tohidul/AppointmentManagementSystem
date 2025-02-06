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
    internal class AppointmentServiceTests
    {
        private Mock<IAppointmentRepository> _appointmentRepositoryMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IDoctorRepository> _doctorRepositoryMock = null!;
        private AppointmentService _appointmentService = null!;

        [SetUp]
        public void Setup()
        {
            _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
            _mapperMock = new Mock<IMapper>();
            _doctorRepositoryMock = new Mock<IDoctorRepository>();
            _appointmentService = new AppointmentService(_appointmentRepositoryMock.Object, _mapperMock.Object, _doctorRepositoryMock.Object);
        }

        [Test]
        public async Task CreateAppointmentAsync_ShouldCreateAppointment_WhenDoctorExists()
        {
            // Arrange
            var appointmentDto = new AppointmentDto { PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 };
            var doctor = new Doctor { DoctorId = 1, DoctorName = "doctor1" };
            var appointment = new Appointment { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 };

            _doctorRepositoryMock.Setup(repo => repo.GetDoctorByIdAsync(appointmentDto.DoctorId)).ReturnsAsync(doctor);
            _mapperMock.Setup(mapper => mapper.Map<Appointment>(appointmentDto)).Returns(appointment);
            _mapperMock.Setup(mapper => mapper.Map<AppointmentDto>(appointment)).Returns(appointmentDto);

            // Act
            var result = await _appointmentService.CreateAppointmentAsync(appointmentDto);

            // Assert
            _appointmentRepositoryMock.Verify(repo => repo.AddAppointmentAsync(appointment), Times.Once);
            Assert.That(result, Is.EqualTo(appointmentDto));
        }

        [Test]
        public void CreateAppointmentAsync_ShouldThrowDoctorNotFoundException_WhenDoctorDoesNotExist()
        {
            // Arrange
            var appointmentDto = new AppointmentDto { PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 };
            _doctorRepositoryMock.Setup(repo => repo.GetDoctorByIdAsync(appointmentDto.DoctorId)).ReturnsAsync((Doctor)null);

            // Act & Assert
            Assert.ThrowsAsync<DoctorNotFoundException>(async () => await _appointmentService.CreateAppointmentAsync(appointmentDto));
        }

        [Test]
        public async Task GetAllAppointmentsAsync_ShouldReturnAllAppointments()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 },
                new Appointment { AppointmentId = 2, PatientName = "patient2", PatientContactInformation = "contact2", AppointmentDateAndTime = DateTime.Now, DoctorId = 2 }
            };
            var appointmentDtos = new List<AppointmentDto>
            {
                new AppointmentDto { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 },
                new AppointmentDto { AppointmentId = 2, PatientName = "patient2", PatientContactInformation = "contact2", AppointmentDateAndTime = DateTime.Now, DoctorId = 2 }
            };

            _appointmentRepositoryMock.Setup(repo => repo.GetAllAppointmentsAsync()).ReturnsAsync(appointments);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<AppointmentDto>>(appointments)).Returns(appointmentDtos);

            // Act
            var result = await _appointmentService.GetAllAppointmentsAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAppointmentByIdAsync_ShouldReturnAppointment_WhenAppointmentExists()
        {
            // Arrange
            var appointment = new Appointment { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 };
            var appointmentDto = new AppointmentDto { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 };

            _appointmentRepositoryMock.Setup(repo => repo.GetAppointmentByIdAsync(1L)).ReturnsAsync(appointment);
            _mapperMock.Setup(mapper => mapper.Map<AppointmentDto>(appointment)).Returns(appointmentDto);

            // Act
            var result = await _appointmentService.GetAppointmentByIdAsync(1L);

            // Assert
            Assert.That(result, Is.EqualTo(appointmentDto));
        }

        [Test]
        public async Task UpdateAppointmentByIdAsync_ShouldUpdateAppointment_WhenAppointmentExists()
        {
            // Arrange
            var appointmentDto = new AppointmentDto { PatientName = "newpatientname", PatientContactInformation = "newcontact", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 };
            var appointment = new Appointment { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 };
            var doctor = new Doctor { DoctorId = 1, DoctorName = "doctor1" };

            _appointmentRepositoryMock.Setup(repo => repo.GetAppointmentByIdAsync(1L)).ReturnsAsync(appointment);
            _doctorRepositoryMock.Setup(repo => repo.GetDoctorByIdAsync(appointmentDto.DoctorId)).ReturnsAsync(doctor);
            _mapperMock.Setup(mapper => mapper.Map<AppointmentDto>(appointment)).Returns(appointmentDto);

            // Act
            var result = await _appointmentService.UpdateAppointmentByIdAsync(1L, appointmentDto);

            // Assert
            _appointmentRepositoryMock.Verify(repo => repo.UpdateAppointmentAsync(appointment), Times.Once);
            Assert.That(result.PatientName, Is.EqualTo("newpatientname"));
        }

        [Test]
        public void UpdateAppointmentByIdAsync_ShouldThrowAppointmentNotFoundException_WhenAppointmentDoesNotExist()
        {
            // Arrange
            var appointmentDto = new AppointmentDto { PatientName = "newpatientname", PatientContactInformation = "newcontact", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 };
            _appointmentRepositoryMock.Setup(repo => repo.GetAppointmentByIdAsync(1L)).ReturnsAsync((Appointment)null);

            // Act & Assert
            Assert.ThrowsAsync<AppointmentNotFoundException>(async () => await _appointmentService.UpdateAppointmentByIdAsync(1L, appointmentDto));
        }

        [Test]
        public void UpdateAppointmentByIdAsync_ShouldThrowDoctorNotFoundException_WhenDoctorDoesNotExist()
        {
            // Arrange
            var appointmentDto = new AppointmentDto { PatientName = "newpatientname", PatientContactInformation = "newcontact", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 };
            var appointment = new Appointment { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 };

            _appointmentRepositoryMock.Setup(repo => repo.GetAppointmentByIdAsync(1L)).ReturnsAsync(appointment);
            _doctorRepositoryMock.Setup(repo => repo.GetDoctorByIdAsync(appointmentDto.DoctorId)).ReturnsAsync((Doctor)null);

            // Act & Assert
            Assert.ThrowsAsync<DoctorNotFoundException>(async () => await _appointmentService.UpdateAppointmentByIdAsync(1L, appointmentDto));
        }

        [Test]
        public async Task DeleteAppointmentByIdAsync_ShouldDeleteAppointment_WhenAppointmentExists()
        {
            // Arrange
            var appointment = new Appointment { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 };
            _appointmentRepositoryMock.Setup(repo => repo.GetAppointmentByIdAsync(1L)).ReturnsAsync(appointment);

            // Act
            await _appointmentService.DeleteAppointmentByIdAsync(1L);

            // Assert
            _appointmentRepositoryMock.Verify(repo => repo.DeleteAppointmentAsync(1L), Times.Once);
        }

        [Test]
        public void DeleteAppointmentByIdAsync_ShouldThrowAppointmentNotFoundException_WhenAppointmentDoesNotExist()
        {
            // Arrange
            _appointmentRepositoryMock.Setup(repo => repo.GetAppointmentByIdAsync(1L)).ReturnsAsync((Appointment)null);

            // Act & Assert
            Assert.ThrowsAsync<AppointmentNotFoundException>(async () => await _appointmentService.DeleteAppointmentByIdAsync(1L));
        }
    }
}




