namespace PRQ.MVC.Models;

public class AutomovilViewModel
{
    public int Id { get; set; }
    public string Fabricante { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Anio { get; set; }
    public string Tipo { get; set; } = string.Empty;
}