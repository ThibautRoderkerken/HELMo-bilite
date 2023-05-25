using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using HELMo_bilite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using HELMo_bilite.Areas.Identity.Pages.Account.Manage;

namespace HELMo_bilite.Areas.Identity.Pages.Account
{
    public class RegisterClientModel : PageModel
    {
        private readonly SignInManager<Client> _signInManager;
        private readonly UserManager<Client> _userManager;
        private readonly IUserStore<Client> _userStore;
        private readonly IUserEmailStore<Client> _emailStore;
        private readonly ILogger<RegisterClientModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterClientModel(
            UserManager<Client> userManager,
                       IUserStore<Client> userStore,
                                  SignInManager<Client> signInManager,
                                             ILogger<RegisterClientModel> logger,
                                                        IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }
        
        [BindProperty]  
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Adresse e-mail")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Le {0} doit contenir au moins {2} et au maximum {1} caractères.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mot de passe")]
            public string Password { get; set; }

            [Required]
            [Display(Name = "Confirmer le mot de passe")]
            [Compare("Password", ErrorMessage = "Le mot de passe et le mot de passe de confirmation ne correspondent pas.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Nom de l'entreprise")]
            public string CompanyName { get; set; }

            [Required]
            [Display(Name = "Rue")]
            public string Street { get; set; }

            [Required]
            [Display(Name = "Numéro")]
            public int Number { get; set; }

            [Required]
            [Display(Name = "Ville")]
            public string City { get; set; }

            [Required]
            [Display(Name = "Code postal")]
            public int PostalCode { get; set; }

            [Required]
            [Display(Name = "Pays")]
            public string Country { get; set; }

            [Display(Name = "Logo de l'entreprise")]
            public IFormFile? CompanyLogo { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.CompanyName = Input.CompanyName;
                user.Street = Input.Street;
                user.Number = Input.Number;
                user.City = Input.City;
                user.PostalCode = Input.PostalCode;
                user.Country = Input.Country;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirmez votre adresse e-mail",
                        $"Veuillez confirmer votre compte en <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>cliquant ici</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }

        private Client CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Client>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(Client)}'. " +
                    $"Ensure that '{nameof(Client)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<Client> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<Client>)_userStore;
        }

    }
}
