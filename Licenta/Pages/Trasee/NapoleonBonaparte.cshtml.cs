using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Licenta.Data;
using System.Threading.Tasks;

namespace Licenta.Pages.Trasee
{
    [Authorize]
    public class NapoleonBonaparteModel : PageModel
    {
        private readonly LicentaDbContext _context;

        public NapoleonBonaparteModel(LicentaDbContext context)
        {
            _context = context;
        }

        public int TraseuId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var traseu = await _context.Trasee
                .FirstOrDefaultAsync(t => t.DenumireTraseu == "Traseul lui Napoleon Bonaparte");

            if (traseu == null)
            {
                return NotFound();
            }

            TraseuId = traseu.IdTraseu;
            return Page();
        }
    }
}
