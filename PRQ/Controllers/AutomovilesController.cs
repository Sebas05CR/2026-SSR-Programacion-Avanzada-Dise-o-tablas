using Microsoft.AspNetCore.Mvc;
using PRQ.Interfaces;
using PRQ.Models;

namespace PRQ.Controllers;

[ApiController]
[Route("api/automoviles")]
public class AutomovilesController : ControllerBase
{
    private readonly IAutomovilRepository _repo;

    public AutomovilesController(IAutomovilRepository repo)
    {
        _repo = repo;
    }

    // ── GET /api/automoviles ──────────────────────────────────────────────
    [HttpGet]
    public ActionResult<IEnumerable<Automovil>> GetAll()
        => Ok(_repo.GetAll());

    // ── GET /api/automoviles/{id} ─────────────────────────────────────────
    [HttpGet("{id:int}")]
    public ActionResult<Automovil> GetById(int id)
    {
        var automovil = _repo.GetById(id);
        return automovil is null ? NotFound() : Ok(automovil);
    }

    // ── POST /api/automoviles ─────────────────────────────────────────────
    [HttpPost]
    public ActionResult<Automovil> Insert([FromBody] Automovil automovil)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _repo.Insert(automovil);
        return CreatedAtAction(nameof(GetById), new { id = automovil.Id }, automovil);
    }

    // ── PUT /api/automoviles/{id} ─────────────────────────────────────────
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Automovil automovil)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != automovil.Id)
            return BadRequest("Route id does not match body Id.");

        if (_repo.GetById(id) is null)
            return NotFound();

        _repo.Update(automovil);
        return NoContent();
    }

    // ── DELETE /api/automoviles/{id} ──────────────────────────────────────
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (_repo.GetById(id) is null)
            return NotFound();

        _repo.Delete(id);
        return NoContent();
    }

    // ── GET /api/automoviles/filter/color?value={color} ───────────────────
    [HttpGet("filter/color")]
    public ActionResult<IEnumerable<Automovil>> GetByColor([FromQuery] string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return BadRequest("Query parameter 'value' is required.");

        return Ok(_repo.GetByColor(value));
    }

    // ── GET /api/automoviles/filter/year-range?start={}&end={} ───────────
    [HttpGet("filter/year-range")]
    public ActionResult<IEnumerable<Automovil>> GetByYearRange(
        [FromQuery] int start, [FromQuery] int end)
    {
        if (start > end)
            return BadRequest("'start' must be less than or equal to 'end'.");

        return Ok(_repo.GetByYearRange(start, end));
    }

    // ── GET /api/automoviles/filter/fabricante?value={fabricante} ─────────
    [HttpGet("filter/fabricante")]
    public ActionResult<IEnumerable<Automovil>> GetByFabricante([FromQuery] string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return BadRequest("Query parameter 'value' is required.");

        return Ok(_repo.GetByFabricante(value));
    }

    // ── GET /api/automoviles/filter/tipo?value={tipo} ─────────────────────
    [HttpGet("filter/tipo")]
    public ActionResult<IEnumerable<Automovil>> GetByTipo([FromQuery] string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return BadRequest("Query parameter 'value' is required.");

        return Ok(_repo.GetByTipo(value));
    }
}
