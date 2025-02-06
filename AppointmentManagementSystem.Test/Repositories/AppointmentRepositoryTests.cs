using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using AppointmentManagementSystem.API.Data;
using AppointmentManagementSystem.API.Models.Entities;
using AppointmentManagementSystem.API.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentManagementSystem.Test.Repositories
{
    [TestFixture]
    internal class AppointmentRepositoryTests
    {
        private AppointmentManagementSystemDbContext _context = null!;
        private AppointmentRepository _appointmentRepository = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppointmentManagementSystemDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new AppointmentManagementSystemDbContext(options);
            _appointmentRepository = new AppointmentRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllAppointmentsAsync_ShouldReturnAllAppointments()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor { DoctorId = 1, DoctorName = "doctor1" },
                new Doctor { DoctorId = 2, DoctorName = "doctor2" }
            };
            await _context.Doctors.AddRangeAsync(doctors);
            await _context.SaveChangesAsync();

            var appointments = new List<Appointment>
            {
                new Appointment { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 },
                new Appointment { AppointmentId = 2, PatientName = "patient2", PatientContactInformation = "contact2", AppointmentDateAndTime = DateTime.Now, DoctorId = 2 }
            };
            await _context.Appointments.AddRangeAsync(appointments);
            await _context.SaveChangesAsync();

            // Act
            var result = await _appointmentRepository.GetAllAppointmentsAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }


        [Test]
        public async Task GetAppointmentByIdAsync_ShouldReturnAppointment_WhenAppointmentExists()
        {
            // Arrange
            var doctor = new Doctor { DoctorId = 1, DoctorName = "doctor1" };
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            var appointment = new Appointment { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 };
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _appointmentRepository.GetAppointmentByIdAsync(1L);

            // Assert
            Assert.That(result, Is.EqualTo(appointment));
        }



        [Test]
        public async Task AddAppointmentAsync_ShouldAddAppointment()
        {
            // Arrange
            var appointment = new Appointment { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 };

            // Act
            await _appointmentRepository.AddAppointmentAsync(appointment);

            // Assert
            var addedAppointment = await _context.Appointments.FindAsync(appointment.AppointmentId);
            Assert.That(addedAppointment, Is.EqualTo(appointment));
        }

        [Test]
        public async Task UpdateAppointmentAsync_ShouldUpdateAppointment()
        {
            // Arrange
            var appointment = new Appointment { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 };
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            // Act
            appointment.PatientName = "newpatientname";
            await _appointmentRepository.UpdateAppointmentAsync(appointment);

            // Assert
            var updatedAppointment = await _context.Appointments.FindAsync(appointment.AppointmentId);
            Assert.That(updatedAppointment?.PatientName, Is.EqualTo("newpatientname"));
        }

        [Test]
        public async Task DeleteAppointmentAsync_ShouldDeleteAppointment_WhenAppointmentExists()
        {
            // Arrange
            var appointment = new Appointment { AppointmentId = 1, PatientName = "patient1", PatientContactInformation = "contact1", AppointmentDateAndTime = DateTime.Now, DoctorId = 1 };
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            // Act
            await _appointmentRepository.DeleteAppointmentAsync(1L);

            // Assert
            var deletedAppointment = await _context.Appointments.FindAsync(1L);
            Assert.That(deletedAppointment, Is.Null);
        }
    }
}


