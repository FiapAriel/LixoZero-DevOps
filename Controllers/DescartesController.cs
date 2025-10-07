using Microsoft.AspNetCore.Mvc;
using LixoZero.Services;
using LixoZero.ViewModels;

namespace LixoZero.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DescartesController : ControllerBase
    {
        private readonly DescarteService _service;

        public DescartesController(DescarteService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista todos os descartes com paginação
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<DescarteDto>>> GetDescartes([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var descartes = await _service.GetDescartesAsync(page, pageSize);
            return Ok(descartes);
        }

        /// <summary>
        /// Busca um descarte pelo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DescarteDto>> GetDescarteById(int id)
        {
            var descarte = await _service.GetByIdAsync(id);
            if (descarte == null)
                return NotFound();

            return Ok(descarte);
        }

        /// <summary>
        /// Cria um novo descarte
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateDescarte([FromBody] DescarteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var criado = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetDescarteById), new { id = criado.Id }, criado);
        }

        /// <summary>
        /// Exclui um descarte pelo ID
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDescarte(int id)
        {
            var sucesso = await _service.DeleteAsync(id);
            if (!sucesso)
                return NotFound();

            return NoContent();
        }
    }
}
