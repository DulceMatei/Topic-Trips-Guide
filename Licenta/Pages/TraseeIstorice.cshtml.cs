using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Licenta.Pages
{
    [Authorize]
    public class TraseeIstoriceModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
