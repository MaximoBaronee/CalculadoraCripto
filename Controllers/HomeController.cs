using Microsoft.AspNetCore.Mvc;
using CalculadoraCripto.Models;

namespace CalculadoraCripto.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View(new CalculadoraSimpleModel());
    }

    [HttpPost]
    public IActionResult Calcular(CalculadoraSimpleModel modelo)
    {
        if (ModelState.IsValid)
        {
            // Calcular ganancia y porcentaje
            modelo.Ganancia = (modelo.PrecioVenta - modelo.PrecioCompra);
            modelo.Porcentaje = modelo.PrecioCompra > 0
                ? (modelo.PrecioVenta - modelo.PrecioCompra) / modelo.PrecioCompra * 100
                : 0;

            return View("Resultado", modelo);
        }

        return View("Index", modelo);
    }
}