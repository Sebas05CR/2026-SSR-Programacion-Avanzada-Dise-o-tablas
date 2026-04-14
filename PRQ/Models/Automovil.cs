using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PRQ.Models;

public class Automovil
{
    public int    Id         { get; set; }
    public string Color      { get; set; } = string.Empty;
    public int    Anio       { get; set; }
    public string Fabricante { get; set; } = string.Empty;

    /// <summary>sedan | 4x4 | moto</summary>
    public string Tipo { get; set; } = string.Empty;

    [JsonIgnore]
    public ICollection<Ingreso>? Ingresos { get; set; }
}
