using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Licenta.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity;
using System.Text.Encodings.Web;
using Licenta.Areas.Identity.Data;

namespace Licenta.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<LicentaUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<LicentaUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Câmpul email este obligatoriu.")]
            [EmailAddress(ErrorMessage = "Adresa de email nu este validă.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

        }
        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null) 
            {
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code = encodedToken, email = Input.Email },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                Input.Email,
                "Resetare parolă",
                $"Salut!<br><br>Ai solicitat resetarea parolei pentru contul tău.<br><br>" +
                $"Click <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aici</a> pentru a seta o parolă nouă.<br><br>" +
                $"Dacă nu ai solicitat acest lucru, poți ignora acest mesaj.");

            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}
