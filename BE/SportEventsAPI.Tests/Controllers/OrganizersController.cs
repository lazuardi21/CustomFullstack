using NUnit.Framework;
using SportEventsAPI.Controllers;
using SportEventsAPI.Data;
using SportEventsAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SportEventsAPI.Data.SportEventsAPI.Data;

namespace SportEventsAPI.Tests.Controllers
{
    [TestFixture]
    public class OrganizersControllerTests
    {
        private OrganizersController _controller;
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            // Use an in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "test_organizers_db")
                .Options;
            _context = new ApplicationDbContext(options);

            // Seed test data
            _context.Organizers.Add(new Organizer { Id = 1, OrganizerName = "Test Organizer", ImageLocation = "/images/test.jpg" });
            _context.SaveChanges();

            _controller = new OrganizersController(_context, null); // You may mock IMapper if needed
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up test data after each test
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task GetOrganizers_ReturnsAllOrganizers()
        {
            // Act
            var result = await _controller.GetOrganizers();

            // Assert
            Assert.IsInstanceOf<ActionResult<PagedResponse<Organizer>>>(result);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var pagedResponse = okResult.Value as PagedResponse<Organizer>;
            Assert.IsNotNull(pagedResponse);

            Assert.AreEqual(1, pagedResponse.Data.Count()); // Assuming Data is the property holding the list of organizers
        }

        [Test]
        public async Task GetOrganizer_ValidId_ReturnsOrganizer()
        {
            // Arrange
            var organizerId = 1;

            // Act
            var actionResult = await _controller.GetOrganizer(organizerId);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<ActionResult<Organizer>>(actionResult);

            var okObjectResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);

            var organizer = okObjectResult.Value as Organizer;
            Assert.IsNotNull(organizer);
            Assert.AreEqual(organizerId, organizer.Id);
        }





        [Test]
        public async Task CreateOrganizer_ValidData_CreatesOrganizer()
        {
            // Arrange
            var organizerRequest = new CreateOrganizerRequest
            {
                OrganizerName = "New Organizer",
                ImageLocation = "/images/new.jpg"
            };

            // Act
            var result = await _controller.CreateOrganizer(organizerRequest) as OkObjectResult;
            var organizerResponse = result.Value as Organizer;

            // Assert
            Assert.AreEqual(200, result.StatusCode); // Assuming you return Ok with organizerResponse
            Assert.AreEqual(organizerRequest.OrganizerName, organizerResponse.OrganizerName);
            Assert.AreEqual(organizerRequest.ImageLocation, organizerResponse.ImageLocation);
        }

        [Test]
        public async Task DeleteOrganizer_ValidId_DeletesOrganizer()
        {
            // Arrange
            var organizerId = 1;

            // Act
            var result = await _controller.DeleteOrganizer(organizerId) as NoContentResult;

            // Assert
            Assert.AreEqual(204, result.StatusCode); // Assuming you return NoContent
            Assert.IsFalse(_context.Organizers.Any(o => o.Id == organizerId));
        }
    }
}
