using Licenta.Models;
using Licenta.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Licenta.Data.Seeding
{
    public static class SeedLyonCulinar
    {
        public static void Seed(LicentaDbContext context)
        {
            if (context.Trasee.Any(t => t.DenumireTraseu == "Traseu Culinar - Lyon")) return;

            // 1. Tematica
            var tematica = context.Tematici.FirstOrDefault(t => t.Denumire == "Gastronomie Lyon");
            if (tematica == null)
            {
                tematica = new Tematica { Denumire = "Gastronomie Lyon", Tip = "Culinar" };
                context.Tematici.Add(tematica);
                context.SaveChanges();
            }

            // 2. Tara
            var franta = context.Tari.FirstOrDefault(t => t.Denumire == "Franța");
            if (franta == null)
            {
                franta = new Tara { Denumire = "Franța" };
                context.Tari.Add(franta);
                context.SaveChanges();
            }

            // 3. Oras
            var lyon = context.Orase.FirstOrDefault(o => o.Denumire == "Lyon" && o.TaraId == franta.IdTara);
            if (lyon == null)
            {
                lyon = new Oras { Denumire = "Lyon", TaraId = franta.IdTara };
                context.Orase.Add(lyon);
                context.SaveChanges();
            }

            // 4. Locatii
            var locatii = new List<Locatie>
            {
                new Locatie{Denumire="Les Halles de Lyon – Paul Bocuse",TipLocatie="Piață gastronomică",Geolocatie="45.762935020464965, 4.851026021550611",TimpEstimativ=60,Descriere="Piață emblematică cu produse gourmet și brânzeturi locale precum Saint-Marcellin și Saint-Félicien.",OrasId=lyon.IdOras,Strada="Cours Lafayette",NumarStrada="102"},
                new Locatie{Denumire="Café des Fédérations",TipLocatie="Bouchon",Geolocatie="45.76656351066607, 4.832607087183428",TimpEstimativ=75,Descriere="Bouchon tradițional din 1872, renumit pentru salata lyonnaise cu bacon crocant și ou poșat.",OrasId=lyon.IdOras,Strada="Rue Major Martin",NumarStrada="8"},
                new Locatie{Denumire="Le Garet",TipLocatie="Bouchon",Geolocatie="45.766949222836935, 4.836964202713851",TimpEstimativ=75,Descriere="Bouchon rustic autentic, celebru pentru andouillette și cervelle de canut.",OrasId=lyon.IdOras,Strada="Rue du Garet",NumarStrada="7"},
                new Locatie{Denumire="La Mère Brazier",TipLocatie="Restaurant Michelin",Geolocatie="45.7714572544691, 4.837207013991493",TimpEstimativ=120,Descriere="Restaurant cu 2 stele Michelin, fondat în 1921, ce reinterpretează preparatele tradiționale lyonnaise.",OrasId=lyon.IdOras,Strada="Rue Royale",NumarStrada="12"},
                new Locatie{Denumire="Maison Bouillet",TipLocatie="Patiserie",Geolocatie="45.77606959417449, 4.8328788218235585",TimpEstimativ=30,Descriere="Patiserie artizanală celebră pentru tartele cu praline rose și creațiile lui Sébastien Bouillet.",OrasId=lyon.IdOras,Strada="Place de la Croix-Rousse",NumarStrada="15"},
                new Locatie{Denumire="Fromagerie Galland",TipLocatie="Fromagerie",Geolocatie="45.77599565805576, 4.833455735940722",TimpEstimativ=20,Descriere="Mică fromagerie de cartier cu selecție atentă de brânzeturi locale și regionale.",OrasId=lyon.IdOras,Strada="Rue d'Austerlitz",NumarStrada="8"},
                new Locatie{Denumire="Fromagerie Mons",TipLocatie="Fromagerie",Geolocatie="45.75171681608801, 4.831249873687443",TimpEstimativ=25,Descriere="Affineur celebru cu selecție maturată în tuneluri proprii; prezent în Halles Paul Bocuse.",OrasId=lyon.IdOras,Strada="Rue de la Charité",NumarStrada="39"},
                new Locatie{Denumire="Brasserie Georges",TipLocatie="Brasserie",Geolocatie="45.7482845791304, 4.828372313872349",TimpEstimativ=90,Descriere="Brasserie istorică din 1836, renumită pentru choucroute, brânzeturi și vinuri locale.",OrasId=lyon.IdOras,Strada="Cours de Verdun Perrache",NumarStrada="30"},
                new Locatie{Denumire="Bouchon Les Lyonnais",TipLocatie="Bouchon",Geolocatie="45.76187586963673, 4.826199544842352",TimpEstimativ=75,Descriere="Bouchon autentic cu preparate rustice și specialități cu brânză precum gratin dauphinois.",OrasId=lyon.IdOras,Strada="Rue de la Bombarde",NumarStrada="19"}
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
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Les Halles de Lyon – Paul Bocuse").IdLocatie, NumeImagine = "Les Halles de Lyon – Paul Bocuse", Cale = "/Images/LyonCulinar/LesHallesDeLyon.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Café des Fédérations").IdLocatie, NumeImagine = "Café des Fédérations", Cale = "/Images/LyonCulinar/CafeDesFederations.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Le Garet").IdLocatie, NumeImagine = "Le Garet", Cale = "/Images/LyonCulinar/LeGaret.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "La Mère Brazier").IdLocatie, NumeImagine = "La Mère Brazier", Cale = "/Images/LyonCulinar/LaMereBrazier.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Maison Bouillet").IdLocatie, NumeImagine = "Maison Bouillet", Cale = "/Images/LyonCulinar/MaisonBouillet.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Fromagerie Galland").IdLocatie, NumeImagine = "Fromagerie Galland", Cale = "/Images/LyonCulinar/FromagerieGalland.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Fromagerie Mons").IdLocatie, NumeImagine = "Fromagerie Mons", Cale = "/Images/LyonCulinar/FromagerieMons.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Brasserie Georges").IdLocatie, NumeImagine = "Brasserie Georges", Cale = "/Images/LyonCulinar/BrasserieGeorges.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Bouchon Les Lyonnais").IdLocatie, NumeImagine = "Bouchon Les Lyonnais", Cale = "/Images/LyonCulinar/BouchonLesLyonnais.jpg" }
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
                DenumireTraseu = "Traseu Culinar - Lyon",
                Descriere = "Descoperă capitala gastronomiei franceze prin bouchon-uri tradiționale, fromagerii și restaurante iconice din Lyon.",
                DurataEstimata = 570,
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