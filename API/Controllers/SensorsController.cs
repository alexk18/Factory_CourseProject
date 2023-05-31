using API.DAL.EF;
using API.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public SensorsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("{roomId}/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRoomSensors(Guid roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);

            if (room == null)
            {
                return NotFound("No room found");
            }

            return Ok(new
            {
                room.Id,
                room.Type,
                room.Number,
                room.IsVisible,
                Sensors = await _context.Sensors.Where(x => x.RoomId == roomId).ToListAsync()
            });
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Sensor>> PostSensor(Sensor model)
        {
            if(!(model.Min < model.Max))
            {
                return BadRequest("Max smaller than Min");
            }

            _context.Sensors.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        [HttpPut("edit/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutSensor(Guid id, Sensor model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var sensor = await _context.Sensors.FindAsync(id);
            if (sensor == null)
            {
                return NotFound();
            }

            if (!(model.Min < model.Max))
            {
                return BadRequest("Max smaller than Min");
            }

            sensor.ValueType = model.ValueType;
            sensor.Min = model.Min;
            sensor.Max = model.Max;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!SensorExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSensor(Guid id)
        {
            var sensor = await _context.Sensors.FindAsync(id);
            if (sensor == null)
            {
                return NotFound();
            }

            _context.Sensors.Remove(sensor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SensorExists(Guid id)
        {
            return _context.Sensors.Any(e => e.Id == id);
        }
    }
}
