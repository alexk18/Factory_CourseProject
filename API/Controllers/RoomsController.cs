using API.DAL.EF;
using API.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Data;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public RoomsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            return await _context.Rooms
                .ToListAsync();
        }

        [HttpGet("all/visible")]
        [Authorize(Roles = "Worker")]
        public async Task<ActionResult<IEnumerable<Room>>> GetVisibleRooms()
        {
            return await _context.Rooms.Where(x => x.IsVisible)
                .ToListAsync();
        }

        [HttpGet("{id}/stats")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Room>>> GetRoomStats(Guid id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                room.Id,
                room.Type,
                room.Number,
                Stats = await _context.Sensors.Where(x => x.RoomId == id)
                .Select(x => new
                {
                    x.Values.OrderByDescending(x => x.Created).First().Value,
                    x.ValueType,
                    x.Values.OrderByDescending(x => x.Created).First().Created
                })
                .ToListAsync()

            });
        }

        [HttpPut("edit/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutRoom(Guid id, Room model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            if (await _context.Rooms.AnyAsync(x => x.Number == model.Number && x.Id != id))
            {
                return BadRequest("This number is used");
            }

            room.Number = model.Number;
            room.Type = model.Type;
            room.IsVisible = model.IsVisible;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!RoomExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("visit/{id}")]
        [Authorize(Roles = "Worker")]
        public async Task<ActionResult<Visit>> VisitRoom(Guid id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null || !room.IsVisible)
            {
                return NotFound();
            }

            var visit = new Visit { RoomId = id, UserId = Guid.Parse(User.Identity!.Name!), Created = DateTime.UtcNow };

            _context.Visits.Add(visit);
            await _context.SaveChangesAsync();

            return Ok(visit);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Room>> PostRoom(Room model)
        {
            if (await _context.Rooms.AnyAsync(x => x.Number == model.Number))
            {
                return BadRequest("This number is used");
            }

            _context.Rooms.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoom(Guid id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomExists(Guid id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
}
