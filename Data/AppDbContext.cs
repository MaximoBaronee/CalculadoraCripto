using CalculadoraCripto.Models;
using Microsoft.EntityFrameworkCore;

namespace CalculadoraCripto.Data;

public class AppDbContext : DbContext
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Operacion> Operaciones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=cripto.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Operacion>()
            .HasOne(o => o.Usuario)
            .WithMany()
            .HasForeignKey(o => o.IdUsuario);
    }
}