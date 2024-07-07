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
    public class OrganizersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrganizersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<Organizer>>> GetOrganizers(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Organizers.AsQueryable();
            var pagedResponse = PaginationHelper.CreatePagedResponse(query, pageNumber, pageSize);
            return Ok(pagedResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CreateOrganizerResponse>> GetOrganizer(int id)
        {
            var organizer = await _context.Organizers.FindAsync(id);
            if (organizer == null) return NotFound();
            return Ok(organizer);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrganizer([FromBody] CreateOrganizerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var organizer = new Organizer
            {
                OrganizerName = request.OrganizerName,
                ImageLocation = request.ImageLocation
            };

            _context.Organizers.Add(organizer);
            await _context.SaveChangesAsync();

            var organizerResponse = new Organizer
            {
                Id = organizer.Id,
                OrganizerName = request.OrganizerName,
                ImageLocation = request.ImageLocation
            };

            return Ok(organizerResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrganizer(int id, UpdateOrganizerRequest organizerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var organizer = await _context.Organizers.FindAsync(id);
            if (organizer == null)
            {
                return NotFound();
            }

            // Update organizer properties
            organizer.OrganizerName = organizerRequest.OrganizerName;
            organizer.ImageLocation = organizerRequest.ImageLocation;

            _context.Entry(organizer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrganizerExists(id))
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
        public async Task<IActionResult> DeleteOrganizer(int id)
        {
            var organizer = await _context.Organizers.FindAsync(id);
            if (organizer == null) return NotFound();
            _context.Organizers.Remove(organizer);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool OrganizerExists(int id)
        {
            return _context.Organizers.Any(e => e.Id == id);
        }
    }
}
