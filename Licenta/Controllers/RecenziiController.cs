using System.Security.Claims;
using Licenta.Areas.Identity.Data;
using Licenta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class RecenziiController : ControllerBase
{
    private readonly LicentaDbContext _context;
    private readonly UserManager<LicentaUser> _userManager;

    public RecenziiController(LicentaDbContext context, UserManager<LicentaUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("{locatieId}")]
    public async Task<IActionResult> GetRecenzii(int locatieId)
    {
        var utilizator = await _userManager.GetUserAsync(User);
        var esteAdmin = User.IsInRole("Admin");
        var userId = utilizator?.Id;

        var recenzii = await _context.Recenzii
            .Where(r => r.LocatieId == locatieId)
            .OrderByDescending(r => r.DataRecenzie)
            .Select(r => new
            {
                id = r.IdRecenzie,
                utilizatorNume = r.Utilizator.Nume,
                data = r.DataRecenzie.ToString("dd/MM/yyyy"),
                rating = r.Rating,
                comentariu = r.Comentariu,
                canDelete = userId != null && (r.UtilizatorId == userId || esteAdmin)
            })
            .ToListAsync();

        return Ok(recenzii);
    }

//  Post recenzie noua
[HttpPost]
    public async Task<IActionResult> AdaugaRecenzie([FromBody] RecenzieDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Restrictie: un utilizator poate lasa o singura recenzie pe zi per locatie
        bool alreadyReviewedToday = await _context.Recenzii.AnyAsync(r =>
            r.UtilizatorId == dto.UtilizatorId &&
            r.LocatieId == dto.LocatieId &&
            r.DataRecenzie.Date == DateTime.Today);

        if (alreadyReviewedToday)
        {
            return BadRequest(new { error = "Ai lăsat deja o recenzie pentru această locație astăzi." });
        }

        bool userExists = await _context.Users.AnyAsync(u => u.Id == dto.UtilizatorId);
        bool locatieExists = await _context.Locatii.AnyAsync(l => l.IdLocatie == dto.LocatieId);

        if (!userExists || !locatieExists)
        {
            return BadRequest(new { error = "Utilizatorul sau locația nu există." });
        }

        var recenzie = new Recenzie
        {
            CodRecenzie = Guid.NewGuid().ToString().Substring(0, 8),
            DataRecenzie = DateTime.Now,
            Comentariu = dto.Comentariu,
            Rating = dto.Rating,
            UtilizatorId = dto.UtilizatorId,
            LocatieId = dto.LocatieId
        };

        _context.Recenzii.Add(recenzie);
        await _context.SaveChangesAsync();

        return Ok(new { success = true });
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> StergeRecenzie(int id)
    {
        var recenzie = await _context.Recenzii.Include(r => r.Utilizator).FirstOrDefaultAsync(r => r.IdRecenzie == id);
        if (recenzie == null)
            return NotFound();

        // Verificare permisiune: admin sau autor
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var esteAdmin = User.IsInRole("Admin");

        if (!esteAdmin && recenzie.UtilizatorId != userId)
            return Forbid();

        _context.Recenzii.Remove(recenzie);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Recenzie ștearsă." });
    }

    [HttpGet("rating-summary/{locatieId}")]
    public IActionResult GetRatingSummary(int locatieId)
    {
        var recenzii = _context.Recenzii.Where(r => r.LocatieId == locatieId).ToList();

        if (!recenzii.Any())
            return Ok(new { medie = 0, total = 0 });

        var medie = recenzii.Average(r => r.Rating);
        var total = recenzii.Count;
        var distributie = recenzii.GroupBy(r => r.Rating)
                                  .ToDictionary(g => g.Key, g => g.Count());

        return Ok(new
        {
            medie = Math.Round(medie, 1),
            total,
            distributie
        });
    }

    [HttpGet("admin/all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllReviews()
    {
        var recenzii = await _context.Recenzii
            .Include(r => r.Locatie)
            .Include(r => r.Utilizator)
            .Select(r => new {
                r.IdRecenzie,
                r.Rating,
                r.Comentariu,
                r.DataRecenzie,
                Utilizator = r.Utilizator.Nume,
                Email = r.Utilizator.Email,
                Locatie = r.Locatie.Denumire
            })
            .ToListAsync();

        return Ok(recenzii);
    }


}
