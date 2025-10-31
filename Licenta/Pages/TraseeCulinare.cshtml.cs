using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Licenta.Pages
{
    [Authorize]
    public class TraseeCulinareModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
