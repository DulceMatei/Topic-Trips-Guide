using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Licenta.Areas.Identity.Data;

namespace Licenta.Models
{
    public class Tematica
    {
        [Key]
        public int IdTematica { get; set; }

        [Required]
        [StringLength(50)]
        public string Denumire { get; set; }

        [Required]
        [StringLength(30)]
        public string Tip { get; set; }

        // Relatii
        public virtual ICollection<Traseu> Trasee { get; set; }
        public virtual ICollection<LocatieTematica> LocatiiTematice { get; set; }
    }

    public class Traseu
    {
        [Key]
        public int IdTraseu { get; set; }

        [Required]
        [StringLength(100)]
        public string DenumireTraseu { get; set; }

        public string? Descriere { get; set; }

        [Required]
        [Range(1, 99999)]
        public int DurataEstimata { get; set; }

        [Required]
        [DateRange]
        public DateTime DataCreare { get; set; }

        // Chei straine
        [Required]
        public string UtilizatorId { get; set; }

        [Required]
        public int TematicaId { get; set; }

        // Relatii
        [ForeignKey("UtilizatorId")]
        public virtual LicentaUser Utilizator { get; set; }

        [ForeignKey("TematicaId")]
        public virtual Tematica Tematica { get; set; }

        public virtual ICollection<Itinerar> Itinerarii { get; set; }
    }

    public class Tara
    {
        [Key]
        public int IdTara { get; set; }

        [Required]
        [StringLength(50)]
        public string Denumire { get; set; }

        // Relatii
        public virtual ICollection<Oras> Orase { get; set; }
    }

    public class Oras
    {
        [Key]
        public int IdOras { get; set; }

        [Required]
        [StringLength(50)]
        public string Denumire { get; set; }

        // Chei straine
        [Required]
        public int TaraId { get; set; }

        // Relatii
        [ForeignKey("TaraId")]
        public virtual Tara Tara { get; set; }

        public virtual ICollection<Locatie> Locatii { get; set; }
    }

    public class Locatie
    {
        [Key]
        public int IdLocatie { get; set; }

        [Required]
        [StringLength(100)]
        public string Denumire { get; set; }

        [Required]
        [StringLength(30)]
        public string TipLocatie { get; set; }

        public string? Descriere { get; set; }

        [Required]
        [Range(1, 1440)] 
        public int TimpEstimativ { get; set; }

        [Required]
        [StringLength(50)]
        public string Geolocatie { get; set; }

        [StringLength(100)]
        public string? Strada { get; set; }

        public string? NumarStrada { get; set; } 

        [Required]
        public int OrasId { get; set; }

        [ForeignKey("OrasId")]
        public virtual Oras Oras { get; set; }

        public virtual ICollection<Imagine> Imagini { get; set; }
        public virtual ICollection<Recenzie> Recenzii { get; set; }
        public virtual ICollection<LocatieTematica> TematiciLocatie { get; set; }
    }

    public class Imagine
    {
        [Key]
        public int IdImagine { get; set; }

        [Required]
        [StringLength(100)]
        public string NumeImagine { get; set; }

        [Required]
        [StringLength(255)]
        public string Cale { get; set; }

        // Chei straine
        [Required]
        public int LocatieId { get; set; }

        // Relatii
        [ForeignKey("LocatieId")]
        public virtual Locatie Locatie { get; set; }
    }

    public class Recenzie
    {
        [Key]
        public int IdRecenzie { get; set; }

        [StringLength(20)]
        public string CodRecenzie { get; set; }

        [Required]
        [DateRange]
        public DateTime DataRecenzie { get; set; }

        [Required(ErrorMessage = "Comentariul este obligatoriu.")]
        public string? Comentariu { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating-ul trebuie să fie între 1 și 5.")]
        public int Rating { get; set; }

        // Chei straine
        [Required]
        public string UtilizatorId { get; set; }

        [Required]
        public int LocatieId { get; set; }

        // Relatii
        [ForeignKey("UtilizatorId")]
        public virtual LicentaUser Utilizator { get; set; }

        [ForeignKey("LocatieId")]
        public virtual Locatie Locatie { get; set; }
    }

    public class LocatieTematica
    {
        [Key]
        public int IdLocatieTematica { get; set; }

        // Chei straine
        [Required]
        public int LocatieId { get; set; }

        [Required]
        public int TematicaId { get; set; }

        // Relatii
        [ForeignKey("LocatieId")]
        public virtual Locatie Locatie { get; set; }

        [ForeignKey("TematicaId")]
        public virtual Tematica Tematica { get; set; }

        public virtual ICollection<Itinerar> Itinerarii { get; set; }
    }

    public class Itinerar
    {
        [Key]
        public int IdItinerar { get; set; }

        [DateRange]
        public DateTime? Data { get; set; }

        [Required]
        [Range(1, 100)]
        public int NrOrdine { get; set; }

        // Chei straine
        [Required]
        public int LocatieTematicaId { get; set; }

        [Required]
        public int TraseuId { get; set; }

        // Relatii
        [ForeignKey("LocatieTematicaId")]
        public virtual LocatieTematica LocatieTematica { get; set; }

        [ForeignKey("TraseuId")]
        public virtual Traseu Traseu { get; set; }
    }
}