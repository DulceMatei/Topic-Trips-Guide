using Licenta.Models;
using Licenta.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Licenta.Data.Seeding
{
    public static class SeedBolognaCulinar
    {
        public static void Seed(LicentaDbContext context)
        {
            if (context.Trasee.Any(t => t.DenumireTraseu == "Traseu Culinar - Bologna")) return;

            // 1. Tematica
            var tematica = context.Tematici.FirstOrDefault(t => t.Denumire == "Gastronomie Bologna");
            if (tematica == null)
            {
                tematica = new Tematica { Denumire = "Gastronomie Bologna", Tip = "Culinar" };
                context.Tematici.Add(tematica);
                context.SaveChanges();
            }

            // 2. Tara
            var italia = context.Tari.FirstOrDefault(t => t.Denumire == "Italia");
            if (italia == null)
            {
                italia = new Tara { Denumire = "Italia" };
                context.Tari.Add(italia);
                context.SaveChanges();
            }

            // 3. Oras
            var bologna = context.Orase.FirstOrDefault(o => o.Denumire == "Bologna" && o.TaraId == italia.IdTara);
            if (bologna == null)
            {
                bologna = new Oras { Denumire = "Bologna", TaraId = italia.IdTara };
                context.Orase.Add(bologna);
                context.SaveChanges();
            }

            // 4. Locatii
            var locatii = new List<Locatie>
            {
                new Locatie{Denumire="Sfoglia Rina",TipLocatie="Restaurant",Geolocatie="44.493195849406355, 11.346375155837542",TimpEstimativ=45,Descriere="Punct de referință pentru paste proaspete artizanale: tagliatelle, tortellini, lasagna. Atmosferă caldă și modernă.",OrasId=bologna.IdOras,Strada="Via Castiglione",NumarStrada="5/B"},
                new Locatie{Denumire="Osteria dell'Orsa",TipLocatie="Osterie",Geolocatie="44.497152719931776, 11.347595738659331",TimpEstimativ=60,Descriere="Osterie populară printre localnici și studenți, renumită pentru tagliatelle al ragù bolognese.",OrasId=bologna.IdOras,Strada="Via Mentana",NumarStrada="1/F"},
                new Locatie{Denumire="Trattoria di Via Serra",TipLocatie="Trattorie",Geolocatie="44.50950771160455, 11.345720178538336",TimpEstimativ=75,Descriere="Trattorie premiată cu Bib Gourmand, cunoscută pentru tortelloni cu ricotta și ingrediente Slow Food.",OrasId=bologna.IdOras,Strada="Via Luigi Serra",NumarStrada="9/B"},
                new Locatie{Denumire="La Vecchia Scuola Bolognese",TipLocatie="Școală de gătit",Geolocatie="44.4923010225369, 11.314847083319162",TimpEstimativ=90,Descriere="Atelier fondat de Alessandra Spisni, dedicat pastelor tradiționale bologneze făcute manual.",OrasId=bologna.IdOras,Strada="Via del Partigiano",NumarStrada="7"},
                new Locatie{Denumire="Drogheria della Rosa",TipLocatie="Restaurant",Geolocatie="44.48999832449574, 11.350041430092753",TimpEstimativ=75,Descriere="Restaurant cu paste lucrate manual și preparate cu trufe în sezon. Atmosferă boemă.",OrasId=bologna.IdOras,Strada="Via Cartoleria",NumarStrada="10B"},
                new Locatie{Denumire="Tamburini",TipLocatie="Delicatese & Restaurant",Geolocatie="44.493909474711344, 11.345850921496872",TimpEstimativ=60,Descriere="Salumerie istorică și restaurant cu paste, brânzeturi și mezeluri emiliene. Ideal pentru prânzuri rapide.",OrasId=bologna.IdOras,Strada="Via Caprarie",NumarStrada="1"},
                new Locatie{Denumire="Trattoria Anna Maria",TipLocatie="Trattorie",Geolocatie="44.49775171879861, 11.349247481597207",TimpEstimativ=75,Descriere="Trattorie tradițională cu paste făcute în casă, decor vintage și rețete autentice din Emilia-Romagna.",OrasId=bologna.IdOras,Strada="Via delle Belle Arti",NumarStrada="17/A"},
                new Locatie{Denumire="Pasta Fresca Naldi",TipLocatie="Restaurant",Geolocatie="44.49559365331754, 11.330969798780805",TimpEstimativ=20,Descriere="Mică afacere locală cu paste proaspete la pachet. Balanzoni, tortellini și gramigna la porție.",OrasId=bologna.IdOras,Strada="Via del Pratello",NumarStrada="69"},
                new Locatie{Denumire="Ristorante Da Cesarina",TipLocatie="Restaurant",Geolocatie="44.49199997214539, 11.348436800624775",TimpEstimativ=90,Descriere="Restaurant elegant în Piazza Santo Stefano, renumit pentru tagliolini și lasagna alla bolognese.",OrasId=bologna.IdOras,Strada="Via Santo Stefano",NumarStrada="19/B"}
            };

            foreach (var locatie in locatii)
            {
                if (!context.Locatii.Any(l => l.Denumire == locatie.Denumire && l.Geolocatie == locatie.Geolocatie))
                {
                    context.Locatii.Add(locatie);
                }
            }
            context.SaveChanges();

            // 5. Imagini pentru locatii
            var toateLocatiile = context.Locatii.ToList();
            var imagini = new List<Imagine>
            {
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Sfoglia Rina").IdLocatie, NumeImagine = "Sfoglia Rina", Cale = "/Images/BolognaCulinar/SfogliaRina.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Osteria dell'Orsa").IdLocatie, NumeImagine = "Osteria dell'Orsa", Cale = "/Images/BolognaCulinar/OsteriaDellOrsa.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Trattoria di Via Serra").IdLocatie, NumeImagine = "Trattoria di Via Serra", Cale = "/Images/BolognaCulinar/TrattoriaDiViaSerra.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "La Vecchia Scuola Bolognese").IdLocatie, NumeImagine = "La Vecchia Scuola Bolognese", Cale = "/Images/BolognaCulinar/LaVecchiaScuola.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Drogheria della Rosa").IdLocatie, NumeImagine = "Drogheria della Rosa", Cale = "/Images/BolognaCulinar/DrogheriaDellaRosa.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Tamburini").IdLocatie, NumeImagine = "Tamburini", Cale = "/Images/BolognaCulinar/Tamburini.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Trattoria Anna Maria").IdLocatie, NumeImagine = "Trattoria Anna Maria", Cale = "/Images/BolognaCulinar/TrattoriaAnnaMaria.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Pasta Fresca Naldi").IdLocatie, NumeImagine = "Pasta Fresca Naldi", Cale = "/Images/BolognaCulinar/PastaFrescaNaldi.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Ristorante Da Cesarina").IdLocatie, NumeImagine = "Ristorante Da Cesarina", Cale = "/Images/BolognaCulinar/DaCesarina.jpg" }
            };

            foreach (var img in imagini)
            {
                if (!context.Imagini.Any(i => i.Cale == img.Cale && i.LocatieId == img.LocatieId))
                {
                    context.Imagini.Add(img);
                }
            }
            context.SaveChanges();

            // 6. LocatiiTematice
            var locatiiTematice = locatii.Select(l => new LocatieTematica
            {
                LocatieId = context.Locatii.First(x => x.Denumire == l.Denumire).IdLocatie,
                TematicaId = tematica.IdTematica
            }).ToList();

            foreach (var lt in locatiiTematice)
            {
                if (!context.LocatiiTematice.Any(x => x.LocatieId == lt.LocatieId && x.TematicaId == lt.TematicaId))
                {
                    context.LocatiiTematice.Add(lt);
                }
            }
            context.SaveChanges();

            // 7. Traseu
            var systemUser = context.Users.FirstOrDefault(u => u.Email == "system@topictrips.com");
            var traseu = new Traseu
            {
                DenumireTraseu = "Traseu Culinar - Bologna",
                Descriere = "Explorează inima gastronomiei italiene prin paste artizanale, trattorii tradiționale și delicatese autentice din Bologna.",
                DurataEstimata = 590,
                DataCreare = DateTime.Now,
                TematicaId = tematica.IdTematica,
                UtilizatorId = systemUser?.Id
            };
            context.Trasee.Add(traseu);
            context.SaveChanges();

            // 8. Itinerar
            int ordine = 1;
            foreach (var lt in locatiiTematice)
            {
                var idLocatieTematica = context.LocatiiTematice.First(x => x.LocatieId == lt.LocatieId && x.TematicaId == lt.TematicaId).IdLocatieTematica;

                context.Itinerarii.Add(new Itinerar
                {
                    TraseuId = traseu.IdTraseu,
                    LocatieTematicaId = idLocatieTematica,
                    NrOrdine = ordine++
                });
            }
            context.SaveChanges();
        }
    }
}