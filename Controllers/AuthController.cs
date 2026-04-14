using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CalculadoraCripto.Models;
using CalculadoraCripto.Data;
using BCrypt.Net;

namespace CalculadoraCripto.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string nombreUsuario, string password)
    {
        if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(password))
        {
            ViewBag.Error = "Complete todos los campos";
            return View();
        }

        var usuario = _context.Usuarios
            .FirstOrDefault(u => u.NombreUsuario == nombreUsuario);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.Password))
        {
            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), authProperties);

        return RedirectToAction("Index", "Operaciones");
    }

    public IActionResult Registro()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registro(Usuario usuario, string confirmPassword)
    {
        Console.WriteLine("---- DEBUG REGISTRO ----");
        Console.WriteLine($"Usuario: {usuario.NombreUsuario}");
        Console.WriteLine($"Email: {usuario.Email}");
        Console.WriteLine($"Password: {usuario.Password}");

        if (usuario.Password != confirmPassword)
        {
            ModelState.AddModelError("Password", "Las contraseñas no coinciden");
        }

        if (_context.Usuarios.Any(u => u.NombreUsuario == usuario.NombreUsuario))
        {
            ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya existe");
        }

        if (!ModelState.IsValid)
        {
            Console.WriteLine("---- ERRORES ----");

            foreach (var error in ModelState)
            {
                foreach (var subError in error.Value.Errors)
                {
                    Console.WriteLine($"Error en {error.Key}: {subError.ErrorMessage}");
                }
            }

            return View(usuario);
        }

        usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);

        _context.Usuarios.Add(usuario);
        _context.SaveChanges();

        TempData["Mensaje"] = "Registro exitoso. Inicie sesión.";
        return RedirectToAction("Login");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}