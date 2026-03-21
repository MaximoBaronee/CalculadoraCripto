using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CalculadoraCripto.Models;
using CalculadoraCripto.Data;

namespace CalculadoraCripto.Controllers;

[Authorize]
public class OperacionesController : Controller
{
    private int ObtenerUsuarioId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    }

    public IActionResult Index()
    {
        var idUsuario = ObtenerUsuarioId();
        var operaciones = Database.ObtenerOperacionesPorUsuario(idUsuario);
        var totales = Database.ObtenerTotales(idUsuario);

        ViewBag.TotalInvertido = totales.totalInvertido;
        ViewBag.TotalFinal = totales.totalFinal;
        ViewBag.GananciaTotal = totales.gananciaTotal;
        ViewBag.PorcentajeTotal = totales.totalInvertido > 0
            ? (totales.gananciaTotal / totales.totalInvertido) * 100
            : 0;

        return View(operaciones);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Operacion operacion)
    {
        // Calcular valores automáticos
        operacion.Invertido = operacion.PrecioCompra * operacion.Cantidad;
        operacion.ValorFinal = operacion.PrecioVenta * operacion.Cantidad;
        operacion.Ganancia = (operacion.PrecioVenta - operacion.PrecioCompra) * operacion.Cantidad;
        operacion.Porcentaje = operacion.PrecioCompra > 0
            ? (operacion.PrecioVenta - operacion.PrecioCompra) / operacion.PrecioCompra * 100
            : 0;

        // Asignar usuario logueado
        operacion.IdUsuario = ObtenerUsuarioId();

        // Validar modelo
        if (!ModelState.IsValid)
        {
            return View(operacion);
        }

        Database.InsertarOperacion(operacion);
        TempData["Mensaje"] = "Operación registrada correctamente";
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var idUsuario = ObtenerUsuarioId();
        var operacion = Database.ObtenerOperacionPorId(id, idUsuario);
        if (operacion == null)
            return NotFound();

        return View(operacion);
    }

    [HttpPost]
    public IActionResult Edit(Operacion operacion)
    {
        // Recalcular
        operacion.Invertido = operacion.PrecioCompra * operacion.Cantidad;
        operacion.ValorFinal = operacion.PrecioVenta * operacion.Cantidad;
        operacion.Ganancia = (operacion.PrecioVenta - operacion.PrecioCompra) * operacion.Cantidad;
        operacion.Porcentaje = operacion.PrecioCompra > 0
            ? (operacion.PrecioVenta - operacion.PrecioCompra) / operacion.PrecioCompra * 100
            : 0;

        operacion.IdUsuario = ObtenerUsuarioId();

        if (!ModelState.IsValid)
        {
            return View(operacion);
        }

        Database.ActualizarOperacion(operacion);
        TempData["Mensaje"] = "Operación actualizada";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var idUsuario = ObtenerUsuarioId();
        Database.EliminarOperacion(id, idUsuario);
        TempData["Mensaje"] = "Operación eliminada";
        return RedirectToAction("Index");
    }
}