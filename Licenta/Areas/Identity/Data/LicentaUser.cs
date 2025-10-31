using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Licenta.Models;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using Licenta.Areas.Identity.Data;

namespace Licenta.Areas.Identity.Data;

public class LicentaUser : IdentityUser
{
    [PersonalData]
    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string Nume { get; set; }

    [PersonalData]
    [DateRange]
    public DateTime DataInregistrării { get; set; }

    public virtual ICollection<Traseu> Trasee { get; set; }
    public virtual ICollection<Recenzie> Recenzii { get; set; }
}

