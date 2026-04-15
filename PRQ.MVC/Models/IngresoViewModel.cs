namespace PRQ.MVC.Models;

public class IngresoViewModel
{
    public int Consecutivo { get; set; }
    public int AutomovilId { get; set; }
    public int ParqueoId { get; set; }
    public DateTime FechaEntrada { get; set; }
    public DateTime? FechaSalida { get; set; }
    public ParqueoViewModel? Parqueo { get; set; }
    public AutomovilViewModel? Automovil { get; set; }
    public double? DuracionMinutos { get; set; }
    public double? DuracionHoras { get; set; }
    public decimal? MontoTotal { get; set; }
}