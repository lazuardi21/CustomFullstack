using NUnit.Framework;
using SportEventsAPI.Controllers;
using SportEventsAPI.Data;
using SportEventsAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SportEventsAPI.Data.SportEventsAPI.Data;
using AutoMapper;
using System.Collections.Generic;
using Moq;
using Microsoft.Extensions.Logging;

namespace SportEventsAPI.Tests.Controllers
{
    [TestFixture]
    public class UsersControllerTests
    {
        private UsersController _controller;
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            // Use an in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "test_users_db")
                .EnableSensitiveDataLogging() // Add this to see detailed entity information
                .Options;
            _context = new ApplicationDbContext(options);

            // Seed test data
            _context.Users.Add(new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@test.com", Password = "password", RepeatPassword = "password" });
            _context.SaveChanges();

            // Mock IMapper
            var mockMapper = new Mock<IMapper>();

            // Example: Setup mapping for UserRequest to UserResponse
            mockMapper.Setup(m => m.Map<UserResponse>(It.IsAny<User>()))
                .Returns((User u) => new UserResponse { Id = u.Id, FirstName = u.FirstName, LastName = u.LastName, Email = u.Email });

            // Mock ILogger
            var mockLogger = new Mock<ILogger<UsersController>>();

            _controller = new UsersController(_context, null, mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up test data after each test
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task GetUsers_ReturnsAllUsers()
        {
            // Act
            var result = await _controller.GetUsers();

            // Assert
            Assert.IsInstanceOf<ActionResult<IEnumerable<UserResponse>>>(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var users = okResult.Value as IEnumerable<UserResponse>;
            Assert.IsNotNull(users);
            Assert.AreEqual(1, users.Count());
        }

        [Test]
        public async Task GetUser_ValidId_ReturnsUser()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            Assert.IsInstanceOf<ActionResult<UserResponse>>(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var user = okResult.Value as UserResponse;
            Assert.IsNotNull(user);
            Assert.AreEqual(userId, user.Id);
        }

        [Test]
        public async Task PostUser_ValidData_CreatesUser()
        {
            // Arrange
            var userRequest = new UserRequest
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@test.com",
                Password = "password",
                RepeatPassword = "password"
            };

            // Act
            var result = await _controller.PostUser(userRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CreatedAtActionResult>(result);

            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);

            var userResponse = createdResult.Value as UserResponse;
            Assert.IsNotNull(userResponse);
            Assert.AreEqual(userRequest.FirstName, userResponse.FirstName);
            Assert.AreEqual(userRequest.LastName, userResponse.LastName);
            Assert.AreEqual(userRequest.Email, userResponse.Email);
        }


        [Test]
        public async Task DeleteUser_ValidId_DeletesUser()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = await _controller.DeleteUser(userId) as NoContentResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(204, result.StatusCode); // Assuming you return NoContent
            Assert.IsFalse(_context.Users.Any(u => u.Id == userId));
        }
    }
}
