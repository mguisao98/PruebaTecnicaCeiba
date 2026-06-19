using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly AppDbContext _context;

    public VenuesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CrearVenue([FromBody] Venue venue)
    {
        _context.Venues.Add(venue);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(ObtenerVenues), venue);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerVenues()
    {
        var venues = await _context.Venues.ToListAsync();
        return Ok(venues);
    }
}
