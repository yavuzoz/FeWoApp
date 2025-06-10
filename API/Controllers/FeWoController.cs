using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// API-Controller für CRUD-Operationen auf Ferienwohnungen (FeWo).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FeWoController : ControllerBase
    {
        private readonly FeWoService _feWoService;

        /// <summary>
        /// Erstellt eine neue Instanz des FeWoControllers.
        /// </summary>
        /// <param name="feWoService">Service für FeWo-Operationen</param>
        public FeWoController(FeWoService feWoService)
        {
            _feWoService = feWoService;
        }

        /// <summary>
        /// Gibt eine Liste aller Ferienwohnungen zurück.
        /// </summary>
        /// <returns>Liste von FeWo-Objekten</returns>
        [HttpGet]
        public ActionResult<List<FeWo>> GetAll()
        {
            return Ok(_feWoService.LesenAlle());
        }

        /// <summary>
        /// Gibt eine Liste aller aktiven Ferienwohnungen zurück.
        /// </summary>
        /// <returns>Liste von aktiven FeWo-Objekten</returns>
        [HttpGet("active")]
        public ActionResult<List<FeWo>> GetAllActive()
        {
            return Ok(_feWoService.LesenAlleAktive());
        }

        /// <summary>
        /// Gibt eine einzelne Ferienwohnung anhand ihrer Id zurück.
        /// </summary>
        /// <param name="id">Id der gesuchten Ferienwohnung</param>
        /// <returns>FeWo-Objekt oder NotFound</returns>
        [HttpGet("{id}")]
        public ActionResult<FeWo> Get(long id)
        {
            var feWo = _feWoService.LesenEinzeln(id);
            if (feWo == null)
                return NotFound(new { Message = $"FeWo mit Id {id} wurde nicht gefunden." });
            return feWo;
        }

        /// <summary>
        /// Erstellt eine neue Ferienwohnung.
        /// </summary>
        /// <param name="feWo">Das zu erstellende FeWo-Objekt</param>
        /// <returns>Id der erstellten Ferienwohnung</returns>
        [HttpPost]
        public ActionResult<long> Create([FromBody] FeWo feWo)
        {
            try
            {
                var id = _feWoService.Erstellen(feWo);
                if (id == 0)
                    return BadRequest(new { Message = "Fehler beim Erstellen der FeWo." });
                return CreatedAtAction(nameof(Get), new { id = id }, id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Aktualisiert eine bestehende Ferienwohnung.
        /// </summary>
        /// <param name="id">Id der zu aktualisierenden FeWo</param>
        /// <param name="feWo">Neue Daten der FeWo</param>
        /// <returns>NoContent oder Fehlernachricht</returns>
        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] FeWo feWo)
        {
            if (id != feWo.Id)
                return BadRequest(new { Message = "Id stimmt nicht überein." });

            var updated = _feWoService.Aktualisieren(feWo);
            if (!updated)
                return NotFound(new { Message = $"FeWo mit Id {id} nicht gefunden oder Update fehlgeschlagen." });
            return NoContent();
        }

        /// <summary>
        /// Setzt eine Ferienwohnung auf inaktiv (Soft Delete).
        /// </summary>
        /// <param name="id">Id der zu deaktivierenden FeWo</param>
        /// <returns>NoContent oder Fehlernachricht</returns>
        [HttpDelete("{id}")]
        public IActionResult Deactivate(long id)
        {
            var success = _feWoService.Deaktivieren(id);
            if (!success)
                return NotFound(new { Message = $"FeWo mit Id {id} nicht gefunden oder bereits inaktiv." });
            return NoContent();
        }
    }
}