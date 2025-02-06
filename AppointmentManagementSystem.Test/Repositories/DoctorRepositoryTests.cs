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
    internal class DoctorRepositoryTests
    {
        private AppointmentManagementSystemDbContext _context = null!;
        private DoctorRepository _doctorRepository = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppointmentManagementSystemDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new AppointmentManagementSystemDbContext(options);
            _doctorRepository = new DoctorRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
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
            await _context.Doctors.AddRangeAsync(doctors);
            await _context.SaveChangesAsync();

            // Act
            var result = await _doctorRepository.GetAllDoctorsAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetDoctorByIdAsync_ShouldReturnDoctor_WhenDoctorExists()
        {
            // Arrange
            var doctor = new Doctor { DoctorId = 1, DoctorName = "doctor1" };
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            // Act
            var result = await _doctorRepository.GetDoctorByIdAsync(1L);

            // Assert
            Assert.That(result, Is.EqualTo(doctor));
        }

        [Test]
        public async Task AddDoctorAsync_ShouldAddDoctor()
        {
            // Arrange
            var doctor = new Doctor { DoctorId = 1, DoctorName = "doctor1" };

            // Act
            await _doctorRepository.AddDoctorAsync(doctor);

            // Assert
            var addedDoctor = await _context.Doctors.FindAsync(doctor.DoctorId);
            Assert.That(addedDoctor, Is.EqualTo(doctor));
        }

        [Test]
        public async Task UpdateDoctorAsync_ShouldUpdateDoctor()
        {
            // Arrange
            var doctor = new Doctor { DoctorId = 1, DoctorName = "doctor1" };
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            // Act
            doctor.DoctorName = "newdoctorname";
            await _doctorRepository.UpdateDoctorAsync(doctor);

            // Assert
            var updatedDoctor = await _context.Doctors.FindAsync(doctor.DoctorId);
            Assert.That(updatedDoctor?.DoctorName, Is.EqualTo("newdoctorname"));
        }

        [Test]
        public async Task DeleteDoctorAsync_ShouldDeleteDoctor_WhenDoctorExists()
        {
            // Arrange
            var doctor = new Doctor { DoctorId = 1, DoctorName = "doctor1" };
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            // Act
            await _doctorRepository.DeleteDoctorAsync(1L);

            // Assert
            var deletedDoctor = await _context.Doctors.FindAsync(1L);
            Assert.That(deletedDoctor, Is.Null);
        }
    }
}

