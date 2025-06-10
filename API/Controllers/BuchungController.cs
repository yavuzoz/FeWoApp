using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuchungController : ControllerBase
    {
        private readonly BuchungService _buchungService;

        public BuchungController(BuchungService buchungService)
        {
            _buchungService = buchungService;
        }

        [HttpGet]
        public ActionResult<List<Buchung>> GetAll()
        {
            return Ok(_buchungService.LesenAlle());
        }

        [HttpGet("active")]
        public ActionResult<List<Buchung>> GetAllActive()
        {
            return Ok(_buchungService.LesenAlleAktive());
        }

        [HttpGet("{id}")]
        public ActionResult<Buchung> Get(long id)
        {
            var buchung = _buchungService.LesenEinzeln(id);
            if (buchung == null)
                return NotFound(new { Message = $"Buchung with id {id} not found." });
            return buchung;
        }

        [HttpPost]
        public ActionResult<long> Create([FromBody] Buchung buchung)
        {
            try
            {
                var id = _buchungService.Erstellen(buchung);
                if (id == 0)
                    return BadRequest(new { Message = "Error creating Buchung. (Collision or other error)" });
                return CreatedAtAction(nameof(Get), new { id = id }, id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Buchung buchung)
        {
            if (id != buchung.Id)
                return BadRequest(new { Message = "ID mismatch." });

            var updated = _buchungService.Aktualisieren(buchung);
            if (!updated)
                return BadRequest(new { Message = "Update failed, possibly due to collision or not found." });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Deactivate(long id)
        {
            var success = _buchungService.Deaktivieren(id);
            if (!success)
                return NotFound(new { Message = $"Buchung with id {id} not found or already inactive." });
            return NoContent();
        }
    }
}
