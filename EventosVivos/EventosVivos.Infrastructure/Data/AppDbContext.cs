using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Evento> Eventos { get; set; }
    public DbSet<Venue> Venues { get; set; }
    public DbSet<Reserva> Reservas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed de Venues (los que da la prueba)
        modelBuilder.Entity<Venue>().HasData(
            new Venue { Id = 1, Nombre = "Auditorio Central", Capacidad = 200, Ciudad = "Bogotá" },
            new Venue { Id = 2, Nombre = "Sala Norte", Capacidad = 50, Ciudad = "Bogotá" },
            new Venue { Id = 3, Nombre = "Arena Sur", Capacidad = 500, Ciudad = "Medellín" }
        );
    }
}
