using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeWoController : ControllerBase
    {
        private readonly FeWoService _feWoService;

        public FeWoController(FeWoService feWoService)
        {
            _feWoService = feWoService;
        }

        [HttpGet]
        public ActionResult<List<FeWo>> GetAll()
        {
            return Ok(_feWoService.LesenAlle());
        }

        [HttpGet("active")]
        public ActionResult<List<FeWo>> GetAllActive()
        {
            return Ok(_feWoService.LesenAlleAktive());
        }

        [HttpGet("{id}")]
        public ActionResult<FeWo> Get(long id)
        {
            var feWo = _feWoService.LesenEinzeln(id);
            if (feWo == null)
                return NotFound(new { Message = $"FeWo with id {id} not found." });
            return feWo;
        }

        [HttpPost]
        public ActionResult<long> Create([FromBody] FeWo feWo)
        {
            try
            {
                var id = _feWoService.Erstellen(feWo);
                if (id == 0)
                    return BadRequest(new { Message = "Error creating FeWo." });
                return CreatedAtAction(nameof(Get), new { id = id }, id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] FeWo feWo)
        {
            if (id != feWo.Id)
                return BadRequest(new { Message = "ID mismatch." });

            var updated = _feWoService.Aktualisieren(feWo);
            if (!updated)
                return NotFound(new { Message = $"FeWo with id {id} not found or update failed." });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Deactivate(long id)
        {
            var success = _feWoService.Deaktivieren(id);
            if (!success)
                return NotFound(new { Message = $"FeWo with id {id} not found or already inactive." });
            return NoContent();
        }
    }
}
