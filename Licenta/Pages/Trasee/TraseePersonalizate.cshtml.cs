using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Licenta.Pages.Trasee
{
    [Authorize]
    public class TraseePersonaleModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
