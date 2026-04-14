using System.Collections.Generic;
using PRQ.Models;

namespace PRQ.Interfaces;

public interface IParqueoRepository
{
    // ── CRUD ──────────────────────────────────────────────────────
    IEnumerable<Parqueo> GetAll();
    Parqueo?             GetById(int id);
    void                 Insert(Parqueo parqueo);
    void                 Update(Parqueo parqueo);
    void                 Delete(int id);

    // ── Filters ───────────────────────────────────────────────────
    IEnumerable<Parqueo> GetByProvincia(string provincia);
    IEnumerable<Parqueo> GetByNombre(string nombre);
    IEnumerable<Parqueo> GetByPrecioRange(decimal precioMin, decimal precioMax);

    // ── Advanced ──────────────────────────────────────────────────
    decimal ObtenerPrecioPorHoraPorParqueo(int parqueoId);
}
