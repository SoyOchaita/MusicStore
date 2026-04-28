using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace MusicStore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);
            // No reveles si el usuario no existe o no tiene email confirmado
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return RedirectToPage("./ForgotPasswordConfirmation");

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var codeBytes = Encoding.UTF8.GetBytes(code);
            var encodedCode = WebEncoders.Base64UrlEncode(codeBytes);

            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code = encodedCode, email = Input.Email },
                protocol: Request.Scheme);

            // Aquí enviarías el email con callbackUrl. Por ahora lo registramos en TempData.
            TempData["PasswordResetLink"] = callbackUrl;

            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}