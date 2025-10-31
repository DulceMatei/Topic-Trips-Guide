using Licenta.Models;
using Licenta.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Licenta.Data.Seeding
{
    public static class SeedOttoVonBismarck
    {
        public static void Seed(LicentaDbContext context)
        {
            if (context.Trasee.Any(t => t.DenumireTraseu == "Traseul lui Otto von Bismarck")) return;

            // 1. Tematica
            var tematica = context.Tematici.FirstOrDefault(t => t.Denumire == "Otto von Bismarck");
            if (tematica == null)
            {
                tematica = new Tematica { Denumire = "Otto von Bismarck", Tip = "Istoric" };
                context.Tematici.Add(tematica);
                context.SaveChanges();
            }

            // 2. Tari
            var germania = context.Tari.FirstOrDefault(t => t.Denumire == "Germania");
            if (germania == null)
            {
                germania = new Tara { Denumire = "Germania" };
                context.Tari.Add(germania);
                context.SaveChanges();
            }

            var cehia = context.Tari.FirstOrDefault(t => t.Denumire == "Cehia");
            if (cehia == null)
            {
                cehia = new Tara { Denumire = "Cehia" };
                context.Tari.Add(cehia);
                context.SaveChanges();
            }

            // 3. Orase
            string[] oraseGer = { "Schönhausen", "Göttingen", "Aachen", "Frankfurt", "Bad Ems", "Berlin", "Bad Kissingen", "Friedrichsruh" };
            string orasCehia = "Königgrätz";
            var orase = new List<Oras>();

            foreach (var nume in oraseGer)
            {
                var oras = context.Orase.FirstOrDefault(o => o.Denumire == nume && o.TaraId == germania.IdTara);
                if (oras == null)
                {
                    oras = new Oras { Denumire = nume, TaraId = germania.IdTara };
                    context.Orase.Add(oras);
                    context.SaveChanges();
                }
                orase.Add(oras);
            }

            var koniggratz = context.Orase.FirstOrDefault(o => o.Denumire == orasCehia && o.TaraId == cehia.IdTara);
            if (koniggratz == null)
            {
                koniggratz = new Oras { Denumire = orasCehia, TaraId = cehia.IdTara };
                context.Orase.Add(koniggratz);
                context.SaveChanges();
            }
            orase.Add(koniggratz);

            // 4. Locatii
            var locatii = new List<Locatie>
            {
                new Locatie { Denumire = "Bismarck-Museum Schönhausen", TipLocatie = "Muzeu", Geolocatie = "52.583036620857634, 12.035811815976833", TimpEstimativ = 45, Descriere = "Locul nașterii lui Otto von Bismarck.", OrasId = orase[0].IdOras, Strada = "Kirchberg", NumarStrada = "4" },
                new Locatie { Denumire = "Universitatea din Göttingen", TipLocatie = "Universitate", Geolocatie = "51.54169595319657, 9.937420763263988", TimpEstimativ = 30, Descriere = "Studiile universitare ale lui Bismarck.", OrasId = orase[1].IdOras, Strada = "Wilhelmsplatz", NumarStrada = "1" },
                new Locatie { Denumire = "Catedrala din Aachen", TipLocatie = "Catedrală", Geolocatie = "50.77481475605256, 6.0845101831337995", TimpEstimativ = 50, Descriere = "Simbol imperial al Europei, relevant pentru contextul politic al epocii lui Bismarck.", OrasId = orase[2].IdOras, Strada = "Domhof", NumarStrada = "1" },
                new Locatie { Denumire = "Römer Frankfurt", TipLocatie = "Primărie", Geolocatie = "50.110586910405296, 8.681783496555585", TimpEstimativ = 40, Descriere = "Reprezentarea lui Bismarck la Dieta Germană.", OrasId = orase[3].IdOras, Strada = "Römerberg", NumarStrada = "27" },
                new Locatie { Denumire = "Monumentul Telegrama de la Ems", TipLocatie = "Monument", Geolocatie = "50.33164325706814, 7.721718341888229", TimpEstimativ = 20, Descriere = "Locul celebrului incident diplomatic ce a dus la declanșarea războiului franco-prusac.", OrasId = orase[4].IdOras, Strada = "Römerstraße", NumarStrada = "1" },
                new Locatie { Denumire = "Situl bătăliei de la Königgrätz", TipLocatie = "Câmp de bătălie", Geolocatie = "50.279014411245946, 15.743303920941893", TimpEstimativ = 60, Descriere = "Locul bătăliei decisive din 1866 între Prusia și Austria.", OrasId = orase[8].IdOras, Strada = "Sadová", NumarStrada = "0" },
                new Locatie { Denumire = "Palatul Reichstag", TipLocatie = "Clădire istorică", Geolocatie = "52.518647303938316, 13.376409209578418", TimpEstimativ = 90, Descriere = "Simbolul unificării Germaniei și al epocii imperiale.", OrasId = orase[5].IdOras, Strada = "Platz der Republik", NumarStrada = "1" },
                new Locatie { Denumire = "Monumentul Bismarck Bad Kissingen", TipLocatie = "Monument", Geolocatie = "50.214859026535194, 10.071911313828", TimpEstimativ = 25, Descriere = "Primul monument dedicat lui Bismarck, ridicat în timpul vieții sale.", OrasId = orase[6].IdOras, Strada = "Salinenpromenade", NumarStrada = "1" },
                new Locatie { Denumire = "Mausoleul Bismarck Friedrichsruh", TipLocatie = "Mausoleu", Geolocatie = "53.527678069327756, 10.336064427517593", TimpEstimativ = 40, Descriere = "Locul de veci și reședința finală a lui Otto von Bismarck.", OrasId = orase[7].IdOras, Strada = "Holzhof", NumarStrada = "1" }
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
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Bismarck-Museum Schönhausen").IdLocatie, NumeImagine = "Bismarck-Museum Schönhausen", Cale = "/Images/OttoVonBismarck/Schönhausen.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Universitatea din Göttingen").IdLocatie, NumeImagine = "Universitatea din Göttingen", Cale = "/Images/OttoVonBismarck/Göttingen.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Catedrala din Aachen").IdLocatie, NumeImagine = "Catedrala din Aachen", Cale = "/Images/OttoVonBismarck/Aachen.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Römer Frankfurt").IdLocatie, NumeImagine = "Römer Frankfurt", Cale = "/Images/OttoVonBismarck/Frankfurt.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Monumentul Telegrama de la Ems").IdLocatie, NumeImagine = "Monumentul Telegrama de la Ems", Cale = "/Images/OttoVonBismarck/BadEms.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Situl bătăliei de la Königgrätz").IdLocatie, NumeImagine = "Situl bătăliei de la Königgrätz", Cale = "/Images/OttoVonBismarck/Koniggratz.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Palatul Reichstag").IdLocatie, NumeImagine = "Palatul Reichstag", Cale = "/Images/OttoVonBismarck/Berlin.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Monumentul Bismarck Bad Kissingen").IdLocatie, NumeImagine = "Monumentul Bismarck Bad Kissingen", Cale = "/Images/OttoVonBismarck/BadKissingen.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Mausoleul Bismarck Friedrichsruh").IdLocatie, NumeImagine = "Mausoleul Bismarck Friedrichsruh", Cale = "/Images/OttoVonBismarck/Friedrichsruh.jpg" }
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
                DenumireTraseu = "Traseul lui Otto von Bismarck",
                Descriere = "O incursiune în viața politică și personală a lui Otto von Bismarck.",
                DurataEstimata = 430,
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