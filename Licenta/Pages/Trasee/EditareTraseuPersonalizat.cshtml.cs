using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Licenta.Pages.Trasee
{
    [Authorize]
    public class EditareTraseuPersonalizatModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
