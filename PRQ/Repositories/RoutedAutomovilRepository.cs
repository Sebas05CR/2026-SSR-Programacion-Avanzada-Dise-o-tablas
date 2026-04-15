using PRQ.Interfaces;
using PRQ.Models;
using PRQ.Repositories.DB;
using PRQ.Repositories.Json;

namespace PRQ.Repositories;

public class RoutedAutomovilRepository(
    AutomovilDbRepository dbRepository,
    AutomovilJsonRepository jsonRepository,
    IApiSourceSelector sourceSelector) : IAutomovilRepository
{
    private IAutomovilRepository Current => sourceSelector.UseJsonSource() ? jsonRepository : dbRepository;

    public IEnumerable<Automovil> GetAll() => Current.GetAll();
    public Automovil? GetById(int id) => Current.GetById(id);
    public void Insert(Automovil automovil) => Current.Insert(automovil);
    public void Update(Automovil automovil) => Current.Update(automovil);
    public void Delete(int id) => Current.Delete(id);
    public IEnumerable<Automovil> GetByColor(string color) => Current.GetByColor(color);
    public IEnumerable<Automovil> GetByYearRange(int startYear, int endYear) => Current.GetByYearRange(startYear, endYear);
    public IEnumerable<Automovil> GetByFabricante(string fabricante) => Current.GetByFabricante(fabricante);
    public IEnumerable<Automovil> GetByTipo(string tipo) => Current.GetByTipo(tipo);
}