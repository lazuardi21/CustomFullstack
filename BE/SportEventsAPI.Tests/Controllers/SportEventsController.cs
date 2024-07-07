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
    public class SportEventsControllerTests
    {
        private SportEventsController _controller;
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            // Use an in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "test_sportevents_db")
                .Options;
            _context = new ApplicationDbContext(options);

            // Seed test data
            _context.SportEvents.Add(new SportEvent { Id = 1, EventName = "Test Event", EventType = "Test Type", EventDate = DateTime.UtcNow });
            _context.SaveChanges();

            _controller = new SportEventsController(_context, null); // You may mock IMapper if needed
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up test data after each test
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task GetSportEvents_ReturnsAllSportEvents()
        {
            // Act
            var result = await _controller.GetSportEvents();

            // Assert
            Assert.IsInstanceOf<ActionResult<PagedResponse<SportEvent>>>(result);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var pagedResponse = okResult.Value as PagedResponse<SportEvent>;
            Assert.IsNotNull(pagedResponse);

            Assert.AreEqual(1, pagedResponse.Data.Count()); // Assuming Data is the property holding the list of sport events
        }

        [Test]
        public async Task GetSportEvent_ValidId_ReturnsSportEvent()
        {
            // Arrange
            var sportEventId = 1;

            // Act
            var result = await _controller.GetSportEvent(sportEventId);

            // Assert
            Assert.IsInstanceOf<ActionResult<SportEvent>>(result);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var sportEvent = okResult.Value as SportEvent;
            Assert.IsNotNull(sportEvent);

            Assert.AreEqual(sportEventId, sportEvent.Id);
        }

        [Test]
        public async Task CreateSportEvent_ValidData_CreatesSportEvent()
        {
            // Arrange
            var sportEventRequest = new CreateSportEventRequest
            {
                EventName = "New Event",
                EventType = "New Type",
                EventDate = DateTime.UtcNow,
                OrganizerId = 1 // Assuming OrganizerId exists in the test context
            };

            // Act
            var result = await _controller.CreateSportEvent(sportEventRequest) as OkObjectResult;
            var sportEventResponse = result.Value as CreateSportEventResponse;

            // Assert
            Assert.AreEqual(200, result.StatusCode); // Assuming you return Ok with sportEventResponse
            Assert.AreEqual(sportEventRequest.EventName, sportEventResponse.EventName);
            Assert.AreEqual(sportEventRequest.EventType, sportEventResponse.EventType);
            Assert.AreEqual(sportEventRequest.EventDate, sportEventResponse.EventDate);
            Assert.AreEqual(sportEventRequest.OrganizerId, sportEventResponse.OrganizerId);
        }

        [Test]
        public async Task DeleteSportEvent_ValidId_DeletesSportEvent()
        {
            // Arrange
            var sportEventId = 1;

            // Act
            var result = await _controller.DeleteSportEvent(sportEventId) as NoContentResult;

            // Assert
            Assert.AreEqual(204, result.StatusCode); // Assuming you return NoContent
            Assert.IsFalse(_context.SportEvents.Any(se => se.Id == sportEventId));
        }
    }
}
