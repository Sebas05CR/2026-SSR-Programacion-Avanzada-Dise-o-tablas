using System.Text.Json.Serialization;

namespace PRQ.Models;

public class Parqueo
{
    public int     Id            { get; set; }
    public string  Nombre        { get; set; } = string.Empty;
    public string  Provincia     { get; set; } = string.Empty;
    public decimal PrecioPorHora { get; set; }

    [JsonIgnore]
    public ICollection<Ingreso>? Ingresos { get; set; }
}
