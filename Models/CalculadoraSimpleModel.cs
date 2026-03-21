using System.ComponentModel.DataAnnotations;

namespace CalculadoraCripto.Models;

public class CalculadoraSimpleModel
{
    [Required(ErrorMessage = "La criptomoneda es obligatoria")]
    public string Criptomoneda { get; set; }

    [Required(ErrorMessage = "El precio de compra es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Debe ser mayor a 0")]
    public decimal PrecioCompra { get; set; }

    [Required(ErrorMessage = "El precio de venta es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Debe ser mayor a 0")]
    public decimal PrecioVenta { get; set; }

    // Resultados calculados
    public decimal Ganancia { get; set; }
    public decimal Porcentaje { get; set; }
}