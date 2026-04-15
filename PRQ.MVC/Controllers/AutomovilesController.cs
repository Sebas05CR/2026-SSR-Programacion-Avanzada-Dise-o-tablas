using System.Net;
using Microsoft.AspNetCore.Mvc;
using PRQ.MVC.Models;
using PRQ.MVC.Services;

namespace PRQ.MVC.Controllers;

public class AutomovilesController(IApiService apiService) : Controller
{
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

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var automovil = await TryGetAutomovilAsync(id);
        return automovil is null ? NotFound() : View(automovil);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new AutomovilViewModel());
    }

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

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var automovil = await TryGetAutomovilAsync(id);
        return automovil is null ? NotFound() : View(automovil);
    }

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

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var automovil = await TryGetAutomovilAsync(id);
        return automovil is null ? NotFound() : View(automovil);
    }

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