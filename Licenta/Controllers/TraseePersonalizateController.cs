using Licenta.Areas.Identity.Data;
using Licenta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TraseePersonalizateController : ControllerBase
{
    private readonly LicentaDbContext _context;
    private readonly UserManager<LicentaUser> _userManager;

    public TraseePersonalizateController(LicentaDbContext context, UserManager<LicentaUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrasee()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var trasee = await _context.Trasee
            .Where(t => t.UtilizatorId == user.Id)
            .Select(t => new
            {
                t.IdTraseu,
                t.DenumireTraseu,
                t.DataCreare,
                t.DurataEstimata
            })
            .ToListAsync();

        return Ok(trasee);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreeazaTraseuNou([FromQuery] int templateTraseuId, [FromQuery] string templateName)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var template = await _context.Trasee
            .Include(t => t.Itinerarii)
                .ThenInclude(i => i.LocatieTematica)
            .FirstOrDefaultAsync(t => t.IdTraseu == templateTraseuId);

        if (template == null)
            return NotFound("Traseul de bază nu există.");

        if (template.TematicaId == null)
            return BadRequest("Traseul original nu are tematică.");

        var prefix = $"Traseul meu personalizat {templateName} #";
        var existente = await _context.Trasee
            .Where(t => t.UtilizatorId == user.Id && t.DenumireTraseu.StartsWith(prefix))
            .Select(t => t.DenumireTraseu)
            .ToListAsync();

        var numereExistente = existente
            .Select(nume =>
            {
                var part = nume.Replace(prefix, "");
                return int.TryParse(part, out int nr) ? nr : -1;
            })
            .Where(nr => nr > 0)
            .OrderBy(nr => nr)
            .ToList();

        int nextNumber = 1;
        foreach (var nr in numereExistente)
        {
            if (nr != nextNumber)
                break;
            nextNumber++;
        }

        var traseuNou = new Traseu
        {
            DenumireTraseu = $"{prefix}{nextNumber}",
            Descriere = "Traseu personalizat",
            DataCreare = DateTime.Now,
            DurataEstimata = 0,
            UtilizatorId = user.Id,
            TematicaId = template.TematicaId
        };

        _context.Trasee.Add(traseuNou);
        await _context.SaveChangesAsync();

        var itinerariiCopiati = template.Itinerarii
            .OrderBy(i => i.NrOrdine)
            .Select((i, index) => new Itinerar
            {
                TraseuId = traseuNou.IdTraseu,
                LocatieTematicaId = i.LocatieTematicaId,
                NrOrdine = index + 1
            });

        _context.Itinerarii.AddRange(itinerariiCopiati);
        await _context.SaveChangesAsync();

        return Ok(new { traseuNou.IdTraseu, traseuNou.DenumireTraseu });
    }


    [HttpGet("locatii")]
    public async Task<IActionResult> GetLocatii(int traseuId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var traseu = _context.Trasee
    .Include(t => t.Itinerarii)
        .ThenInclude(i => i.LocatieTematica)
            .ThenInclude(lt => lt.Locatie)
                .ThenInclude(l => l.Imagini)
    .Include(t => t.Itinerarii)
        .ThenInclude(i => i.LocatieTematica)
            .ThenInclude(lt => lt.Locatie)
                .ThenInclude(l => l.Oras)
                    .ThenInclude(o => o.Tara)
    .FirstOrDefault(t => t.IdTraseu == traseuId);

        if (traseu == null)
            return NotFound("Traseul nu a fost găsit.");

        var locatii = traseu.Itinerarii
            .OrderBy(i => i.NrOrdine)
            .Select(i => new LocatieDto
            {
                Id = i.LocatieTematica.Locatie.IdLocatie,
                Name = i.LocatieTematica.Locatie.Denumire,
                Lat = double.Parse(i.LocatieTematica.Locatie.Geolocatie.Split(',')[0]),
                Lon = double.Parse(i.LocatieTematica.Locatie.Geolocatie.Split(',')[1]),
                ImageUrl = i.LocatieTematica.Locatie.Imagini.FirstOrDefault()?.Cale,
                Descriere = i.LocatieTematica.Locatie.Descriere,
                Strada = i.LocatieTematica.Locatie.Strada,
                NumarStrada = i.LocatieTematica.Locatie.NumarStrada,
                TimpEstimativ = i.LocatieTematica.Locatie.TimpEstimativ,
                Oras = i.LocatieTematica.Locatie.Oras.Denumire,
                Tara = i.LocatieTematica.Locatie.Oras.Tara.Denumire
            })
            .ToList();

        return Ok(locatii);
    }

    [HttpPost("salveaza-ordine")]
    public async Task<IActionResult> SalveazaOrdine(int traseuId, [FromBody] List<int> locatieIds)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var traseu = await _context.Trasee.FirstOrDefaultAsync(t => t.IdTraseu == traseuId && t.UtilizatorId == user.Id);
        if (traseu == null) return Forbid();

        var itinerarii = await _context.Itinerarii
            .Include(i => i.LocatieTematica)
                .ThenInclude(lt => lt.Locatie)
            .Where(i => i.TraseuId == traseuId)
            .ToListAsync();

        var deSters = itinerarii.Where(i => !locatieIds.Contains(i.LocatieTematica.Locatie.IdLocatie)).ToList();
        _context.Itinerarii.RemoveRange(deSters);

        for (int i = 0; i < locatieIds.Count; i++)
        {
            var locatieId = locatieIds[i];
            var itinerar = itinerarii.FirstOrDefault(x => x.LocatieTematica.Locatie.IdLocatie == locatieId);

            if (itinerar != null)
            {
                itinerar.NrOrdine = i + 1;
            }
            else
            {
                var locatieTematica = await _context.LocatiiTematice.FirstOrDefaultAsync(lt => lt.LocatieId == locatieId);
                if (locatieTematica != null)
                {
                    _context.Itinerarii.Add(new Itinerar
                    {
                        TraseuId = traseuId,
                        LocatieTematicaId = locatieTematica.IdLocatieTematica,
                        NrOrdine = i + 1
                    });
                }
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { success = true });
    }

    [HttpGet("locatii-disponibile")]
    public async Task<IActionResult> GetLocatiiDisponibile(int traseuId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var traseu = _context.Trasee
            .Include(t => t.Tematica)
                .ThenInclude(t => t.LocatiiTematice)
                    .ThenInclude(lt => lt.Locatie)
                        .ThenInclude(l => l.Imagini)
            .Include(t => t.Tematica)
                .ThenInclude(t => t.LocatiiTematice)
                    .ThenInclude(lt => lt.Locatie)
                        .ThenInclude(l => l.Oras)
                            .ThenInclude(o => o.Tara)
            .Include(t => t.Itinerarii)
                .ThenInclude(i => i.LocatieTematica)
                    .ThenInclude(lt => lt.Locatie)
            .FirstOrDefault(t => t.IdTraseu == traseuId);

        if (traseu == null)
            return NotFound();

        var locatiiInTraseu = traseu.Itinerarii
            .Select(i => i.LocatieTematica.Locatie.IdLocatie)
            .ToList();

        var disponibile = traseu.Tematica.LocatiiTematice
            .Where(lt => !locatiiInTraseu.Contains(lt.Locatie.IdLocatie))
            .Select(lt => new LocatieDto
            {
                Id = lt.Locatie.IdLocatie,
                Name = lt.Locatie.Denumire,
                Lat = double.Parse(lt.Locatie.Geolocatie.Split(',')[0]),
                Lon = double.Parse(lt.Locatie.Geolocatie.Split(',')[1]),
                ImageUrl = lt.Locatie.Imagini.FirstOrDefault()?.Cale,
                Descriere = lt.Locatie.Descriere,
                Strada = lt.Locatie.Strada,
                NumarStrada = lt.Locatie.NumarStrada,
                TimpEstimativ = lt.Locatie.TimpEstimativ,
                Oras = lt.Locatie.Oras.Denumire,
                Tara = lt.Locatie.Oras.Tara.Denumire
            })
            .ToList();

        return Ok(disponibile);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTraseu(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var traseu = await _context.Trasee.FirstOrDefaultAsync(t => t.IdTraseu == id && t.UtilizatorId == user.Id);
        if (traseu == null) return NotFound();

        _context.Trasee.Remove(traseu);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
