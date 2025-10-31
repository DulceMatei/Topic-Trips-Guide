// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Licenta.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Licenta.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<LicentaUser> _userManager;
        private readonly SignInManager<LicentaUser> _signInManager;

        public IndexModel(
            UserManager<LicentaUser> userManager,
            SignInManager<LicentaUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }
        public string CurrentNume { get; set; }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            /// 
            [Display(Name = "Nume")]
            public string Nume { get; set; }

            [Phone(ErrorMessage = "Numărul de telefon nu este valid.")]
            [Display(Name = "Număr de telefon")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(LicentaUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            Username = userName;
            CurrentNume = user.Nume;
            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Nume = ""
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Nu se poate încărca utilizatorul cu ID-ul '{_userManager.GetUserId(User)}'.");
            }
            await LoadAsync(user);
            StatusMessage = ""; 
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Nu se poate încărca utilizatorul cu ID-ul '{_userManager.GetUserId(User)}'.");
            }
            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Eroare neașteptată la setarea numărului de telefon.";
                    return RedirectToPage();
                }
            }
            if (!string.IsNullOrEmpty(Input.Nume))
            {
                if (Input.Nume == user.Nume)
                {
                    StatusMessage = "Numele introdus este identic cu numele actual.";
                    return RedirectToPage();
                }
                else
                {
                    user.Nume = Input.Nume;
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        StatusMessage = "Eroare la actualizarea numelui.";
                        return RedirectToPage();
                    }
                }
            }
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Profilul tău a fost actualizat cu succes.";
            return RedirectToPage();
        }
    }
}
