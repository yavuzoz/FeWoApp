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

        /// <summary>
        /// Erstellt eine neue Instanz des BuchungControllers.
        /// </summary>
        /// <param name="buchungService"></param>
        public BuchungController(IService<Buchung> buchungService)
        {
            _buchungService = buchungService;
        }

        /// <summary>
        /// Gibt eine Liste aller Buchungen zurück.
        /// </summary>
        /// <returns>Liste von Buchung-Objekten</returns>
        [HttpGet]
        public ActionResult<List<Buchung>> GetAll()
        {
            return Ok(_buchungService.LesenAlle());
        }

        /// <summary>
        /// Gibt eine Liste aller aktiven Buchungen zurück.
        /// </summary>
        /// <returns>Liste von aktiven Buchungen</returns>
        [HttpGet("active")]
        public ActionResult<List<Buchung>> GetAllActive()
        {
            return Ok(_buchungService.LesenAlleAktive());
        }

        /// <summary>
        /// Gibt eine einzelne Buchung anhand ihrer Id zurück.
        /// </summary>
        /// <param name="id">Id der gesuchten Buchung</param>
        /// <returns>Buchung-Objekt oder NotFound</returns>
        [HttpGet("{id}")]
        public ActionResult<Buchung> Get(long id)
        {
            var buchung = _buchungService.LesenEinzeln(id);
            if (buchung == null)
                return NotFound(new { Message = $"Buchung mit Id {id} wurde nicht gefunden." });
            return buchung;
        }

        /// <summary>
        /// Erstellt eine neue Buchung.
        /// </summary>
        /// <param name="buchung">Das zu erstellende Buchung-Objekt</param>
        /// <returns>Id der erstellten Buchung</returns>
        [HttpPost]
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
        /// <param name="id">Id der zu aktualisierenden Buchung</param>
        /// <param name="buchung">Neue Daten der Buchung</param>
        /// <returns>NoContent oder Fehlernachricht</returns>
        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Buchung buchung)
        {
            if (id != buchung.Id)
                return BadRequest(new { Message = "Id stimmt nicht überein." });

            var updated = _buchungService.Aktualisieren(buchung);
            if (!updated)
                return BadRequest(new { Message = "Aktualisierung fehlgeschlagen, möglicherweise wegen Kollision oder nicht gefunden." });
            return NoContent();
        }

        /// <summary>
        /// Setzt eine Buchung auf inaktiv (Soft Delete).
        /// </summary>
        /// <param name="id">Id der zu deaktivierenden Buchung</param>
        /// <returns>NoContent oder Fehlernachricht</returns>
        [HttpDelete("{id}")]
        public IActionResult Deactivate(long id)
        {
            var success = _buchungService.Deaktivieren(id);
            if (!success)
                return NotFound(new { Message = $"Buchung mit Id {id} nicht gefunden oder bereits inaktiv." });
            return NoContent();
        }
    }
}