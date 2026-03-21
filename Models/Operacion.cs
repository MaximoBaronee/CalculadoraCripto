using System.ComponentModel.DataAnnotations;

namespace CalculadoraCripto.Models;

public class Operacion
{
    public int Id { get; set; }
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "La criptomoneda es obligatoria")]
    public string Criptomoneda { get; set; }

    [Required(ErrorMessage = "El precio de compra es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Debe ser mayor a 0")]
    public decimal PrecioCompra { get; set; }

    [Required(ErrorMessage = "El precio de venta es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Debe ser mayor a 0")]
    public decimal PrecioVenta { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Debe ser mayor a 0")]
    public decimal Cantidad { get; set; }

    public decimal Invertido { get; set; }
    public decimal ValorFinal { get; set; }
    public decimal Ganancia { get; set; }
    public decimal Porcentaje { get; set; }

    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; }
}