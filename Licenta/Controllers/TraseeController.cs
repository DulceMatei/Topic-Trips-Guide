using Licenta.Areas.Identity.Data;
using Licenta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TraseeController : ControllerBase
{
    private readonly LicentaDbContext _context;
    private readonly UserManager<LicentaUser> _userManager;

    public TraseeController(LicentaDbContext context, UserManager<LicentaUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    [HttpGet("locatii")]
    public IActionResult GetLocatiiDinTraseu(int traseuId)
    {
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
        var itinerarii = await _context.Itinerarii
            .Include(i => i.LocatieTematica)
                .ThenInclude(lt => lt.Locatie)
            .Where(i => i.TraseuId == traseuId)
            .ToListAsync();

        // Sterge locatiile eliminate
        var deSters = itinerarii
            .Where(i => !locatieIds.Contains(i.LocatieTematica.Locatie.IdLocatie))
            .ToList();

        _context.Itinerarii.RemoveRange(deSters);

        for (int i = 0; i < locatieIds.Count; i++)
        {
            var locatieId = locatieIds[i];
            var itinerar = itinerarii
                .FirstOrDefault(x => x.LocatieTematica.Locatie.IdLocatie == locatieId);

            if (itinerar != null)
            {
                itinerar.NrOrdine = i + 1;
            }
            else
            {
                var locatieTematica = await _context.LocatiiTematice
                    .FirstOrDefaultAsync(lt => lt.LocatieId == locatieId && lt.Tematica.Trasee.Any(t => t.IdTraseu == traseuId));

                if (locatieTematica != null)
                {
                    var nou = new Itinerar
                    {
                        TraseuId = traseuId,
                        LocatieTematicaId = locatieTematica.IdLocatieTematica,
                        NrOrdine = i + 1
                    };
                    _context.Itinerarii.Add(nou);
                }
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { success = true });
    }

    [HttpGet("locatii-disponibile")]
    public IActionResult GetLocatiiDisponibile(int traseuId)
    {
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


    private string FormatTimp(int minute)
    {
        if (minute < 60)
            return $"{minute} {(minute == 1 ? "minut" : "minute")}";

        int ore = minute / 60;
        int min = minute % 60;

        string oreText = ore == 1 ? "1 oră" : $"{ore} ore";
        string minText = min == 0 ? "" : (min == 1 ? "1 minut" : $"{min} minute");

        string rezultat = min == 0 ? oreText : $"{oreText} și {minText}";
        if (min > 0) rezultat += $" ({minute} minute)";
        return rezultat;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create-locatie")]
    public async Task<IActionResult> CreateLocatie([FromBody] LocatieCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Denumire) || string.IsNullOrWhiteSpace(dto.Geolocatie))
            return BadRequest(new { message = "Denumirea și geolocația sunt obligatorii." });

        int? taraId = dto.TaraId;
        int? orasId = dto.OrasId;

        // Verificare denumire duplicata
        var existaDenumire = await _context.Locatii
            .AnyAsync(l => l.Denumire == dto.Denumire.Trim());
        if (existaDenumire)
            return BadRequest(new { message = "Această denumire este deja folosită de o altă locație." });

        // Verificare geolocatie duplicata
        var existaGeolocatie = await _context.Locatii
            .AnyAsync(l => l.Geolocatie == dto.Geolocatie.Trim());
        if (existaGeolocatie)
            return BadRequest(new { message = "Această geolocație este deja asociată unei locații existente." });

        // Adaugare sau selectare tara
        if (!string.IsNullOrWhiteSpace(dto.TaraNoua))
        {
            var denumireTrim = dto.TaraNoua.Trim();
            var taraExistenta = await _context.Tari
                .FirstOrDefaultAsync(t => t.Denumire == denumireTrim);

            if (taraExistenta != null)
                taraId = taraExistenta.IdTara;
            else
            {
                var taraNoua = new Tara { Denumire = denumireTrim };
                _context.Tari.Add(taraNoua);
                await _context.SaveChangesAsync();
                taraId = taraNoua.IdTara;
            }
        }

        // Adaugare sau selectare oras
        if (!string.IsNullOrWhiteSpace(dto.OrasNou))
        {
            if (taraId == null)
                return BadRequest(new { message = "Trebuie să selectezi sau să adaugi o țară înainte de a adăuga un oraș nou." });

            var denumireOras = dto.OrasNou.Trim();
            var orasExistent = await _context.Orase
                .FirstOrDefaultAsync(o => o.Denumire == denumireOras && o.TaraId == taraId.Value);

            if (orasExistent != null)
                orasId = orasExistent.IdOras;
            else
            {
                var orasNou = new Oras { Denumire = denumireOras, TaraId = taraId.Value };
                _context.Orase.Add(orasNou);
                await _context.SaveChangesAsync();
                orasId = orasNou.IdOras;
            }
        }

        if (orasId == null)
            return BadRequest(new { message = "Trebuie să selectezi un oraș sau să adaugi unul nou." });

        // Creeaza locatia
        var locatie = new Locatie
        {
            Denumire = dto.Denumire.Trim(),
            TipLocatie = dto.TipLocatie?.Trim(),
            Descriere = dto.Descriere?.Trim(),
            Geolocatie = dto.Geolocatie.Trim(),
            TimpEstimativ = dto.TimpEstimativ,
            OrasId = orasId.Value,
            Strada = dto.Strada?.Trim(),
            NumarStrada = dto.NumarStrada?.Trim()
        };

        _context.Locatii.Add(locatie);
        await _context.SaveChangesAsync();

        // Imagine
        if (!string.IsNullOrWhiteSpace(dto.ImagineUrl))
        {
            _context.Imagini.Add(new Imagine
            {
                LocatieId = locatie.IdLocatie,
                NumeImagine = locatie.Denumire,
                Cale = dto.ImagineUrl.Trim()
            });
        }

        // Verifica traseul și tematica
        var traseu = await _context.Trasee
            .Include(t => t.Tematica)
            .FirstOrDefaultAsync(t => t.IdTraseu == dto.TraseuId);

        if (traseu == null)
            return BadRequest(new { message = "Traseul nu a fost găsit." });

        var locatieTematica = new LocatieTematica
        {
            LocatieId = locatie.IdLocatie,
            TematicaId = traseu.TematicaId
        };
        _context.LocatiiTematice.Add(locatieTematica);
        await _context.SaveChangesAsync();

        // Itinerar
        int nrMax = await _context.Itinerarii
            .Where(i => i.TraseuId == traseu.IdTraseu)
            .Select(i => (int?)i.NrOrdine)
            .MaxAsync() ?? 0;

        _context.Itinerarii.Add(new Itinerar
        {
            TraseuId = traseu.IdTraseu,
            LocatieTematicaId = locatieTematica.IdLocatieTematica,
            NrOrdine = nrMax + 1
        });

        await _context.SaveChangesAsync();
        return Ok();
    }




    [HttpGet("/api/tari")]
    public async Task<IActionResult> GetTari()
    {
        var tari = await _context.Tari
            .Select(t => new { id = t.IdTara, denumire = t.Denumire })
            .ToListAsync();

        return Ok(tari);
    }
    [HttpGet("/api/orase")]
    public async Task<IActionResult> GetOrase([FromQuery] int taraId)
    {
        if (taraId <= 0)
            return BadRequest("ID țară invalid.");

        var orase = await _context.Orase
            .Where(o => o.TaraId == taraId)
            .Select(o => new { id = o.IdOras, denumire = o.Denumire })
            .ToListAsync();

        return Ok(orase);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLocatie(int id)
    {
        var locatie = await _context.Locatii
            .Include(l => l.Recenzii)
            .Include(l => l.Imagini)
            .Include(l => l.TematiciLocatie)
            .FirstOrDefaultAsync(l => l.IdLocatie == id);

        if (locatie == null)
            return NotFound("Locația nu a fost găsită.");

        try
        {
            // Colecteaza Id-urile legaturilor LocatieTematica
            var locatieTematicaIds = locatie.TematiciLocatie
                .Select(lt => lt.IdLocatieTematica)
                .ToList();

            // Sterge itinerariile legate
            var itinerarii = await _context.Itinerarii
                .Where(i => locatieTematicaIds.Contains(i.LocatieTematicaId))
                .ToListAsync();
            _context.Itinerarii.RemoveRange(itinerarii);

            // Sterge recenziile
            if (locatie.Recenzii.Any())
                _context.Recenzii.RemoveRange(locatie.Recenzii);

            // Sterge imaginile
            if (locatie.Imagini.Any())
                _context.Imagini.RemoveRange(locatie.Imagini);

            // Sterge legaturile tematice
            if (locatie.TematiciLocatie.Any())
                _context.LocatiiTematice.RemoveRange(locatie.TematiciLocatie);

            // Sterge locația
            _context.Locatii.Remove(locatie);

            await _context.SaveChangesAsync();
            return Ok("Locația și toate legăturile au fost șterse.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Eroare la ștergere: {ex.Message} | {ex.InnerException?.Message}");
        }
    }



    [HttpPost("pdf-complet/{traseuId}")]
    public IActionResult GenereazaPdfComplet(int traseuId, [FromBody] PdfTraseuDto dto)
    {
        try
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(25);

                    page.Header().ShowOnce().Height(70).Background(Colors.Blue.Darken1).Padding(10)
                    .Column(headerCol =>
                    {
                        headerCol.Item().AlignCenter().Text(dto.Titlu)
                            .FontSize(24).Bold().FontColor(Colors.White);

                        headerCol.Item().AlignCenter()
                            .Text($"Traseu generat pe {DateTime.Now:dd MMMM yyyy}")
                            .FontSize(10).FontColor(Colors.Grey.Lighten1);
                    });

                    page.Content().PaddingTop(10).Column(col =>
                    {
                        if (!string.IsNullOrEmpty(dto.ImageBase64))
                        {
                            try
                            {
                                var base64Data = dto.ImageBase64.Contains(",")
                                    ? dto.ImageBase64.Split(',')[1]
                                    : dto.ImageBase64;
                                var bytes = Convert.FromBase64String(base64Data);

                                col.Item().PaddingBottom(20).Border(2).BorderColor(Colors.Grey.Lighten1)
                                    .Padding(5).Background(Colors.White)
                                    .Height(250).AlignCenter()
                                    .Image(bytes).FitArea();
                            }
                            catch
                            {
                                col.Item().Height(120).Background(Colors.Grey.Lighten3)
                                    .Border(1).BorderColor(Colors.Grey.Lighten1)
                                    .AlignCenter().AlignMiddle()
                                    .Text("Eroare la încărcarea hărții").FontSize(12).FontColor(Colors.Grey.Darken1);
                            }
                        }

                        col.Item().PaddingTop(25).PaddingBottom(15)
                            .Background(Colors.Blue.Lighten3).Padding(12)
                            .Text("🗺️ ITINERAR DETALIAT").FontSize(18).Bold().FontColor(Colors.Blue.Darken1);

                        for (int i = 0; i < dto.Locatii.Count; i++)
                        {
                            var loc = dto.Locatii[i];

                            col.Item().PaddingTop(15).ShowEntire().Border(1).BorderColor(Colors.Grey.Lighten1)
                                .Padding(15)
                                .Background(i % 2 == 0 ? Colors.White : Colors.Grey.Lighten3)
                                .Column(locCol =>
                                {
                                    locCol.Item().Row(row =>
                                    {
                                        row.ConstantItem(40).Height(40).Background(Colors.Blue.Medium)
                                            .AlignCenter().AlignMiddle()
                                            .Text($"{i + 1}").FontSize(16).Bold().FontColor(Colors.White);

                                        row.RelativeItem().PaddingLeft(15).AlignMiddle()
                                            .Text(loc.Nume).FontSize(16).Bold().FontColor(Colors.Blue.Darken1);
                                    });

                                    locCol.Item().PaddingTop(10).Row(infoRow =>
                                    {
                                        infoRow.RelativeItem().Column(leftCol =>
                                        {
                                            leftCol.Item().PaddingBottom(5).Row(coordRow =>
                                            {
                                                coordRow.ConstantItem(20).Text("📍").FontSize(12);
                                                coordRow.RelativeItem().PaddingLeft(5)
                                                    .Text($"Coordonate: {loc.Geolocatie}")
                                                    .FontSize(11).FontColor(Colors.Grey.Darken1);
                                            });

                                            if (!string.IsNullOrWhiteSpace(loc.Strada) || !string.IsNullOrWhiteSpace(loc.NumarStrada))
                                            {
                                                leftCol.Item().PaddingBottom(5).Row(addrRow =>
                                                {
                                                    addrRow.ConstantItem(20).Text("🏠").FontSize(12);
                                                    addrRow.RelativeItem().PaddingLeft(5)
                                                        .Text($"Adresă: {loc.Tara}, {loc.Oras}, {loc.Strada} {loc.NumarStrada}")
                                                        .FontSize(11).FontColor(Colors.Grey.Darken1);
                                                });
                                            }

                                            if (loc.TimpEstimativ.HasValue)
                                            {
                                                leftCol.Item().PaddingBottom(5).Row(timeRow =>
                                                {
                                                    timeRow.ConstantItem(20).Text("⏱️").FontSize(12);
                                                    timeRow.RelativeItem().PaddingLeft(5)
                                                        .Text($"Timp estimat: {FormatTimp(loc.TimpEstimativ.Value)}")
                                                        .FontSize(11).FontColor(Colors.Grey.Darken1);
                                                });
                                            }

                                            if (!string.IsNullOrWhiteSpace(loc.Descriere))
                                            {
                                                leftCol.Item().PaddingTop(8).PaddingBottom(5)
                                                    .Background(Colors.Blue.Lighten3).Padding(10)
                                                    .Text(loc.Descriere).FontSize(11).Italic().FontColor(Colors.Blue.Darken1);
                                            }
                                        });

                                        if (!string.IsNullOrEmpty(loc.ImagineUrl))
                                        {
                                            infoRow.ConstantItem(200).PaddingLeft(15).Height(150).Column(imgCol =>
                                            {
                                                try
                                                {
                                                    var imagePath = Path.Combine("wwwroot", loc.ImagineUrl.TrimStart('/'));
                                                    if (System.IO.File.Exists(imagePath))
                                                    {
                                                        var imageBytes = System.IO.File.ReadAllBytes(imagePath);
                                                        imgCol.Item().Border(2).BorderColor(Colors.Grey.Lighten1)
                                                            .Padding(3).Background(Colors.White).AlignCenter().AlignMiddle()
                                                            .Image(imageBytes).FitArea();
                                                    }
                                                }
                                                catch
                                                {
                                                    imgCol.Item().Height(100).Background(Colors.Grey.Lighten3)
                                                        .Border(1).BorderColor(Colors.Grey.Lighten1)
                                                        .AlignCenter().AlignMiddle()
                                                        .Text("📸\nImagine\nindisponibilă").FontSize(9)
                                                        .FontColor(Colors.Grey.Darken1).AlignCenter();
                                                }
                                            });
                                        }
                                    });
                                });

                            if (i < dto.Locatii.Count - 1 && loc.Distanta.HasValue && loc.Durata.HasValue)
                            {
                                col.Item().PaddingTop(8).PaddingBottom(8).AlignCenter()
                                    .Background(Colors.Green.Lighten2).Padding(12)
                                    .Column(distCol =>
                                    {
                                        distCol.Item().AlignCenter().PaddingBottom(8)
                                            .Text("🛣️ Călătorie până la următoarea locație")
                                            .FontSize(13).Bold().FontColor(Colors.Green.Darken2);

                                        distCol.Item().Row(distRow =>
                                        {
                                            distRow.AutoItem().Text("🚗 ").FontSize(14);
                                            distRow.AutoItem().Text($"{loc.Distanta:F1} km")
                                                .FontSize(12).Bold().FontColor(Colors.Green.Darken1);
                                            distRow.AutoItem().PaddingLeft(15).Text("⏰ ").FontSize(14);
                                            distRow.AutoItem().Text($"{FormatTimp((int)loc.Durata.Value)}")
                                                .FontSize(12).Bold().FontColor(Colors.Green.Darken1);
                                        });
                                    });
                            }
                        }


                        col.Item().PaddingTop(30).ShowEntire().Border(3).BorderColor(Colors.Blue.Medium)
                            .Background(Colors.Blue.Lighten3).Padding(20)
                            .Column(summary =>
                            {
                                summary.Item().AlignCenter().Text("📊 REZUMAT TRASEU")
                                    .FontSize(18).Bold().FontColor(Colors.Blue.Darken1);

                                summary.Item().PaddingTop(15).Row(summaryRow =>
                                {
                                    summaryRow.RelativeItem().Column(leftSum =>
                                    {
                                        leftSum.Item().PaddingBottom(8).Row(row =>
                                        {
                                            row.ConstantItem(30).Text("🛣️").FontSize(16);
                                            row.RelativeItem().Text($"Distanță totală: {dto.DistantaTotala:F1} km")
                                                .FontSize(14).Bold().FontColor(Colors.Blue.Darken1);
                                        });

                                        leftSum.Item().PaddingBottom(8).Row(row =>
                                        {
                                            row.ConstantItem(30).Text("🕐").FontSize(16);
                                            row.RelativeItem().Text($"Timp deplasare: {FormatTimp(dto.DurataTotala)}")
                                                .FontSize(14).Bold().FontColor(Colors.Blue.Darken1);
                                        });
                                    });

                                    summaryRow.RelativeItem().Column(rightSum =>
                                    {
                                        rightSum.Item().PaddingBottom(8).Row(row =>
                                        {
                                            row.ConstantItem(30).Text("📍").FontSize(16);
                                            row.RelativeItem().Text($"Numărul de locații: {dto.Locatii.Count}")
                                                .FontSize(14).Bold().FontColor(Colors.Blue.Darken1);
                                        });

                                        int timpTotal = dto.Locatii.Sum(l => l.TimpEstimativ ?? 0);
                                        rightSum.Item().PaddingBottom(8).Row(row =>
                                        {
                                            row.ConstantItem(30).Text("⏳").FontSize(16);
                                            row.RelativeItem().Text($"Timp total la locații: {FormatTimp(timpTotal)}")
                                                .FontSize(14).Bold().FontColor(Colors.Blue.Darken1);
                                        });
                                    });
                                });

                                int timpTotalCombinant = dto.DurataTotala + dto.Locatii.Sum(l => l.TimpEstimativ ?? 0);
                                summary.Item().PaddingTop(15).Background(Colors.Orange.Lighten2)
                                    .Padding(12).AlignCenter()
                                    .Row(totalRow =>
                                    {
                                        totalRow.AutoItem().Text("🎯 ").FontSize(18);
                                        totalRow.AutoItem().Text($"TIMP TOTAL ESTIMAT: {FormatTimp(timpTotalCombinant)}")
                                            .FontSize(16).Bold().FontColor(Colors.Orange.Darken1);
                                    });
                            });
                    });

                    page.Footer().Height(40).Background(Colors.Grey.Lighten2).Padding(10)
                    .Row(footerRow =>
                    {
                        footerRow.RelativeItem().AlignLeft()
                            .Text($"Generat pe {DateTime.Now:dd/MM/yyyy la HH:mm}")
                            .FontSize(10).FontColor(Colors.Grey.Darken1);

                        footerRow.RelativeItem().AlignCenter().Text(x =>
                        {
                            x.Span("Pagina ").FontSize(10).FontColor(Colors.Grey.Darken1);
                            x.CurrentPageNumber().FontSize(10).FontColor(Colors.Grey.Darken1);
                        });

                    });

                });
            });

            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/pdf", $"traseu_{traseuId}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Eroare la generarea PDF-ului", details = ex.Message });
        }
    }
}