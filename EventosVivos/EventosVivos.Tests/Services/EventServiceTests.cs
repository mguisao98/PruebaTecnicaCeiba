using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using EventosVivos.Application.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Tests.Services;

public class EventServiceTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private async Task SeedVenue(AppDbContext context)
    {
        context.Venues.Add(new Venue { Id = 1, Nombre = "Auditorio Test", Capacidad = 100, Ciudad = "Bogotá" });
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task CrearEvento_Valido_RetornaEventoActivo()
    {
        using var context = CreateContext();
        await SeedVenue(context);
        var service = new EventService(context);

        var evento = new Evento
        {
            Titulo = "Conferencia Tech",
            Descripcion = "Descripción válida del evento de conferencia tech",
            VenueId = 1,
            CapacidadMaxima = 80,
            FechaInicio = DateTime.UtcNow.AddDays(10),
            FechaFin = DateTime.UtcNow.AddDays(10).AddHours(3),
            PrecioEntrada = 60m,
            Tipo = TipoEvento.Conferencia
        };

        var resultado = await service.CrearEventoAsync(evento);

        Assert.NotNull(resultado);
        Assert.Equal("Conferencia Tech", resultado.Titulo);
        Assert.Equal(EstadoEvento.Activo, resultado.Estado);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("")]
    public async Task CrearEvento_TituloInvalido_ArrojaDomainException(string titulo)
    {
        using var context = CreateContext();
        await SeedVenue(context);
        var service = new EventService(context);

        var evento = new Evento
        {
            Titulo = titulo,
            Descripcion = "Descripción válida del evento de prueba técnica",
            VenueId = 1,
            CapacidadMaxima = 50,
            FechaInicio = DateTime.UtcNow.AddDays(5),
            FechaFin = DateTime.UtcNow.AddDays(5).AddHours(2),
            PrecioEntrada = 30m,
            Tipo = TipoEvento.Taller
        };

        await Assert.ThrowsAsync<DomainException>(() => service.CrearEventoAsync(evento));
    }

    [Fact]
    public async Task CrearEvento_VenueInexistente_ArrojaNotFoundException()
    {
        using var context = CreateContext();
        var service = new EventService(context);

        var evento = new Evento
        {
            Titulo = "Evento Sin Venue",
            Descripcion = "Descripción del evento sin venue registrado en sistema",
            VenueId = 999,
            CapacidadMaxima = 50,
            FechaInicio = DateTime.UtcNow.AddDays(5),
            FechaFin = DateTime.UtcNow.AddDays(5).AddHours(2),
            PrecioEntrada = 30m,
            Tipo = TipoEvento.Conferencia
        };

        await Assert.ThrowsAsync<NotFoundException>(() => service.CrearEventoAsync(evento));
    }

    [Fact]
    public async Task CrearEvento_CapacidadExcedeVenue_ArrojaDomainException()
    {
        using var context = CreateContext();
        await SeedVenue(context);
        var service = new EventService(context);

        var evento = new Evento
        {
            Titulo = "Evento Masivo",
            Descripcion = "Descripción del evento que supera la capacidad del venue asignado",
            VenueId = 1,
            CapacidadMaxima = 200,
            FechaInicio = DateTime.UtcNow.AddDays(5),
            FechaFin = DateTime.UtcNow.AddDays(5).AddHours(2),
            PrecioEntrada = 30m,
            Tipo = TipoEvento.Concierto
        };

        await Assert.ThrowsAsync<DomainException>(() => service.CrearEventoAsync(evento));
    }

    [Fact]
    public async Task CrearEvento_HorariosSolapados_ArrojaDomainException()
    {
        using var context = CreateContext();
        await SeedVenue(context);

        context.Eventos.Add(new Evento
        {
            Titulo = "Evento Existente ya",
            Descripcion = "Descripción del evento existente que ya ocupa el venue",
            VenueId = 1,
            CapacidadMaxima = 50,
            FechaInicio = DateTime.UtcNow.AddDays(10),
            FechaFin = DateTime.UtcNow.AddDays(10).AddHours(4),
            PrecioEntrada = 50m,
            Tipo = TipoEvento.Conferencia,
            Estado = EstadoEvento.Activo
        });
        await context.SaveChangesAsync();

        var service = new EventService(context);
        var eventoNuevo = new Evento
        {
            Titulo = "Evento Solapado",
            Descripcion = "Descripción del evento nuevo que se solapa con el anterior",
            VenueId = 1,
            CapacidadMaxima = 30,
            FechaInicio = DateTime.UtcNow.AddDays(10).AddHours(2),
            FechaFin = DateTime.UtcNow.AddDays(10).AddHours(6),
            PrecioEntrada = 75m,
            Tipo = TipoEvento.Taller
        };

        await Assert.ThrowsAsync<DomainException>(() => service.CrearEventoAsync(eventoNuevo));
    }

    [Fact]
    public async Task CrearEvento_FinDeSemanaDesp22h_ArrojaDomainException()
    {
        using var context = CreateContext();
        await SeedVenue(context);
        var service = new EventService(context);

        var proximoSabado = DateTime.UtcNow.AddDays(1);
        while (proximoSabado.DayOfWeek != DayOfWeek.Saturday)
            proximoSabado = proximoSabado.AddDays(1);

        var fechaNocturna = new DateTime(proximoSabado.Year, proximoSabado.Month, proximoSabado.Day, 22, 30, 0, DateTimeKind.Utc);

        var evento = new Evento
        {
            Titulo = "Fiesta Nocturna",
            Descripcion = "Descripción del evento nocturno de fin de semana prohibido",
            VenueId = 1,
            CapacidadMaxima = 50,
            FechaInicio = fechaNocturna,
            FechaFin = fechaNocturna.AddHours(3),
            PrecioEntrada = 80m,
            Tipo = TipoEvento.Concierto
        };

        await Assert.ThrowsAsync<DomainException>(() => service.CrearEventoAsync(evento));
    }

    [Fact]
    public async Task FiltrarEventos_PorTipo_RetornaSoloEseTipo()
    {
        using var context = CreateContext();
        await SeedVenue(context);

        context.Eventos.AddRange(
            new Evento { Titulo = "Conf A válida", Descripcion = "Descripción conferencia A del sistema", VenueId = 1, CapacidadMaxima = 30, FechaInicio = DateTime.UtcNow.AddDays(5), FechaFin = DateTime.UtcNow.AddDays(5).AddHours(2), PrecioEntrada = 20m, Tipo = TipoEvento.Conferencia, Estado = EstadoEvento.Activo },
            new Evento { Titulo = "Taller B válido", Descripcion = "Descripción taller B del sistema registrado", VenueId = 1, CapacidadMaxima = 20, FechaInicio = DateTime.UtcNow.AddDays(6), FechaFin = DateTime.UtcNow.AddDays(6).AddHours(2), PrecioEntrada = 15m, Tipo = TipoEvento.Taller, Estado = EstadoEvento.Activo }
        );
        await context.SaveChangesAsync();

        var service = new EventService(context);
        var resultado = await service.FiltrarEventosAsync(TipoEvento.Conferencia, null, null, null, null, null);

        Assert.All(resultado, e => Assert.Equal(TipoEvento.Conferencia, e.Tipo));
    }
}
