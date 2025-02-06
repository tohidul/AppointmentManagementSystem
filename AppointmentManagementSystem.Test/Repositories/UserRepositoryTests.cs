using NUnit.Framework;
using Moq;
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
    internal class UserRepositoryTests
    {
        private AppointmentManagementSystemDbContext _context = null!;
        private UserRepository _userRepository = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppointmentManagementSystemDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new AppointmentManagementSystemDbContext(options);
            _userRepository = new UserRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserId = 1, Username = "user1", Password="test" },
                new User { UserId = 2, Username = "user2", Password="test" }
            };
            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetAllUsersAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User { UserId = 1, Username = "user1", Password = "test" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByIdAsync(1);

            // Assert
            Assert.That(result, Is.EqualTo(user));
        }

        [Test]
        public async Task GetUserByUsernameAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User { UserId = 1, Username = "user1", Password = "test" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByUsernameAsync("user1");

            // Assert
            Assert.That(result, Is.EqualTo(user));
        }

        [Test]
        public async Task AddUserAsync_ShouldAddUser()
        {
            // Arrange
            var user = new User { UserId = 1, Username = "user1", Password = "test" };

            // Act
            await _userRepository.AddUserAsync(user);

            // Assert
            var addedUser = await _context.Users.FindAsync(user.UserId);
            Assert.That(addedUser, Is.EqualTo(user));
        }

        [Test]
        public async Task UpdateUserAsync_ShouldUpdateUser()
        {
            // Arrange
            var user = new User { UserId = 1, Username = "user1", Password = "test" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            user.Password = "newpassword";
            await _userRepository.UpdateUserAsync(user);

            // Assert
            var updatedUser = await _context.Users.FindAsync(user.UserId);
            Assert.That(updatedUser?.Password, Is.EqualTo("newpassword"));
        }

        [Test]
        public async Task DeleteUserAsync_ShouldDeleteUser_WhenUserExists()
        {
            // Arrange
            var user = new User { UserId = 1, Username = "user1", Password = "test" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            await _userRepository.DeleteUserAsync(1);

            // Assert
            var deletedUser = await _context.Users.FindAsync(1L);
            Assert.That(deletedUser, Is.Null);
        }
    }
}
