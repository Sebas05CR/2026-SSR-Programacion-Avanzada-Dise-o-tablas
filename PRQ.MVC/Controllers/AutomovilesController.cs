using System.Net;
using Microsoft.AspNetCore.Mvc;
using PRQ.MVC.Models;
using PRQ.MVC.Services;

namespace PRQ.MVC.Controllers;

public class AutomovilesController(IApiService apiService) : Controller
{
    [ApiSourceFeature("AutomovilesMaestroDetalle")]
    [HttpGet]
    public IActionResult MaestroDetalle()
    {
        return View();
    }

    [ApiSourceFeature("AutomovilesMaestroDetalle")]
    [HttpGet]
    public async Task<IActionResult> BuscarPorTipo(string tipo)
    {
        if (string.IsNullOrWhiteSpace(tipo))
        {
            return BadRequest(new { message = "Debe seleccionar un tipo." });
        }

        try
        {
            var automoviles = await apiService.GetAsync<List<AutomovilViewModel>>(
                $"automoviles/filter/tipo?value={Uri.EscapeDataString(tipo)}") ?? [];

            return Json(automoviles);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode((int?)ex.StatusCode ?? 500, new { message = ex.Message });
        }
    }

    [ApiSourceFeature("AutomovilesMaestroDetalle")]
    [HttpGet]
    public async Task<IActionResult> BuscarIngresos(int automovilId, string tipo)
    {
        if (automovilId <= 0)
        {
            return BadRequest(new { message = "Debe seleccionar un automovil valido." });
        }

        if (string.IsNullOrWhiteSpace(tipo))
        {
            return BadRequest(new { message = "Debe seleccionar un tipo." });
        }

        try
        {
            var start = new DateTime(2000, 1, 1).ToString("O");
            var end = new DateTime(2100, 12, 31).ToString("O");

            var ingresos = await apiService.GetAsync<List<IngresoViewModel>>(
                $"ingresos/query/tipo?value={Uri.EscapeDataString(tipo)}&start={Uri.EscapeDataString(start)}&end={Uri.EscapeDataString(end)}") ?? [];

            var filtrados = ingresos
                .Where(ingreso => ingreso.AutomovilId == automovilId)
                .OrderByDescending(ingreso => ingreso.FechaEntrada)
                .ToList();

            return Json(filtrados);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode((int?)ex.StatusCode ?? 500, new { message = ex.Message });
        }
    }

    [ApiSourceFeature("AutomovilesCrud")]
    [HttpGet]
    public async Task<IActionResult> Index(int? anioInferior, int? anioSuperior)
    {
        List<AutomovilViewModel> automoviles = [];

        try
        {
            if (anioInferior.HasValue && anioSuperior.HasValue)
            {
                if (anioInferior > anioSuperior)
                {
                    ModelState.AddModelError(string.Empty, "El rango de anios no es valido.");
                }
                else
                {
                    automoviles = await apiService.GetAsync<List<AutomovilViewModel>>(
                        $"automoviles/filter/year-range?start={anioInferior.Value}&end={anioSuperior.Value}") ?? [];
                }
            }
            else
            {
                automoviles = await apiService.GetAsync<List<AutomovilViewModel>>("automoviles") ?? [];
            }
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewBag.AnioInferior = anioInferior;
        ViewBag.AnioSuperior = anioSuperior;
        return View(automoviles);
    }

    [ApiSourceFeature("AutomovilesCrud")]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var automovil = await TryGetAutomovilAsync(id);
        return automovil is null ? NotFound() : View(automovil);
    }

    [ApiSourceFeature("AutomovilesCrud")]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new AutomovilViewModel());
    }

    [ApiSourceFeature("AutomovilesCrud")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AutomovilViewModel automovil)
    {
        if (!ModelState.IsValid)
        {
            return View(automovil);
        }

        try
        {
            await apiService.PostAsync<AutomovilViewModel>("automoviles", automovil);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(automovil);
        }
    }

    [ApiSourceFeature("AutomovilesCrud")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var automovil = await TryGetAutomovilAsync(id);
        return automovil is null ? NotFound() : View(automovil);
    }

    [ApiSourceFeature("AutomovilesCrud")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AutomovilViewModel automovil)
    {
        if (id != automovil.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(automovil);
        }

        try
        {
            await apiService.PutAsync<object>($"automoviles/{id}", automovil);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(automovil);
        }
    }

    [ApiSourceFeature("AutomovilesCrud")]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var automovil = await TryGetAutomovilAsync(id);
        return automovil is null ? NotFound() : View(automovil);
    }

    [ApiSourceFeature("AutomovilesCrud")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await apiService.DeleteAsync($"automoviles/{id}");
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            var automovil = await TryGetAutomovilAsync(id);
            if (automovil is null)
            {
                return NotFound();
            }

            return View(automovil);
        }
    }

    private async Task<AutomovilViewModel?> TryGetAutomovilAsync(int id)
    {
        try
        {
            return await apiService.GetAsync<AutomovilViewModel>($"automoviles/{id}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}