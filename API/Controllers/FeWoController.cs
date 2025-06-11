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
        private readonly IService<FeWo> _feWoService;

        public FeWoController(IService<FeWo> feWoService)
        {
            _feWoService = feWoService;
        }

        /// <summary>
        /// Gibt eine Liste aller Ferienwohnungen zurück.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<FeWo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public ActionResult<List<FeWo>> GetAll()
        {
            try
            {
                return Ok(_feWoService.LesenAlle());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Gibt eine Liste aller aktiven Ferienwohnungen zurück.
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(List<FeWo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public ActionResult<List<FeWo>> GetAllActive()
        {
            try
            {
                return Ok(_feWoService.LesenAlleAktive());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Gibt eine einzelne Ferienwohnung anhand ihrer Id zurück.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FeWo), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public ActionResult<FeWo> Get(long id)
        {
            try
            {
                var feWo = _feWoService.LesenEinzeln(id);
                if (feWo == null)
                    return NotFound(new { Message = $"FeWo mit Id {id} wurde nicht gefunden." });
                return Ok(feWo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Erstellt eine neue Ferienwohnung.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
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
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public IActionResult Update(long id, [FromBody] FeWo feWo)
        {
            try
            {
                if (id != feWo.Id)
                    return BadRequest(new { Message = "Id stimmt nicht überein." });

                var updated = _feWoService.Aktualisieren(feWo);
                if (!updated)
                    return NotFound(new { Message = $"FeWo mit Id {id} nicht gefunden oder Update fehlgeschlagen." });
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Setzt eine Ferienwohnung auf inaktiv (Soft Delete).
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public IActionResult Deactivate(long id)
        {
            try
            {
                var success = _feWoService.Deaktivieren(id);
                if (!success)
                    return NotFound(new { Message = $"FeWo mit Id {id} nicht gefunden oder bereits inaktiv." });
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}