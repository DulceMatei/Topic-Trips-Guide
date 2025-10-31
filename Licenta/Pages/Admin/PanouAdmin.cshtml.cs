using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Licenta.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class PanouAdminModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
