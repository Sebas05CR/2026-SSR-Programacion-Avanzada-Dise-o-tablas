namespace PRQ.MVC.Models;

public class ParqueoViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Provincia { get; set; } = string.Empty;
    public decimal PrecioPorHora { get; set; }
}