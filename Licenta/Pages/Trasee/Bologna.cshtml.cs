using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Licenta.Data;
using System.Threading.Tasks;

namespace Licenta.Pages.Trasee
{
    [Authorize]
    public class BolognaModel : PageModel
    {
        private readonly LicentaDbContext _context;

        public BolognaModel(LicentaDbContext context)
        {
            _context = context;
        }

        public int TraseuId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var traseu = await _context.Trasee
                .FirstOrDefaultAsync(t => t.DenumireTraseu == "Traseu Culinar - Bologna");

            if (traseu == null)
            {
                return NotFound();
            }

            TraseuId = traseu.IdTraseu;
            return Page();
        }
    }
}
