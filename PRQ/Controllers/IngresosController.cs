using Microsoft.AspNetCore.Mvc;
using PRQ.Interfaces;
using PRQ.Models;

namespace PRQ.Controllers;

[ApiController]
[Route("api/ingresos")]
public class IngresosController : ControllerBase
{
    private readonly IIngresoRepository _repo;

    public IngresosController(IIngresoRepository repo)
    {
        _repo = repo;
    }

    // ── GET /api/ingresos ─────────────────────────────────────────────────
    [HttpGet]
    public ActionResult<IEnumerable<Ingreso>> GetAll()
        => Ok(_repo.GetAll());

    // ── GET /api/ingresos/{id} ────────────────────────────────────────────
    [HttpGet("{id:int}")]
    public ActionResult<Ingreso> GetById(int id)
    {
        var ingreso = _repo.GetById(id);
        return ingreso is null ? NotFound() : Ok(ingreso);
    }

    // ── POST /api/ingresos ────────────────────────────────────────────────
    [HttpPost]
    public ActionResult<Ingreso> Insert([FromBody] Ingreso ingreso)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _repo.Insert(ingreso);
        return CreatedAtAction(nameof(GetById), new { id = ingreso.Consecutivo }, ingreso);
    }

    // ── PUT /api/ingresos/{id} ────────────────────────────────────────────
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Ingreso ingreso)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != ingreso.Consecutivo)
            return BadRequest("Route id does not match body Consecutivo.");

        if (_repo.GetById(id) is null)
            return NotFound();

        _repo.Update(ingreso);
        return NoContent();
    }

    // ── DELETE /api/ingresos/{id} ─────────────────────────────────────────
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (_repo.GetById(id) is null)
            return NotFound();

        _repo.Delete(id);
        return NoContent();
    }

    // ── GET /api/ingresos/query/tipo?value={}&start={}&end={} ─────────────
    [HttpGet("query/tipo")]
    public ActionResult<IEnumerable<Ingreso>> GetByTipo(
        [FromQuery] string value,
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        if (string.IsNullOrWhiteSpace(value))
            return BadRequest("Query parameter 'value' is required.");

        if (start > end)
            return BadRequest("'start' must be less than or equal to 'end'.");

        return Ok(_repo.GetByTipoAndDateRange(value, start, end));
    }

    // ── GET /api/ingresos/query/provincia?value={}&start={}&end={} ────────
    [HttpGet("query/provincia")]
    public ActionResult<IEnumerable<Ingreso>> GetByProvincia(
        [FromQuery] string value,
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        if (string.IsNullOrWhiteSpace(value))
            return BadRequest("Query parameter 'value' is required.");

        if (start > end)
            return BadRequest("'start' must be less than or equal to 'end'.");

        return Ok(_repo.GetByProvinciaAndDateRange(value, start, end));
    }
}
