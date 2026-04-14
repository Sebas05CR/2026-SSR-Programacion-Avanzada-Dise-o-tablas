using Microsoft.AspNetCore.Mvc;
using PRQ.Interfaces;
using PRQ.Models;

namespace PRQ.Controllers;

[ApiController]
[Route("api/parqueos")]
public class ParqueosController : ControllerBase
{
    private readonly IParqueoRepository _repo;

    public ParqueosController(IParqueoRepository repo)
    {
        _repo = repo;
    }

    // ── GET /api/parqueos ─────────────────────────────────────────────────
    [HttpGet]
    public ActionResult<IEnumerable<Parqueo>> GetAll()
        => Ok(_repo.GetAll());

    // ── GET /api/parqueos/{id} ────────────────────────────────────────────
    [HttpGet("{id:int}")]
    public ActionResult<Parqueo> GetById(int id)
    {
        var parqueo = _repo.GetById(id);
        return parqueo is null ? NotFound() : Ok(parqueo);
    }

    // ── POST /api/parqueos ────────────────────────────────────────────────
    [HttpPost]
    public ActionResult<Parqueo> Insert([FromBody] Parqueo parqueo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _repo.Insert(parqueo);
        return CreatedAtAction(nameof(GetById), new { id = parqueo.Id }, parqueo);
    }

    // ── PUT /api/parqueos/{id} ────────────────────────────────────────────
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Parqueo parqueo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != parqueo.Id)
            return BadRequest("Route id does not match body Id.");

        if (_repo.GetById(id) is null)
            return NotFound();

        _repo.Update(parqueo);
        return NoContent();
    }

    // ── DELETE /api/parqueos/{id} ─────────────────────────────────────────
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (_repo.GetById(id) is null)
            return NotFound();

        _repo.Delete(id);
        return NoContent();
    }

    // ── GET /api/parqueos/filter/provincia?value={provincia} ──────────────
    [HttpGet("filter/provincia")]
    public ActionResult<IEnumerable<Parqueo>> GetByProvincia([FromQuery] string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return BadRequest("Query parameter 'value' is required.");

        return Ok(_repo.GetByProvincia(value));
    }

    // ── GET /api/parqueos/filter/nombre?value={nombre} ────────────────────
    [HttpGet("filter/nombre")]
    public ActionResult<IEnumerable<Parqueo>> GetByNombre([FromQuery] string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return BadRequest("Query parameter 'value' is required.");

        return Ok(_repo.GetByNombre(value));
    }

    // ── GET /api/parqueos/filter/price-range?min={}&max={} ────────────────
    [HttpGet("filter/price-range")]
    public ActionResult<IEnumerable<Parqueo>> GetByPriceRange(
        [FromQuery] decimal min, [FromQuery] decimal max)
    {
        if (min < 0 || max < 0)
            return BadRequest("'min' and 'max' must be non-negative.");

        if (min > max)
            return BadRequest("'min' must be less than or equal to 'max'.");

        return Ok(_repo.GetByPriceRange(min, max));
    }

    // ── GET /api/parqueos/{id}/precio-por-hora ────────────────────────────
    [HttpGet("{id:int}/precio-por-hora")]
    public ActionResult<decimal> GetPrecioPorHora(int id)
    {
        if (_repo.GetById(id) is null)
            return NotFound();

        return Ok(_repo.ObtenerPrecioPorHoraPorParqueo(id));
    }
}
