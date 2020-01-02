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
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace lonefire.Controllers
{
    [Authorize(Roles = Constants.AdministratorsRole)]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("[controller]")]
    public class LinkController : ControllerBase
    {
        private readonly ApplicationDbContext _context;   
        private readonly INotifier _notifier;
        private readonly ILogger<ArticleController> _logger;
        private readonly IJsonStringLocalizer _localizer;

        public LinkController(
        ApplicationDbContext context,
            INotifier notifier,
            IJsonStringLocalizer localizer,
            ILogger<ArticleController> logger
            )
        {
            _context = context;
            _notifier = notifier;
            _localizer = localizer;
            _logger = logger;
            _localizer.ClearMemCache(new List<CultureInfo>()
            {
               new CultureInfo("en-US")
            });
        }

        // GET: /Link
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            try
            {
                var links = await _context.Link.ToListAsync();
                return Ok(links);
            }
            catch (Exception e)
            {
                _logger.LogError($"Get all links failed {e.Message}");
                _notifier.Notify(_localizer["Get all links Failed"]);
                return StatusCode(500);
            }
        }

        // GET: /Link/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get([RegularExpression(Constants.base64UrlRegex)] string idBase64)
        {
            Guid id = idBase64.Base64UrlDecode();
            try
            {
                var link = await _context.Link.FirstOrDefaultAsync(l => l.Id == id);

                if (link == null)
                {
                    return NotFound();
                }
                return Ok(link);
            }
            catch (Exception e)
            {
                _logger.LogError($"Get link {id} failed {e.Message}");
                _notifier.Notify(_localizer["Get link failed"]);
                return StatusCode(500);
            }
        }

        // POST /Link
        [HttpPost]
        public async Task<IActionResult> Post([Bind("Url,Description,DescriptionZh,IconUrl")] Link link)
        {
            try
            {
                _context.Link.Add(link);
                await _context.SaveChangesAsync();
                return Created($"/Link/{link.Id}", link);
            }
            catch (Exception e)
            {
                _logger.LogError($"Post link {link.Url} failed {e.Message}");
                _notifier.Notify(_localizer["Create link failed"]);
                return StatusCode(500);
            }
        }

        // PUT /Link/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([RegularExpression(Constants.base64UrlRegex)] string idBase64, [Bind("Url,Description,DescriptionZh,IconUrl")] Link link)
        {
            Guid id = idBase64.Base64UrlDecode();
            try
            {
                _context.Link.Update(link);
                await _context.SaveChangesAsync();
                return Created($"/Link/{link.Id}", link);
            }
            catch (Exception e)
            {
                _logger.LogError($"Put link {link.Url} failed {e.Message}");
                _notifier.Notify(_localizer["Update link failed"]);
                return StatusCode(500);
            }
        }

        // PATCH /Link/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([RegularExpression(Constants.base64UrlRegex)] string idBase64, [FromBody] Link link)
        {
            Guid id = idBase64.Base64UrlDecode();
            var linkToUpdate = await _context.Link.FirstOrDefaultAsync(a => a.Id == id);
            if (await TryUpdateModelAsync(linkToUpdate, "",
                 l => l.Url, l => l.Description, l => l.DescriptionZh, l => l.IconUrl
                ))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Patch link {id} failed {e.Message}");
                    _notifier.Notify(_localizer["Update link failed"]);
                    return StatusCode(500);
                }
            }
            return BadRequest();
        }

        // DELETE /Link/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([RegularExpression(Constants.base64UrlRegex)] string idBase64)
        {
            Guid id = idBase64.Base64UrlDecode();
            try
            {
                var link = await _context.Link.Where(l => l.Id == id).FirstOrDefaultAsync();
                if(link == null)
                {
                    return NotFound();
                }

                _context.Link.Remove(link);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"Delete link {id} failed {e.Message}");
                _notifier.Notify(_localizer["Delete link failed"]);
                return StatusCode(500);
            }
        }
    }
}
