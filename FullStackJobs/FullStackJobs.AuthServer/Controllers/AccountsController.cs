using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullStackJobs.AuthServer.Infrastructure.Data;
using FullStackJobs.AuthServer.Infrastructure.Data.Identity;
using FullStackJobs.AuthServer.Models;
using FullStackJobs.AuthServer.Models.ViewModels;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FullStackJobs.AuthServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IEventService _events;

        public AccountsController(SignInManager<AppUser> signInManager, IIdentityServerInteractionService interaction, UserManager<AppUser> userManager, AppIdentityDbContext appIdentityDbContext, IEventService events, IUserRepository userRepository)
        {
            _signInManager = signInManager;
            _interaction = interaction;
            _userManager = userManager;
            _userRepository = userRepository;
            _events = events;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/[controller]")]
        public async Task<IActionResult> Post([FromBody]SignupRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new AppUser { UserName = model.Email, FullName = model.FullName, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("name", user.FullName));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("role", model.Role));

            await _userRepository.InsertEntity(model.Role, user.Id, user.FullName);

            return Ok(new SignupResponse(user, model.Role));
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl,
                Username = GetUserName(returnUrl) ?? context?.LoginHint,
                NewAccount = returnUrl.Contains("newAccount"),
            });
        }

        private static string GetUserName(string returnUrl)
        {
            const string parameter = "&userName=";

            return returnUrl.Contains("userName") ? returnUrl.Substring(returnUrl.IndexOf("&userName=") + parameter.Length) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.FullName));

                    AuthenticationProperties props = null;

                    if (AccountOptions.AllowRememberLogin && model.RememberLogin)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                        };
                    };

                    // cannot convert from 'string' to 'System.Security.Claims.ClaimsPrincipal'
                    //await HttpContext.SignInAsync(user.Id, user.UserName, props);

                    if (context != null)
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        throw new Exception("invalid return URL");
                    }
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));

                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            var vm = new LoginViewModel
            {
                Username = model.Username,
                RememberLogin = model.RememberLogin
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();

            var context = await _interaction.GetLogoutContextAsync(logoutId);

            return Redirect(context.PostLogoutRedirectUri);
        }
    }
}