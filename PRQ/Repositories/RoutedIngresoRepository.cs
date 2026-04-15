using PRQ.Interfaces;
using PRQ.Models;
using PRQ.Repositories.DB;
using PRQ.Repositories.Json;

namespace PRQ.Repositories;

public class RoutedIngresoRepository(
    IngresoDbRepository dbRepository,
    IngresoJsonRepository jsonRepository,
    IApiSourceSelector sourceSelector) : IIngresoRepository
{
    private IIngresoRepository Current => sourceSelector.UseJsonSource() ? jsonRepository : dbRepository;

    public IEnumerable<Ingreso> GetAll() => Current.GetAll();
    public Ingreso? GetById(int id) => Current.GetById(id);
    public void Insert(Ingreso ingreso) => Current.Insert(ingreso);
    public void Update(Ingreso ingreso) => Current.Update(ingreso);
    public void Delete(int id) => Current.Delete(id);
    public IEnumerable<Ingreso> GetByTipoAndDateRange(string tipo, DateTime start, DateTime end) => Current.GetByTipoAndDateRange(tipo, start, end);
    public IEnumerable<Ingreso> GetByProvinciaAndDateRange(string provincia, DateTime start, DateTime end) => Current.GetByProvinciaAndDateRange(provincia, start, end);
}