using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CalculadoraCripto.Models;
using CalculadoraCripto.Data;

namespace CalculadoraCripto.Controllers;

[Authorize]
public class OperacionesController : Controller
{
    private readonly AppDbContext _context;

    public OperacionesController(AppDbContext context)
    {
        _context = context;
    }

    private int ObtenerUsuarioId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    }

    public IActionResult Index()
    {
        var idUsuario = ObtenerUsuarioId();

        var operaciones = _context.Operaciones
            .Where(o => o.IdUsuario == idUsuario)
            .OrderByDescending(o => o.Fecha)
            .ToList();

        var totalInvertido = _context.Operaciones
            .Where(o => o.IdUsuario == idUsuario)
            .Sum(o => (decimal?)o.Invertido) ?? 0;

        var totalFinal = _context.Operaciones
            .Where(o => o.IdUsuario == idUsuario)
            .Sum(o => (decimal?)o.ValorFinal) ?? 0;

        var gananciaTotal = _context.Operaciones
            .Where(o => o.IdUsuario == idUsuario)
            .Sum(o => (decimal?)o.Ganancia) ?? 0;

        ViewBag.TotalInvertido = totalInvertido;
        ViewBag.TotalFinal = totalFinal;
        ViewBag.GananciaTotal = gananciaTotal;
        ViewBag.PorcentajeTotal = totalInvertido > 0
            ? (gananciaTotal / totalInvertido) * 100
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
        Console.WriteLine("🔥 CREATE POST EJECUTADO");

        operacion.Invertido = operacion.PrecioCompra * operacion.Cantidad;
        operacion.ValorFinal = operacion.PrecioVenta * operacion.Cantidad;
        operacion.Ganancia = (operacion.PrecioVenta - operacion.PrecioCompra) * operacion.Cantidad;
        operacion.Porcentaje = operacion.PrecioCompra > 0
            ? (operacion.PrecioVenta - operacion.PrecioCompra) / operacion.PrecioCompra * 100
            : 0;

        operacion.IdUsuario = ObtenerUsuarioId();

        if (!ModelState.IsValid)
            return View(operacion);

        _context.Operaciones.Add(operacion);
        _context.SaveChanges();

        TempData["Mensaje"] = "Operación registrada correctamente";
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var idUsuario = ObtenerUsuarioId();

        var operacion = _context.Operaciones
            .FirstOrDefault(o => o.Id == id && o.IdUsuario == idUsuario);

        if (operacion == null)
            return NotFound();

        return View(operacion);
    }

    [HttpPost]
    public IActionResult Edit(Operacion operacion)
    {
        operacion.Invertido = operacion.PrecioCompra * operacion.Cantidad;
        operacion.ValorFinal = operacion.PrecioVenta * operacion.Cantidad;
        operacion.Ganancia = (operacion.PrecioVenta - operacion.PrecioCompra) * operacion.Cantidad;
        operacion.Porcentaje = operacion.PrecioCompra > 0
            ? (operacion.PrecioVenta - operacion.PrecioCompra) / operacion.PrecioCompra * 100
            : 0;

        operacion.IdUsuario = ObtenerUsuarioId();

        if (!ModelState.IsValid)
            return View(operacion);

        var existente = _context.Operaciones
            .FirstOrDefault(o => o.Id == operacion.Id && o.IdUsuario == operacion.IdUsuario);

        if (existente == null)
            return NotFound();

        _context.Entry(existente).CurrentValues.SetValues(operacion);
        _context.SaveChanges();

        TempData["Mensaje"] = "Operación actualizada";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var idUsuario = ObtenerUsuarioId();

        var operacion = _context.Operaciones
            .FirstOrDefault(o => o.Id == id && o.IdUsuario == idUsuario);

        if (operacion != null)
        {
            _context.Operaciones.Remove(operacion);
            _context.SaveChanges();
        }

        TempData["Mensaje"] = "Operación eliminada";
        return RedirectToAction("Index");
    }
}