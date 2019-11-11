using System;
using System.Net.Mime;
using System.Threading.Tasks;
using lonefire.Models;
using lonefire.Models.AccountModels;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

//TODO: enable [ValidateAntiForgeryToken] after figuring out how to do so in spa

namespace lonefire.Controllers
{
    [Authorize]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[controller]/[action]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly INotifier _notifier;
        private readonly IStringLocalizer _localizer;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            INotifier notifier)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _notifier = notifier;
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            
            // Lockout enabled
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
            DateTimeOffset? endTime = null;
            if (result.Succeeded)
            {

                ApplicationUser user = await _userManager.FindByNameAsync(model.UserName);
                endTime = user.LockoutEnd;

                // Record LastLoginTime Time
                if (user != null)
                {
                    user.LastLoginTime = DateTimeOffset.UtcNow;
                    await _userManager.UpdateAsync(user);
                }
                _logger.LogInformation($"User {model.UserName} logged in.");
                return Ok(_localizer["Login successful"]);

            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning($"User {model.UserName} attempted to login while locked out.");
                return StatusCode(423, new { message = _localizer["Account locked out"], lockoutEnd =
                    endTime?.ToLocalTime() });
            }

            _logger.LogWarning($"User {model.UserName} failed to login.");
            return BadRequest(_localizer["Invalid credentials"]);
        }


        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
          
            var user = new ApplicationUser { UserName = model.UserName };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User {model.UserName} registered, waiting for Email Confirmation");

                // Email Confirmation
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                //await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation($"User {model.UserName} register compelete");
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _notifier.Notify(_localizer["Failed to get user"], NotificationLevel.Error);
                return StatusCode(500);
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return result.Succeeded ? Ok() : StatusCode(400);
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return BadRequest();
            }

            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
            await _emailSender.SendEmailAsync(model.Email, _localizer["Reset Password"],
                $"{_localizer["Click here to reset password"]} <a href='{callbackUrl}'>{_localizer["link"]}</a>");
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            //TODO: implement this
            //code = "1234";
            if (code == null)
            {
                return BadRequest(_localizer["A code must be supplied for password reset."]);
            }
            var model = new ResetPasswordModel { Code = code };
            return Ok(model);
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return BadRequest();
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
