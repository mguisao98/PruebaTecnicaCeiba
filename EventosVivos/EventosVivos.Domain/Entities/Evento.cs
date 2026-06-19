using Domain.Enums;

namespace Domain.Entities;

public class Evento
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int VenueId { get; set; }
    public Venue? Venue { get; set; }
    public int CapacidadMaxima { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal PrecioEntrada { get; set; }
    public TipoEvento Tipo { get; set; }
    public EstadoEvento Estado { get; set; } = EstadoEvento.Activo;
    public List<Reserva>? Reservas { get; set; }
}
