using Licenta.Models;
using Licenta.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Licenta.Data.Seeding
{
    public static class SeedVladTepes
    {
        public static void Seed(LicentaDbContext context)
        {
            if (context.Trasee.Any(t => t.DenumireTraseu == "Traseul lui Vlad Țepeș")) return;

            // 1. Tematica
            var tematica = context.Tematici.FirstOrDefault(t => t.Denumire == "Vlad Țepeș");
            if (tematica == null)
            {
                tematica = new Tematica { Denumire = "Vlad Țepeș", Tip = "Istoric" };
                context.Tematici.Add(tematica);
                context.SaveChanges();
            }

            // 2. Tari
            var romania = context.Tari.FirstOrDefault(t => t.Denumire == "România");
            if (romania == null)
            {
                romania = new Tara { Denumire = "România" };
                context.Tari.Add(romania);
                context.SaveChanges();
            }

            var turcia = context.Tari.FirstOrDefault(t => t.Denumire == "Turcia");
            if (turcia == null)
            {
                turcia = new Tara { Denumire = "Turcia" };
                context.Tari.Add(turcia);
                context.SaveChanges();
            }

            // 3. Orase
            string[] oraseRom = { "Sighișoara", "Târgoviște", "Curtea de Argeș", "Brașov", "Giurgiu", "Bran", "București", "Snagov" };
            string orasTurcia = "Istanbul";
            var orase = new List<Oras>();

            foreach (var nume in oraseRom)
            {
                var oras = context.Orase.FirstOrDefault(o => o.Denumire == nume && o.TaraId == romania.IdTara);
                if (oras == null)
                {
                    oras = new Oras { Denumire = nume, TaraId = romania.IdTara };
                    context.Orase.Add(oras);
                    context.SaveChanges();
                }
                orase.Add(oras);
            }

            var istanbul = context.Orase.FirstOrDefault(o => o.Denumire == orasTurcia && o.TaraId == turcia.IdTara);
            if (istanbul == null)
            {
                istanbul = new Oras { Denumire = orasTurcia, TaraId = turcia.IdTara };
                context.Orase.Add(istanbul);
                context.SaveChanges();
            }
            orase.Add(istanbul);

            // 4. Locatii
            var locatii = new List<Locatie>
            {
                new Locatie { Denumire = "Casa Vlad Dracul", TipLocatie = "Monument", Geolocatie = "46.219549210635314, 24.79285269702005", TimpEstimativ = 40, Descriere = "Locul nașterii lui Vlad Țepeș, în inima cetății medievale Sighișoara.", OrasId = orase[0].IdOras, Strada = "Strada Cositorarilor", NumarStrada = "5" },
                new Locatie { Denumire = "Cetatea Yedikule", TipLocatie = "Fortăreață", Geolocatie = "40.99308639588452, 28.923148819826142", TimpEstimativ = 60, Descriere = "Fortăreața otomană unde Vlad Țepeș a fost prizonier. Include muzeu și turnuri.", OrasId = orase[8].IdOras, Strada = "Yedikule Meydanı Sokak", NumarStrada = "9" },
                new Locatie { Denumire = "Curtea Domnească (Turnul Chindiei)", TipLocatie = "Palat", Geolocatie = "44.93209380931805, 25.45865707526604", TimpEstimativ = 60, Descriere = "Reședința domnească a lui Vlad Țepeș și loc de execuții publice.", OrasId = orase[1].IdOras, Strada = "Strada Calea Domnească", NumarStrada = "181" },
                new Locatie { Denumire = "Cetatea Poenari", TipLocatie = "Cetate", Geolocatie = "45.35383236708806, 24.6350668393969", TimpEstimativ = 90, Descriere = "Refugiu fortificat al lui Vlad Țepeș, accesibil după 1480 de trepte.", OrasId = orase[2].IdOras, Strada = "Transfăgărășan", NumarStrada = "0" },
                new Locatie { Denumire = "Piața Sfatului", TipLocatie = "Piață", Geolocatie = "45.642148540566616, 25.58916842634904", TimpEstimativ = 30, Descriere = "Locul conflictului cu negustorii sași și al execuțiilor publice.", OrasId = orase[3].IdOras, Strada = "Strada Piața Sfatului", NumarStrada = "1" },
                new Locatie { Denumire = "Cetatea Giurgiu", TipLocatie = "Cetate", Geolocatie = "43.88361,25.96182", TimpEstimativ = 30, Descriere = "Fortificație medievală cucerită de Vlad Țepeș în 1461.", OrasId = orase[4].IdOras, Strada = "Strada Mircea Cel Bătrân", NumarStrada = "0" },
                new Locatie { Denumire = "Castelul Bran", TipLocatie = "Castel", Geolocatie = "45.514903580694416, 25.36716034421876", TimpEstimativ = 60, Descriere = "Castel legendar asociat cu Dracula, deși legătura istorică e incertă.", OrasId = orase[5].IdOras, Strada = "Strada General Traian Moșoiu", NumarStrada = "24" },
                new Locatie { Denumire = "Parcul Carol I", TipLocatie = "Parc", Geolocatie = "44.417631170164064, 26.09547972071723", TimpEstimativ = 30, Descriere = "Loc simbolic pentru comemorări, asociat cu mitul luptei finale.", OrasId = orase[6].IdOras, Strada = "Bulevardul Mărășești", NumarStrada = "1" },
                new Locatie { Denumire = "Mănăstirea Snagov", TipLocatie = "Mănăstire", Geolocatie = "44.72957108186303, 26.17569581044189", TimpEstimativ = 45, Descriere = "Locul presupus al înmormântării lui Vlad Țepeș, pe o insulă izolată.", OrasId = orase[7].IdOras, Strada = "Strada Mănăstirea Vlad Țepeș", NumarStrada = "0" }
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
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Casa Vlad Dracul").IdLocatie, NumeImagine = "Casa Vlad Dracul", Cale = "/Images/VladTepes/Casa-Vlad-Dracul.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Cetatea Yedikule").IdLocatie, NumeImagine = "Cetatea Yedikule", Cale = "/Images/VladTepes/Cetatea-Yedikule.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Curtea Domnească (Turnul Chindiei)").IdLocatie, NumeImagine = "Curtea Domnească (Turnul Chindiei)", Cale = "/Images/VladTepes/Turnul-Chindiei.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Cetatea Poenari").IdLocatie, NumeImagine = "Cetatea Poenari", Cale = "/Images/VladTepes/Cetatea-Poenari.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Piața Sfatului").IdLocatie, NumeImagine = "Piața Sfatului", Cale = "/Images/VladTepes/Piata-Sfatului.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Cetatea Giurgiu").IdLocatie, NumeImagine = "Cetatea Giurgiu", Cale = "/Images/VladTepes/Cetatea-Giurgiu.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Castelul Bran").IdLocatie, NumeImagine = "Castelul Bran", Cale = "/Images/VladTepes/Castelul-Bran.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Parcul Carol I").IdLocatie, NumeImagine = "Parcul Carol I", Cale = "/Images/VladTepes/Parcul-Carol.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Mănăstirea Snagov").IdLocatie, NumeImagine = "Mănăstirea Snagov", Cale = "/Images/VladTepes/Manastirea-Snagov.jpg" }
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
                DenumireTraseu = "Traseul lui Vlad Țepeș",
                Descriere = "Explorare istorică a locurilor marcante din viața lui Vlad Țepeș.",
                DurataEstimata = 445,
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
