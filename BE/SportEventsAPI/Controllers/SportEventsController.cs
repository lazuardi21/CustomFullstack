using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportEventsAPI.Data;
using SportEventsAPI.Data.SportEventsAPI.Data;
using SportEventsAPI.Helpers;
using SportEventsAPI.Models;
using System.Threading.Tasks;

namespace SportEventsAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize]
    public class SportEventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SportEventsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<SportEvent>>> GetSportEvents(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.SportEvents.Include(se => se.Organizer).AsQueryable();
            var pagedResponse = PaginationHelper.CreatePagedResponse(query, pageNumber, pageSize);
            return Ok(pagedResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CreateSportEventResponse>> GetSportEvent(int id)
        {
            var sportEvent = await _context.SportEvents
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (sportEvent == null) return NotFound();
            return Ok(sportEvent);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSportEvent([FromBody] CreateSportEventRequest sportEventRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map request to SportEvent entity
            var sportEvent = new SportEvent
            {
                EventName = sportEventRequest.EventName,
                EventType = sportEventRequest.EventType,
                EventDate = sportEventRequest.EventDate,
                OrganizerId = sportEventRequest.OrganizerId  // Assuming organizerId is provided in the request
            };

            // Add to database and save changes
            _context.SportEvents.Add(sportEvent);
            await _context.SaveChangesAsync();

            // Map to response model
            var sportEventResponse = new CreateSportEventResponse
            {
                Id = sportEvent.Id,
                EventName = sportEventRequest.EventName,
                EventType = sportEventRequest.EventType,
                EventDate = sportEventRequest.EventDate,
                OrganizerId = sportEventRequest.OrganizerId  // Assuming organizerId is provided in the request
            };

            // Return the created SportEvent
            return Ok(sportEventResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSportEvent(int id, [FromBody] UpdateSportEventRequest sportEventRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sportEvent = await _context.SportEvents.FindAsync(id);
            if (sportEvent == null)
            {
                return NotFound();
            }

            // Update sportEvent properties
            sportEvent.EventName = sportEventRequest.EventName;
            sportEvent.EventType = sportEventRequest.EventType;
            sportEvent.EventDate = sportEventRequest.EventDate;
            sportEvent.OrganizerId = sportEventRequest.OrganizerId;

            _context.Entry(sportEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SportEventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSportEvent(int id)
        {
            var sportEvent = await _context.SportEvents.FindAsync(id);
            if (sportEvent == null) return NotFound();
            _context.SportEvents.Remove(sportEvent);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool SportEventExists(int id)
        {
            return _context.SportEvents.Any(e => e.Id == id);
        }
    }
}
