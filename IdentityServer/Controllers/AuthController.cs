using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using IdentityServer.ViewModels;
using IdentityServer4.Services;
using System.Security.Claims;

namespace IdentityServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IIdentityServerInteractionService _interactionService;

        public AuthController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IIdentityServerInteractionService interactionService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _interactionService = interactionService;
        }

        #region Login

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var externalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();

            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalProviders = externalProviders,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            // Check if the model is valid

            var result = await _signInManager.PasswordSignInAsync(loginViewModel.UserName, loginViewModel.Password, false, false);

            if (!result.Succeeded)
            {
                ViewData.ModelState.AddModelError(string.Empty, "Invalid login");
                return View(loginViewModel);
            }
            else if (result.IsLockedOut)
            {

            }

            return Redirect(loginViewModel.ReturnUrl);

        }

        #endregion

        #region Logout

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();

            var logoutRequest = await _interactionService.GetLogoutContextAsync(logoutId);

            if (string.IsNullOrEmpty(logoutRequest.PostLogoutRedirectUri))
            {
                return RedirectToAction("Index", "Home");
            }

            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }

        #endregion

        #region Register

        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            // Check if the model is valid

            var user = new IdentityUser(registerViewModel.UserName);
            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            {
                await _signInManager.PasswordSignInAsync(user, registerViewModel.Password, false, false);
                return Redirect(registerViewModel.ReturnUrl);
            }

            return View();
        }
        #endregion

        #region ExternalProvider

        /// <summary>
        /// Inicia o processo de login externo
        /// </summary>
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
        {
            var redirectUri = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUri);

            // facebook middleware -> facebook -> facebook middleware -> redirectUri
            return Challenge(properties, provider);
        }

        /// <summary>
        /// Se autenticado no Login Externo e Cadastrado no Identity, retornar para returnUrl
        /// Caso contrário, redirecionar para RegisterExternal
        /// </summary>
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            // What happends during the facebook signIn
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }

            // The facebook account match the identity account
            var result = await _signInManager
                .ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

            // The fact we authenticate in Facebook, doesn't mean we are registered in Identity
            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }

            var userName = info.Principal.FindFirst(ClaimTypes.Email).Value;
            return View("RegisterExternal", new RegisterExternalViewModel
            {
                UserName = userName,
                ReturnUrl = returnUrl,
            });
        }


        [HttpPost]
        public async Task<IActionResult> ExternalRegister(RegisterExternalViewModel vm)
        {
            // External info
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }

            // Create the user in Identity
            var user = new IdentityUser(vm.UserName);
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                return View(vm);
            }

            // Link the external login to the identity login
            var addLoginResult = await _userManager.AddLoginAsync(user, info);

            if (!addLoginResult.Succeeded)
            {
                return View(vm);
            }


            await _signInManager.SignInAsync(user, false);
            return Redirect(vm.ReturnUrl);
        }

        #endregion
    }
}
