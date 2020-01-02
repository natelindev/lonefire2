using Askmethat.Aspnet.JsonLocalizer.Localizer;
using lonefire.Data;
using lonefire.Models;
using lonefire.Models.HelperModels;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace lonefire.Controllers
{
    [Authorize(Roles = Constants.AdministratorsRole)]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("[controller]")]
    public class NoteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly INotifier _notifier;
        private readonly ILogger<ArticleController> _logger;
        private readonly IJsonStringLocalizer _localizer;
        private readonly UserManager<ApplicationUser> _userManager;

        public NoteController(
        ApplicationDbContext context,
            INotifier notifier,
            IJsonStringLocalizer localizer,
            UserManager<ApplicationUser> userManager,
            ILogger<ArticleController> logger
            )
        {
            _context = context;
            _notifier = notifier;
            _localizer = localizer;
            _logger = logger;
            _userManager = userManager;
            _localizer.ClearMemCache(new List<CultureInfo>()
            {
               new CultureInfo("en-US")
            });
        }

        // GET: /Note
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            try
            {
                var notes = await _context.Note.ToListAsync();
                return Ok(notes);
            }
            catch (Exception e)
            {
                _logger.LogError($"Get all notes failed {e.Message}");
                _notifier.Notify(_localizer["Get all notes failed"]);
                return StatusCode(500);
            }
        }

        // GET: /Note/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get([RegularExpression(Constants.base64UrlRegex)] string idBase64)
        {
            Guid id = idBase64.Base64UrlDecode();
            try
            {
                var note = await _context.Note.FirstOrDefaultAsync(n => n.Id == id);

                if (note == null)
                {
                    return NotFound();
                }
                return Ok(note);
            }
            catch (Exception e)
            {
                _logger.LogError($"Get note {id} failed {e.Message}");
                _notifier.Notify(_localizer["Get note failed"]);
                return StatusCode(500);
            }
        }

        // POST /Note
        [HttpPost]
        public async Task<IActionResult> Post([Bind("NoteName,NoteNameZh,Description,DescriptionZh")] Note note)
        {
            try
            {
                _context.Note.Add(note);
                await _context.SaveChangesAsync();
                return Created($"/Note/{note.Id}", note);
            }
            catch (Exception e)
            {
                _logger.LogError($"Post note {note.Title ?? note.TitleZh} failed {e.Message}");
                _notifier.Notify(_localizer["Create note failed"]);
                return StatusCode(500);
            }
        }

        // POST: /Note/{id}/like
        [HttpPost("{id}/like")]
        [AllowAnonymous]
        public async Task<IActionResult> Like([RegularExpression(Constants.base64UrlRegex)] string idBase64)
        {
            Guid id = idBase64.Base64UrlDecode();
            try
            {
                var userId = _userManager.GetUserId(User).ToGuid();
                var note = await _context.Note.Where(n => (n.StatusValue == Status.Public || n.OwnerId == userId) && n.Id == id)
                                    .FirstOrDefaultAsync();

                if (note == null)
                {
                    return NotFound();
                }

                note.LikeCount++;
                _context.Update(note);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"Like note {id} Failed {e.Message}");
                _notifier.Notify(_localizer["Like note failed"]);
                return StatusCode(500);
            }
        }

        // PUT /Note/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([RegularExpression(Constants.base64UrlRegex)] string idBase64, [Bind("NoteName,NoteNameZh,Description,DescriptionZh")] Note note)
        {
            Guid id = idBase64.Base64UrlDecode();
            try
            {
                _context.Note.Update(note);
                await _context.SaveChangesAsync();
                return Created($"/Note/{note.Id}", note);
            }
            catch (Exception e)
            {
                _logger.LogError($"Put note {note.Title ?? note.TitleZh} failed {e.Message}");
                _notifier.Notify(_localizer["Update note failed"]);
                return StatusCode(500);
            }
        }

        // PATCH /Note/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([RegularExpression(Constants.base64UrlRegex)] string idBase64, [FromBody] Note note)
        {
            Guid id = idBase64.Base64UrlDecode();
            var noteToUpdate = await _context.Note.FirstOrDefaultAsync(n => n.Id == id);
            if (await TryUpdateModelAsync(noteToUpdate, "",
                 n => n.Title, n => n.TitleZh, n => n.Content, n => n.ContentZh
                ))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Patch note {id} failed {e.Message}");
                    _notifier.Notify(_localizer["Update note failed"]);
                    return StatusCode(500);
                }
            }
            return BadRequest();
        }

        // DELETE /Note/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([RegularExpression(Constants.base64UrlRegex)] string idBase64)
        {
            Guid id = idBase64.Base64UrlDecode();
            try
            {
                var note = await _context.Note.Where(n => n.Id == id).FirstOrDefaultAsync();
                if (note == null)
                {
                    return NotFound();
                }

                _context.Note.Remove(note);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"Delete note {id} failed {e.Message}");
                _notifier.Notify(_localizer["Delete note failed"]);
                return StatusCode(500);
            }
        }
    }
}
