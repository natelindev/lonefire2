using System;
using System.Collections.Generic;
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
    public class TagController : ControllerBase
    {
        private readonly ApplicationDbContext _context;   
        private readonly INotifier _notifier;
        private readonly ILogger<ArticleController> _logger;
        private readonly IJsonStringLocalizer _localizer;

        public TagController(
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

        // GET: /Tag
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            try
            {
                var tags = await _context.Tag.ToListAsync();
                return Ok(tags);
            }
            catch (Exception)
            {
                _logger.LogError("Get all tags failed");
                _notifier.Notify(_localizer["Get all tags failed"]);
                return StatusCode(500);
            }
        }

        // GET: /Tag/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var tag = await _context.Tag.FirstOrDefaultAsync(t => t.Id == id);

                if (tag == null)
                {
                    return NotFound();
                }
                return Ok(tag);
            }
            catch (Exception)
            {
                _logger.LogError($"Get tag {id} failed");
                _notifier.Notify(_localizer["Get tag failed"]);
                return StatusCode(500);
            }
        }

        // POST /Tag
        [HttpPost]
        public async Task<IActionResult> Post([Bind("TagName,TagNameZh,Description,DescriptionZh")] Tag tag)
        {
            try
            {
                _context.Tag.Add(tag);
                await _context.SaveChangesAsync();
                return Created($"/Tag/{tag.Id}", tag);
            }
            catch (Exception)
            {
                _logger.LogError($"Post tag {tag.TagName ?? tag.TagNameZh} failed");
                _notifier.Notify(_localizer["Create tag failed"]);
                return StatusCode(500);
            }
        }

        // POST /Tag
        [HttpPost("{id}/Add")]
        public async Task<IActionResult> Post(Guid id)
        {
            try
            {
                var tag = await _context.Tag.Where(t => t.Id == id).FirstOrDefaultAsync();
                if(tag == null)
                {
                    return NotFound();
                }

                tag.TagCount++;
                _context.Tag.Update(tag);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                _logger.LogError($"Add tag count to {id} failed");
                _notifier.Notify(_localizer["Add tag count failed"]);
                return StatusCode(500);
            }
        }

        // PUT /Tag/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [Bind("TagName,TagNameZh,Description,DescriptionZh")] Tag tag)
        {
            try
            {
                _context.Tag.Update(tag);
                await _context.SaveChangesAsync();
                return Created($"/Tag/{tag.Id}", tag);
            }
            catch (Exception)
            {
                _logger.LogError($"Put tag {tag.TagName ?? tag.TagNameZh} failed");
                _notifier.Notify(_localizer["Update tag failed"]);
                return StatusCode(500);
            }
        }

        // PATCH /Tag/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] Tag tag)
        {
            var tagToUpdate = await _context.Tag.FirstOrDefaultAsync(a => a.Id == id);
            if (await TryUpdateModelAsync(tagToUpdate, "",
                 t => t.TagName, t => t.TagNameZh, t => t.Description, t => t.DescriptionZh
                ))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                catch (Exception)
                {
                    _logger.LogError($"Patch tag {id} failed");
                    _notifier.Notify(_localizer["Update tag failed"]);
                    return StatusCode(500);
                }
            }
            return BadRequest();
        }

        // DELETE /Tag/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var tag = await _context.Tag.Where(t => t.Id == id).FirstOrDefaultAsync();
                if(tag == null)
                {
                    return NotFound();
                }

                _context.Tag.Remove(tag);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception)
            {
                _logger.LogError($"Delete tag {id} failed");
                _notifier.Notify(_localizer["Delete tag failed"]);
                return StatusCode(500);
            }
        }
    }
}
