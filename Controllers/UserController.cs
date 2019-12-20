using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using lonefire.Data;
using lonefire.Models;
using lonefire.Models.HelperModels;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace lonefire.Controllers
{
    [Authorize(Constants.AdministratorsRole)]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _aus;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileIoHelper _ioHelper;
        private readonly INotifier _notifier;
        private readonly IConfiguration _config;
        private readonly ILogger<UserController> _logger;
        private readonly IJsonStringLocalizer _localizer;

        public UserController(
        ApplicationDbContext context,
            IAuthorizationService aus,
            UserManager<ApplicationUser> userManager,
            IFileIoHelper ioHelper,
            INotifier notifier,
            IConfiguration config,
            IJsonStringLocalizer localizer,
            ILogger<UserController> logger
            )
        {
            _aus = aus;
            _userManager = userManager;
            _context = context;
            _ioHelper = ioHelper;
            _notifier = notifier;
            _config = config;
            _localizer = localizer;
            _logger = logger;
            _localizer.ClearMemCache(new List<CultureInfo>()
            {
               new CultureInfo("en-US")
            });
        }

        // GET: /User
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return Ok(users);
            }
            catch (Exception e)
            {
                _logger.LogError($"Get all users Failed {e.Message}");
                _notifier.Notify(_localizer["Get all users Failed"]);
                return StatusCode(500);
            }
        }

        // GET: /User/a
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([RegularExpression(Constants.base64UrlRegex)] string idBase64)
        {
            Guid id = idBase64.Base64UrlDecode();
            try
            {
                var user = await _context.Users.Where(u => u.Public)
                                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception e)
            {
                _logger.LogError($"Get user {id} Failed {e.Message}");
                _notifier.Notify(_localizer["Get user Failed"]);
                return StatusCode(500);
            }
        }
    }
}