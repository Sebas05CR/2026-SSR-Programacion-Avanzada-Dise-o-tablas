using System.ComponentModel.DataAnnotations;

namespace PRQ.MVC.Models;

public class AutomovilViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Fabricante { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Color { get; set; } = string.Empty;

    [Range(1886, 3000)]
    public int Anio { get; set; }

    [Required]
    [StringLength(50)]
    public string Tipo { get; set; } = string.Empty;
}