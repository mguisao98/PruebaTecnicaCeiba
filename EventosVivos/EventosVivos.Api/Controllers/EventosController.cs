using Domain.Entities;
using Domain.Enums;
using EventosVivos.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventosVivos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventosController : ControllerBase
{
    private readonly EventService _eventService;
    private readonly ReservationService _reservationService;

    public EventosController(EventService eventService, ReservationService reservationService)
    {
        _eventService = eventService;
        _reservationService = reservationService;
    }

    [HttpPost]
    public async Task<IActionResult> CrearEvento([FromBody] Evento evento)
    {
        var nuevoEvento = await _eventService.CrearEventoAsync(evento);
        return CreatedAtAction(nameof(ObtenerEvento), new { id = nuevoEvento.Id }, nuevoEvento);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerEventos()
    {
        var eventos = await _eventService.ObtenerEventosAsync();
        return Ok(eventos);
    }

    [HttpGet("filtrar")]
    public async Task<IActionResult> FiltrarEventos(
        [FromQuery] TipoEvento? tipo,
        [FromQuery] int? venueId,
        [FromQuery] EstadoEvento? estado,
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        [FromQuery] string? titulo)
    {
        var eventos = await _eventService.FiltrarEventosAsync(tipo, venueId, estado, fechaInicio, fechaFin, titulo);
        return Ok(eventos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerEvento(int id)
    {
        var evento = await _eventService.ObtenerEventoPorIdAsync(id);
        return Ok(evento);
    }

    [HttpGet("{eventoId}/reporte-ocupacion")]
    public async Task<IActionResult> ObtenerReporte(int eventoId)
    {
        var reporte = await _reservationService.ObtenerReporteOcupacionAsync(eventoId);
        return Ok(reporte);
    }
}
