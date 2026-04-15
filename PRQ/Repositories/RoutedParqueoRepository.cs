using PRQ.Interfaces;
using PRQ.Models;
using PRQ.Repositories.DB;
using PRQ.Repositories.Json;

namespace PRQ.Repositories;

public class RoutedParqueoRepository(
    ParqueoDbRepository dbRepository,
    ParqueoJsonRepository jsonRepository,
    IApiSourceSelector sourceSelector) : IParqueoRepository
{
    private IParqueoRepository Current => sourceSelector.UseJsonSource() ? jsonRepository : dbRepository;

    public IEnumerable<Parqueo> GetAll() => Current.GetAll();
    public Parqueo? GetById(int id) => Current.GetById(id);
    public void Insert(Parqueo parqueo) => Current.Insert(parqueo);
    public void Update(Parqueo parqueo) => Current.Update(parqueo);
    public void Delete(int id) => Current.Delete(id);
    public IEnumerable<Parqueo> GetByProvincia(string provincia) => Current.GetByProvincia(provincia);
    public IEnumerable<Parqueo> GetByNombre(string nombre) => Current.GetByNombre(nombre);
    public IEnumerable<Parqueo> GetByPriceRange(decimal minPrice, decimal maxPrice) => Current.GetByPriceRange(minPrice, maxPrice);
    public decimal ObtenerPrecioPorHoraPorParqueo(int parqueoId) => Current.ObtenerPrecioPorHoraPorParqueo(parqueoId);
}