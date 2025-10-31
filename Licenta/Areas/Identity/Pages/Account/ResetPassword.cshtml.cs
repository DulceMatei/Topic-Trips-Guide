using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Licenta.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Licenta.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<LicentaUser> _userManager;

        public ResetPasswordModel(UserManager<LicentaUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public string OriginalEmail { get; set; }  

        public class InputModel
        {
            [Required(ErrorMessage = "Emailul este obligatoriu.")]
            [EmailAddress(ErrorMessage = "Email invalid.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Parola este obligatorie.")]
            [StringLength(100, ErrorMessage = "Parola trebuie să aibă între {2} și {1} caractere.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Parolă nouă")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Confirmarea parolei este obligatorie.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirmă parola")]
            [Compare("Password", ErrorMessage = "Parolele nu coincid.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Code { get; set; }
        }

        public IActionResult OnGet(string code = null, string email = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }

            if (code == null || email == null)
            {
                return BadRequest("Link-ul de resetare nu este valid.");
            }

            OriginalEmail = email;

            Input = new InputModel
            {
                Email = email, 
                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.Email != OriginalEmail)
            {
                ModelState.AddModelError(string.Empty, "Acesta nu este emailul asociat acestui link de resetare.");
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
