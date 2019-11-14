using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using lonefire.Authorization;
using lonefire.Data;
using lonefire.Models;
using lonefire.Models.UtilModels;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace lonefire.Controllers
{
    [Authorize]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("[controller]")]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _aus;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileIoHelper _ioHelper;
        private readonly INotifier _notifier;
        private readonly IConfiguration _config;
        private readonly ILogger<CommentController> _logger;
        private readonly IJsonStringLocalizer _localizer;

        public CommentController(
        ApplicationDbContext context,
            IAuthorizationService aus,
            UserManager<ApplicationUser> userManager,
            IFileIoHelper ioHelper,
            INotifier notifier,
            IConfiguration config,
            IJsonStringLocalizer localizer,
            ILogger<CommentController> logger
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
        }

        public string ImageUploadPath => _config.GetValue<string>("UploadPaths.Images");

        // GET: /Comment
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userId = _userManager.GetUserId(User).ToGuid();
                var comments = await _context.Comment.ToListAsync();
                return Ok(comments);
            }
            catch (Exception)
            {
                _logger.LogError("Get all comments Failed");
                _notifier.Notify(_localizer["Get all comments Failed"]);
                return StatusCode(500);
            }
        }

        // GET: /Comment/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var userId = _userManager.GetUserId(User).ToGuid();
                var comment = await _context.Comment.Where(c => c.Id == id)
                                .FirstOrDefaultAsync();

                if (comment == null)
                {
                    return NotFound();
                }
                return Ok(comment);
            }
            catch (Exception)
            {
                _logger.LogError($"Get comment {id} failed");
                _notifier.Notify(_localizer["Get comment failed"]);
                return StatusCode(500);
            }
        }

        // POST: /Comment
        // New Comment Should not have Likecount
        [HttpPost]
        public async Task<IActionResult> Post([Bind("ParentId,Content,ContentZh,Owner,Website,Email")]Comment comment)
        {
            try
            {
                var isAuthorized = await _aus.AuthorizeAsync(User, comment,
                                                CommentOperations.Post);
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }

                var userId = _userManager.GetUserId(User).ToGuid();

                if (!User.IsInRole(Constants.AdministratorsRole))
                {
                    //Only admin can change comment owner
                    //others use current user as author
                    comment.OwnerId = userId;
                }

                _context.Add(comment);
                await _context.SaveChangesAsync();
                return Created($"Comment/{comment.Id}", comment);
            }
            catch (Exception)
            {
                _logger.LogError($"Post comment {comment.ParentId} failed");
                _notifier.Notify(_localizer["Create comment failed"]);
                return StatusCode(500);
            }
        }

        // POST: /Comment/{id}/like
        [HttpPost("{id}/like")]
        [AllowAnonymous]
        public async Task<IActionResult> Like(Guid id)
        {
            try
            {
                var userId = _userManager.GetUserId(User).ToGuid();
                var comment = await _context.Comment.Where(c => c.Id == id)
                                    .FirstOrDefaultAsync();

                if (comment == null)
                {
                    return NotFound();
                }

                comment.LikeCount++;
                _context.Update(comment);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                _logger.LogError($"Like comment {id} Failed");
                _notifier.Notify(_localizer["Like comment failed"]);
                return StatusCode(500);
            }
        }

        // PATCH: /Comment/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] Comment comment)
        {
            var commentToUpdate = await _context.Comment.FirstOrDefaultAsync(c => c.Id == id);

            if (commentToUpdate == null)
            {
                return NotFound();
            }

            var isAuthorized = await _aus.AuthorizeAsync(
                                                  User, commentToUpdate,
                                                  CommentOperations.Patch);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            //Only admin can change ParentId, Owner, LikeCount
            if (User.IsInRole(Constants.AdministratorsRole))
            {
                if (await TryUpdateModelAsync(commentToUpdate, "",
                 c => c.ParentId, c => c.Content, c => c.ContentZh,
                 c => c.Owner, c => c.Website, c => c.Email,c => c.LikeCount
                ))
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                        return NoContent();
                    }
                    catch (Exception)
                    {
                        _logger.LogError($"Patch comment {id} failed");
                        _notifier.Notify(_localizer["Update comment failed"]);
                        return StatusCode(500);
                    }
                }
            }
            else
            {
                if (await TryUpdateModelAsync(commentToUpdate, "",
                 c => c.Content, c => c.ContentZh,
                 c => c.Website, c => c.Email
                ))
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                        return NoContent();
                    }
                    catch (Exception)
                    {
                        _logger.LogError($"Patch comment {id} failed");
                        _notifier.Notify(_localizer["Update comment failed"]);
                        return StatusCode(500);
                    }
                }
            }
            return BadRequest();
        }

        // PUT not supported, as it would break Comment connection

        // DELETE: Comment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var comment = await _context.Comment.FirstOrDefaultAsync(c => c.Id == id);

            var isAuthorized = await _aus.AuthorizeAsync(
                                                 User, comment,
                                                 CommentOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            //Cascade delete all sub Comments done in DB Context

            return NoContent();
        }

    }
}
