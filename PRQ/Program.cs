using PRQ.Data;
using PRQ.Interfaces;
using PRQ.Models;
using PRQ.Repositories.DB;

string connStr = Environment.GetEnvironmentVariable("PRQ_CONN")
    ?? throw new InvalidOperationException(
        "Connection string not found. Set the PRQ_CONN environment variable before running.");

var dbContext = PRQDbContext.CreateForAzure(connStr);

IAutomovilRepository automovilRepo = new AutomovilDbRepository(dbContext);
IParqueoRepository   parqueoRepo   = new ParqueoDbRepository(dbContext);
IIngresoRepository   ingresoRepo   = new IngresoDbRepository(dbContext);

Console.WriteLine("╔══════════════════════════════════════════╗");
Console.WriteLine("║   PRQ - Sistema de Parqueo (Azure SQL)   ║");
Console.WriteLine("╚══════════════════════════════════════════╝");

// ════════════════════════════════════════════════════════════════════════════
// AUTOMOVILES
// ════════════════════════════════════════════════════════════════════════════
PrintHeader("Todos los automóviles");
foreach (var a in automovilRepo.GetAll())
    Console.WriteLine($"  [{a.Id}] {a.Fabricante,-10} | {a.Color,-8} | {a.Anio} | {a.Tipo}");

PrintHeader("Automóviles tipo 'sedan'");
foreach (var a in automovilRepo.GetByTipo("sedan"))
    Console.WriteLine($"  [{a.Id}] {a.Fabricante} - {a.Color}");

PrintHeader("Automóviles año 2018-2021");
foreach (var a in automovilRepo.GetByAnioRange(2018, 2021))
    Console.WriteLine($"  [{a.Id}] {a.Fabricante} {a.Anio}");

PrintHeader("Automóviles fabricante 'Honda'");
foreach (var a in automovilRepo.GetByFabricante("Honda"))
    Console.WriteLine($"  [{a.Id}] {a.Fabricante} - {a.Color}");

PrintHeader("Automóviles color 'Negro'");
foreach (var a in automovilRepo.GetByColor("Negro"))
    Console.WriteLine($"  [{a.Id}] {a.Fabricante} - {a.Color}");

// ════════════════════════════════════════════════════════════════════════════
// PARQUEOS
// ════════════════════════════════════════════════════════════════════════════
PrintHeader("Todos los parqueos");
foreach (var p in parqueoRepo.GetAll())
    Console.WriteLine($"  [{p.Id}] {p.Nombre,-20} | {p.Provincia,-10} | ₡{p.PrecioPorHora}/hr");

PrintHeader("Parqueos en provincia 'Heredia'");
foreach (var p in parqueoRepo.GetByProvincia("Heredia"))
    Console.WriteLine($"  [{p.Id}] {p.Nombre}");

PrintHeader("Parqueos con precio ₡350 a ₡450");
foreach (var p in parqueoRepo.GetByPrecioRange(350, 450))
    Console.WriteLine($"  [{p.Id}] {p.Nombre} - ₡{p.PrecioPorHora}/hr");

PrintHeader("ObtenerPrecioPorHoraPorParqueo(parqueoId: 1)");
var precio = parqueoRepo.ObtenerPrecioPorHoraPorParqueo(1);
Console.WriteLine($"  ₡{precio}/hr");

