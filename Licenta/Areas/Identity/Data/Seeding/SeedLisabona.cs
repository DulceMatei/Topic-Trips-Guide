using Licenta.Models;
using Licenta.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Licenta.Data.Seeding
{
    public static class SeedLisabonaCulinar
    {
        public static void Seed(LicentaDbContext context)
        {
            if (context.Trasee.Any(t => t.DenumireTraseu == "Traseu Culinar - Lisabona")) return;

            // 1. Tematica
            var tematica = context.Tematici.FirstOrDefault(t => t.Denumire == "Gastronomie Lisabona");
            if (tematica == null)
            {
                tematica = new Tematica { Denumire = "Gastronomie Lisabona", Tip = "Culinar" };
                context.Tematici.Add(tematica);
                context.SaveChanges();
            }

            // 2. Tara
            var portugalia = context.Tari.FirstOrDefault(t => t.Denumire == "Portugalia");
            if (portugalia == null)
            {
                portugalia = new Tara { Denumire = "Portugalia" };
                context.Tari.Add(portugalia);
                context.SaveChanges();
            }

            // 3. Oras
            var lisabona = context.Orase.FirstOrDefault(o => o.Denumire == "Lisabona" && o.TaraId == portugalia.IdTara);
            if (lisabona == null)
            {
                lisabona = new Oras { Denumire = "Lisabona", TaraId = portugalia.IdTara };
                context.Orase.Add(lisabona);
                context.SaveChanges();
            }

            // 4. Locatii
            var locatii = new List<Locatie>
            {
                new Locatie{Denumire="Pastéis de Belém",TipLocatie="Cofetărie",Geolocatie="38.69755017263333, -9.203058641012666",TimpEstimativ=30,Descriere="Cofetărie istorică din 1837, faimoasă pentru pastéis de nata originali preparați după rețetă secretă.",OrasId=lisabona.IdOras,Strada="Rua de Belém",NumarStrada="84-92"},
                new Locatie{Denumire="Manteigaria",TipLocatie="Patiserie",Geolocatie="38.71077344349053, -9.143965249238718",TimpEstimativ=20,Descriere="Rival modern al Belém, cu pastéis de nata coapte live și servite calde, în mai multe locații.",OrasId=lisabona.IdOras,Strada="Rua do Loreto",NumarStrada="2"},
                new Locatie{Denumire="Fábrica da Nata",TipLocatie="Patiserie",Geolocatie="38.71585796902785, -9.140861821372388",TimpEstimativ=25,Descriere="Pastéis de nata preparați live în vitrină, servite cu scorțișoară și zahăr pudră.",OrasId=lisabona.IdOras,Strada="Praça dos Restauradores",NumarStrada="62-68"},
                new Locatie{Denumire="Confeitaria Nacional",TipLocatie="Cofetărie",Geolocatie="38.71321225937361, -9.13785888737101",TimpEstimativ=30,Descriere="Fondată în 1829, oferă prăjituri tradiționale portugheze precum Bolo Rei și Meias Luas.",OrasId=lisabona.IdOras,Strada="Praça da Figueira",NumarStrada="18B"},
                new Locatie{Denumire="Pastelaria Versailles",TipLocatie="Cafenea",Geolocatie="38.735625733125495, -9.145311397435787",TimpEstimativ=40,Descriere="Patiserie cu decor Art Nouveau, renumită pentru prăjituri fine și ambient clasic.",OrasId=lisabona.IdOras,Strada="Avenida da República",NumarStrada="15A"},
                new Locatie{Denumire="Pastelaria Aloma",TipLocatie="Patiserie",Geolocatie="38.71736234885218, -9.166547816785842",TimpEstimativ=20,Descriere="Patiserie premiată de patru ori pentru cel mai bun pastel de nata din Lisabona.",OrasId=lisabona.IdOras,Strada="Rua Francisco Metrass",NumarStrada="67A"},
                new Locatie{Denumire="Chocolataria Equador",TipLocatie="Ciocolaterie",Geolocatie="38.71019301775567, -9.136739882861388",TimpEstimativ=25,Descriere="Ciocolaterie artizanală cu sortimente exotice, trufe și tablete inspirate de cacao de origine.",OrasId=lisabona.IdOras,Strada="Rua da Prata",NumarStrada="97"},
                new Locatie{Denumire="Santini Lisboa",TipLocatie="Gelaterie",Geolocatie="38.7125831480052, -9.139028732711866",TimpEstimativ=20,Descriere="Gelaterie artizanală fondată în 1949, cu arome locale precum portocală, smochine și vin de Porto.",OrasId=lisabona.IdOras,Strada="Rua do Carmo",NumarStrada="9"},
                new Locatie{Denumire="Landeau Chocolate",TipLocatie="Ciocolaterie",Geolocatie="38.70915737101754, -9.143486104819443",TimpEstimativ=30,Descriere="Cafenea minimalistă celebră pentru tortul de ciocolată cu rețetă artizanală, considerat de unii cel mai bun din lume.",OrasId=lisabona.IdOras,Strada="Rua das Flores",NumarStrada="70"}
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
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Pastéis de Belém").IdLocatie, NumeImagine = "Pastéis de Belém", Cale = "/Images/LisabonaCulinar/AntigaConfeitaria.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Manteigaria").IdLocatie, NumeImagine = "Manteigaria", Cale = "/Images/LisabonaCulinar/Manteigaria.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Fábrica da Nata").IdLocatie, NumeImagine = "Fábrica da Nata", Cale = "/Images/LisabonaCulinar/TimeOutMarket.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Confeitaria Nacional").IdLocatie, NumeImagine = "Confeitaria Nacional", Cale = "/Images/LisabonaCulinar/CasaDoAlentejo.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Pastelaria Versailles").IdLocatie, NumeImagine = "Pastelaria Versailles", Cale = "/Images/LisabonaCulinar/AGinjinha.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Pastelaria Aloma").IdLocatie, NumeImagine = "Pastelaria Aloma", Cale = "/Images/LisabonaCulinar/OTrevo.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Chocolataria Equador").IdLocatie, NumeImagine = "Chocolataria Equador", Cale = "/Images/LisabonaCulinar/ManteigariaSilva.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Santini Lisboa").IdLocatie, NumeImagine = "Santini Lisboa", Cale = "/Images/LisabonaCulinar/Santini.jpg" },
                new Imagine { LocatieId = toateLocatiile.First(l => l.Denumire == "Landeau Chocolate").IdLocatie, NumeImagine = "Landeau Chocolate", Cale = "/Images/LisabonaCulinar/Landeau.jpg" }
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
                DenumireTraseu = "Traseu Culinar - Lisabona",
                Descriere = "Descoperă gustul autentic al Lisabonei prin celebrele pastéis de nata, ciocolaterii artizanale și patiserii istorice.",
                DurataEstimata = 240,
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