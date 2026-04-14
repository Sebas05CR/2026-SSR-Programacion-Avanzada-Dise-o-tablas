using System;
using System.Collections.Generic;
using PRQ.Models;

namespace PRQ.Interfaces;

public interface IIngresoRepository
{
    // ── CRUD ──────────────────────────────────────────────────────
    IEnumerable<Ingreso> GetAll();
    Ingreso?             GetById(int consecutivo);
    void                 Insert(Ingreso ingreso);
    void                 Update(Ingreso ingreso);
    void                 Delete(int consecutivo);

    // ── Advanced queries ──────────────────────────────────────────

    /// <summary>
    /// Returns ingresos whose Automovil matches <paramref name="tipo"/>
    /// and whose FechaEntrada falls within [desde, hasta].
    /// Navigation properties (Automovil, Parqueo) are loaded so
    /// MontoTotal is available on each result.
    /// </summary>
    IEnumerable<Ingreso> GetByTipoAutomovilAndDateRange(
        string tipo, DateTime desde, DateTime hasta);

    /// <summary>
    /// Returns ingresos whose Parqueo.Provincia matches
    /// <paramref name="provincia"/> and FechaEntrada in [desde, hasta].
    /// Uses ObtenerPrecioPorHoraPorParqueo internally (loaded via Parqueo).
    /// </summary>
    IEnumerable<Ingreso> GetByProvinciaAndDateRange(
        string provincia, DateTime desde, DateTime hasta);
}
