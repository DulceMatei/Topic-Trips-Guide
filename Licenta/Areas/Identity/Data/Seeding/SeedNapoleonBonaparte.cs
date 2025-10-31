using Licenta.Models;
using Licenta.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Licenta.Data.Seeding
{
    public static class SeedNapoleon
    {
        public static void Seed(LicentaDbContext context)
        {
            if (context.Trasee.Any(t => t.DenumireTraseu == "Traseul lui Napoleon Bonaparte")) return;

            // 1. Tematica
            var tematica = context.Tematici.FirstOrDefault(t => t.Denumire == "Napoleon Bonaparte");
            if (tematica == null)
            {
                tematica = new Tematica { Denumire = "Napoleon Bonaparte", Tip = "Istoric" };
                context.Tematici.Add(tematica);
                context.SaveChanges();
            }

            // 2. Tari
            var franta = context.Tari.FirstOrDefault(t => t.Denumire == "Franța");
            if (franta == null)
            {
                franta = new Tara { Denumire = "Franța" };
                context.Tari.Add(franta);
                context.SaveChanges();
            }

            var cehia = context.Tari.FirstOrDefault(t => t.Denumire == "Cehia");
            if (cehia == null)
            {
                cehia = new Tara { Denumire = "Cehia" };
                context.Tari.Add(cehia);
                context.SaveChanges();
            }

            var germania = context.Tari.FirstOrDefault(t => t.Denumire == "Germania");
            if (germania == null)
            {
                germania = new Tara { Denumire = "Germania" };
                context.Tari.Add(germania);
                context.SaveChanges();
            }

            var italia = context.Tari.FirstOrDefault(t => t.Denumire == "Italia");
            if (italia == null)
            {
                italia = new Tara { Denumire = "Italia" };
                context.Tari.Add(italia);
                context.SaveChanges();
            }

            var belgia = context.Tari.FirstOrDefault(t => t.Denumire == "Belgia");
            if (belgia == null)
            {
                belgia = new Tara { Denumire = "Belgia" };
                context.Tari.Add(belgia);
                context.SaveChanges();
            }

            // 3. Orase
            string[] oraseFranta = { "Ajaccio", "Brienne-le-Château", "Toulon", "Paris" };
            string orasCehia = "Austerlitz";
            string orasGermania = "Leipzig";
            string orasItalia = "Elba";
            string orasBelgia = "Waterloo";
            var orase = new List<Oras>();

            foreach (var nume in oraseFranta)
            {
                var oras = context.Orase.FirstOrDefault(o => o.Denumire == nume && o.TaraId == franta.IdTara);
                if (oras == null)
                {
                    oras = new Oras { Denumire = nume, TaraId = franta.IdTara };
                    context.Orase.Add(oras);
                    context.SaveChanges();
                }
                orase.Add(oras);
            }

            var austerlitz = context.Orase.FirstOrDefault(o => o.Denumire == orasCehia && o.TaraId == cehia.IdTara);
            if (austerlitz == null)
            {
                austerlitz = new Oras { Denumire = orasCehia, TaraId = cehia.IdTara };
                context.Orase.Add(austerlitz);
                context.SaveChanges();
            }
            orase.Add(austerlitz);

            var leipzig = context.Orase.FirstOrDefault(o => o.Denumire == orasGermania && o.TaraId == germania.IdTara);
            if (leipzig == null)
            {
                leipzig = new Oras { Denumire = orasGermania, TaraId = germania.IdTara };
                context.Orase.Add(leipzig);
                context.SaveChanges();
            }
            orase.Add(leipzig);

            var elba = context.Orase.FirstOrDefault(o => o.Denumire == orasItalia && o.TaraId == italia.IdTara);
            if (elba == null)
            {
                elba = new Oras { Denumire = orasItalia, TaraId = italia.IdTara };
                context.Orase.Add(elba);
                context.SaveChanges();
            }
            orase.Add(elba);

            var waterloo = context.Orase.FirstOrDefault(o => o.Denumire == orasBelgia && o.TaraId == belgia.IdTara);
            if (waterloo == null)
            {
                waterloo = new Oras { Denumire = orasBelgia, TaraId = belgia.IdTara };
                context.Orase.Add(waterloo);
                context.SaveChanges();
            }
            orase.Add(waterloo);

            // 4. Locatii
            var locatii = new List<Locatie>
            {
                new Locatie { Denumire = "Casa natală Napoleon Ajaccio", TipLocatie = "Muzeu", Geolocatie = "41.91811204515574, 8.738696581195901", TimpEstimativ = 45, Descriere = "Locul nașterii lui Napoleon Bonaparte, astăzi muzeu dedicat familiei Bonaparte.", OrasId = orase[0].IdOras, Strada = "Rue Saint-Charles", NumarStrada = "1" },
                new Locatie { Denumire = "Școala Militară Brienne-le-Château", TipLocatie = "Școală", Geolocatie = "48.3889107472784, 4.528295278141607", TimpEstimativ = 30, Descriere = "Fosta școală militară unde Napoleon a studiat între 1779–1784, azi muzeu.", OrasId = orase[1].IdOras, Strada = "Rue de l'École Militaire", NumarStrada = "34" },
                new Locatie { Denumire = "Musée Balaguier (Fortul Toulon)", TipLocatie = "Muzeu", Geolocatie = "43.09442590797532, 5.910944837526711", TimpEstimativ = 40, Descriere = "Locul primei sale victorii militare importante în 1793, în timpul asediului Toulonului.", OrasId = orase[2].IdOras, Strada = "Avenue Jean-Baptiste Ivaldi", NumarStrada = "1" },
                new Locatie { Denumire = "Catedrala Notre-Dame Paris", TipLocatie = "Catedrală", Geolocatie = "48.85312586119454, 2.3499301836314026", TimpEstimativ = 30, Descriere = "Locul încoronării lui Napoleon ca Împărat al Franței în 1804.", OrasId = orase[3].IdOras, Strada = "6 Parvis Notre-Dame", NumarStrada = "6" },
                new Locatie { Denumire = "Câmpul de bătălie Austerlitz", TipLocatie = "Câmp de bătălie", Geolocatie = "49.12819131479285, 16.7626678848678", TimpEstimativ = 60, Descriere = "Locul marii victorii napoleoniene din 1805, cunoscută ca Bătălia celor Trei Împărați.", OrasId = orase[4].IdOras, Strada = "Slavkov u Brna", NumarStrada = "0" },
                new Locatie { Denumire = "Monumentul Bătăliei Leipzig", TipLocatie = "Monument", Geolocatie = "51.31239821510665, 12.413469386281433", TimpEstimativ = 60, Descriere = "Monumentul Völkerschlachtdenkmal comemorează înfrângerea lui Napoleon în 1813.", OrasId = orase[5].IdOras, Strada = "Straße des 18. Oktober", NumarStrada = "100" },
                new Locatie { Denumire = "Vila San Martino Elba", TipLocatie = "Vila", Geolocatie = "42.785539800726525, 10.280273901239612", TimpEstimativ = 45, Descriere = "Reședința lui Napoleon în exil pe insula Elba, transformată în muzeu.", OrasId = orase[6].IdOras, Strada = "Località San Martino", NumarStrada = "1" },
                new Locatie { Denumire = "Câmpul de bătălie Waterloo", TipLocatie = "Câmp de bătălie", Geolocatie = "50.6798903217227, 4.403476403105691", TimpEstimativ = 90, Descriere = "Locul ultimei bătălii a lui Napoleon din 1815, cu muzeu, panoramă și Butte du Lion.", OrasId = orase[7].IdOras, Strada = "Route du Lion", NumarStrada = "1815" },
                new Locatie { Denumire = "Les Invalides Paris", TipLocatie = "Mausoleu", Geolocatie = "48.85509190154234, 2.3124353068406993", TimpEstimativ = 60, Descriere = "Mormântul lui Napoleon Bonaparte, aflat sub Domul de Aur al Hôtel des Invalides.", OrasId = orase[3].IdOras, Strada = "Rue de Grenelle", NumarStrada = "129" }
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
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Casa natală Napoleon Ajaccio").IdLocatie, NumeImagine = "Casa natală Napoleon Ajaccio", Cale = "/Images/Napoleon/Ajaccio.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Școala Militară Brienne-le-Château").IdLocatie, NumeImagine = "Școala Militară Brienne-le-Château", Cale = "/Images/Napoleon/Brienne.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Musée Balaguier (Fortul Toulon)").IdLocatie, NumeImagine = "Musée Balaguier (Fortul Toulon)", Cale = "/Images/Napoleon/Toulon.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Catedrala Notre-Dame Paris").IdLocatie, NumeImagine = "Catedrala Notre-Dame Paris", Cale = "/Images/Napoleon/NotreDame.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Câmpul de bătălie Austerlitz").IdLocatie, NumeImagine = "Câmpul de bătălie Austerlitz", Cale = "/Images/Napoleon/Austerlitz.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Monumentul Bătăliei Leipzig").IdLocatie, NumeImagine = "Monumentul Bătăliei Leipzig", Cale = "/Images/Napoleon/Leipzig.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Vila San Martino Elba").IdLocatie, NumeImagine = "Vila San Martino Elba", Cale = "/Images/Napoleon/Elba.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Câmpul de bătălie Waterloo").IdLocatie, NumeImagine = "Câmpul de bătălie Waterloo", Cale = "/Images/Napoleon/Waterloo.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Les Invalides Paris").IdLocatie, NumeImagine = "Les Invalides Paris", Cale = "/Images/Napoleon/LesInvalides.jpg" }
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
                DenumireTraseu = "Traseul lui Napoleon Bonaparte",
                Descriere = "O incursiune în viața militară și politică a lui Napoleon Bonaparte.",
                DurataEstimata = 480,
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