// ════════════════════════════════════════════════════════════════════════════
// INGRESOS — lista completa con campos calculados
// ════════════════════════════════════════════════════════════════════════════
PrintHeader("Todos los ingresos (con campos calculados)");
Console.WriteLine($"  {"#",-4} {"Auto",-12} {"Entrada",-18} {"Salida",-18} {"Min",-8} {"Hrs",-8} {"Total"}");
Console.WriteLine($"  {new string('-', 80)}");
foreach (var i in ingresoRepo.GetAll())
{
    var salida = i.FechaSalida?.ToString("dd/MM HH:mm") ?? "---";
    var mins   = i.DuracionMinutos?.ToString("F0")     ?? "NULL";
    var hrs    = i.DuracionHoras?.ToString("F2")       ?? "NULL";
    var total  = i.MontoTotal.HasValue ? $"₡{i.MontoTotal.Value:F2}" : "NULL";
    var auto   = i.Automovil is not null ? $"{i.Automovil.Fabricante}" : $"Id={i.AutomovilId}";

    Console.WriteLine($"  {i.Consecutivo,-4} {auto,-12} {i.FechaEntrada:dd/MM HH:mm}   {salida,-18} {mins,-8} {hrs,-8} {total}");
}

// ════════════════════════════════════════════════════════════════════════════
// QUERY — Vehículos tipo '4x4' en rango de fechas
// ════════════════════════════════════════════════════════════════════════════
var desde = new DateTime(2025, 1, 1);
var hasta = new DateTime(2025, 6, 30);

PrintHeader($"Ingresos tipo '4x4' del {desde:dd/MM/yyyy} al {hasta:dd/MM/yyyy}");
foreach (var i in ingresoRepo.GetByTipoAutomovilAndDateRange("4x4", desde, hasta))
{
    Console.WriteLine($"  [{i.Consecutivo}] {i.Automovil?.Fabricante,-10} " +
                      $"Entrada:{i.FechaEntrada:dd/MM HH:mm}  " +
                      $"Salida:{i.FechaSalida?.ToString("dd/MM HH:mm") ?? "NULL",-14}  " +
                      $"Total:{i.MontoTotal?.ToString("F2") ?? "NULL"}");
}

// ════════════════════════════════════════════════════════════════════════════
// QUERY — Vehículos en provincia 'San José' en rango de fechas
//         (precio por hora obtenido vía ObtenerPrecioPorHoraPorParqueo)
// ════════════════════════════════════════════════════════════════════════════
PrintHeader($"Ingresos en 'San José' del {desde:dd/MM/yyyy} al {hasta:dd/MM/yyyy}");
foreach (var i in ingresoRepo.GetByProvinciaAndDateRange("San José", desde, hasta))
{
    var precioPH = parqueoRepo.ObtenerPrecioPorHoraPorParqueo(i.ParqueoId);
    Console.WriteLine($"  [{i.Consecutivo}] {i.Automovil?.Fabricante,-10} " +
                      $"Parqueo:{i.Parqueo?.Nombre,-17} " +
                      $"₡/hr:{precioPH,-6} " +
                      $"Entrada:{i.FechaEntrada:dd/MM HH:mm}  " +
                      $"Salida:{i.FechaSalida?.ToString("dd/MM HH:mm") ?? "NULL",-14}  " +
                      $"Total:{i.MontoTotal?.ToString("F2") ?? "NULL"}");
}

// ════════════════════════════════════════════════════════════════════════════
// CRUD DEMO — Insert / Update / Delete
// ════════════════════════════════════════════════════════════════════════════
PrintHeader("CRUD Demo: Insert automóvil nuevo");
var nuevo = new Automovil { Color = "Plateado", Anio = 2024, Fabricante = "BMW", Tipo = "sedan" };
automovilRepo.Insert(nuevo);
Console.WriteLine($"  Insertado: [{nuevo.Id}] {nuevo.Fabricante}");

PrintHeader($"CRUD Demo: Update automóvil #{nuevo.Id}");
nuevo.Color = "Negro Mate";
automovilRepo.Update(nuevo);
Console.WriteLine($"  Actualizado: [{nuevo.Id}] Color={automovilRepo.GetById(nuevo.Id)?.Color}");

PrintHeader($"CRUD Demo: Delete automóvil #{nuevo.Id}");
automovilRepo.Delete(nuevo.Id);
Console.WriteLine($"  Eliminado. GetById({nuevo.Id}) = {automovilRepo.GetById(nuevo.Id)?.Fabricante ?? "null"}");

Console.WriteLine();
Console.WriteLine("Done.");

// ── Utility ──────────────────────────────────────────────────────────────────
static void PrintHeader(string title)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"── {title} ");
    Console.ResetColor();
}
