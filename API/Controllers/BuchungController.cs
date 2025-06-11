using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// API-Controller für CRUD-Operationen auf Buchungen.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BuchungController : ControllerBase
    {
        private readonly IService<Buchung> _buchungService;

        public BuchungController(IService<Buchung> buchungService)
        {
            _buchungService = buchungService;
        }

        /// <summary>
        /// Gibt eine Liste aller Buchungen zurück.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Buchung>), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public ActionResult<List<Buchung>> GetAll()
        {
            try
            {
                return Ok(_buchungService.LesenAlle());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Gibt eine Liste aller aktiven Buchungen zurück.
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(List<Buchung>), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public ActionResult<List<Buchung>> GetAllActive()
        {
            try
            {
                return Ok(_buchungService.LesenAlleAktive());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Gibt eine einzelne Buchung anhand ihrer Id zurück.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Buchung), 200)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public ActionResult<Buchung> Get(long id)
        {
            try
            {
                var buchung = _buchungService.LesenEinzeln(id);
                if (buchung == null)
                    return NotFound(new { Message = $"Buchung mit Id {id} wurde nicht gefunden." });
                return Ok(buchung);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Erstellt eine neue Buchung.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(long), 201)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public ActionResult<long> Create([FromBody] Buchung buchung)
        {
            try
            {
                var id = _buchungService.Erstellen(buchung);
                if (id == 0)
                    return BadRequest(new { Message = "Fehler beim Erstellen der Buchung (Kollision oder anderer Fehler)." });
                return CreatedAtAction(nameof(Get), new { id = id }, id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Aktualisiert eine bestehende Buchung.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public IActionResult Update(long id, [FromBody] Buchung buchung)
        {
            try
            {
                if (id != buchung.Id)
                    return BadRequest(new { Message = "Id stimmt nicht überein." });

                var updated = _buchungService.Aktualisieren(buchung);
                if (!updated)
                    return BadRequest(new { Message = "Aktualisierung fehlgeschlagen, möglicherweise wegen Kollision oder nicht gefunden." });
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Setzt eine Buchung auf inaktiv (Soft Delete).
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public IActionResult Deactivate(long id)
        {
            try
            {
                var success = _buchungService.Deaktivieren(id);
                if (!success)
                    return NotFound(new { Message = $"Buchung mit Id {id} nicht gefunden oder bereits inaktiv." });
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}