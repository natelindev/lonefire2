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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace lonefire.Controllers
{
    [Authorize]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly INotifier _notifier;
        private readonly ILogger<ArticleController> _logger;
        private readonly IJsonStringLocalizer _localizer;

        public ImageController(
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

        // GET: /Image
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            try
            {
                var images = await _context.Image.ToListAsync();
                return Ok(images);
            }
            catch (Exception e)
            {
                _logger.LogError($"Get all images failed {e.Message}");
                _notifier.Notify(_localizer["Get all images Failed"]);
                return StatusCode(500);
            }
        }

        // GET: /Image/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get([RegularExpression(Constants.base64UrlRegex)] string idBase64)
        {
            Guid id = idBase64.Base64UrlDecode();
            try
            {
                var image = await _context.Image.FirstOrDefaultAsync(i => i.Id == id);

                if (image == null)
                {
                    return NotFound();
                }
                return Ok(image);
            }
            catch (Exception e)
            {
                _logger.LogError($"Get image {id} failed {e.Message}");
                _notifier.Notify(_localizer["Get image failed"]);
                return StatusCode(500);
            }
        }

        // POST /Image
        [HttpPost]
        public async Task<IActionResult> Post([Bind("Url,Description,DescriptionZh,IconUrl")] Image image,IFormFile imageFile)
        {
            try
            {
                _context.Image.Add(image);
                await _context.SaveChangesAsync();
                return Created($"/Image/{image.Id}", image);
            }
            catch (Exception e)
            {
                _logger.LogError($"Post image {image.Path} failed {e.Message}");
                _notifier.Notify(_localizer["Create image failed"]);
                return StatusCode(500);
            }
        }

        // PUT /Image/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([RegularExpression(Constants.base64UrlRegex)] string idBase64, [Bind("Url,Description,DescriptionZh,IconUrl")] Image image, IFormFile imageFile)
        {
            Guid id = idBase64.Base64UrlDecode();
            try
            {
                _context.Image.Update(image);
                await _context.SaveChangesAsync();
                return Created($"/Image/{image.Id}", image);
            }
            catch (Exception e)
            {
                _logger.LogError($"Put image {image.Path} failed {e.Message}");
                _notifier.Notify(_localizer["Update image failed"]);
                return StatusCode(500);
            }
        }

        // PATCH /Image/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([RegularExpression(Constants.base64UrlRegex)] string idBase64, [FromBody] Image image)
        {
            Guid id = idBase64.Base64UrlDecode();
            var imageToUpdate = await _context.Image.FirstOrDefaultAsync(a => a.Id == id);
            if (await TryUpdateModelAsync(imageToUpdate, "",
                 i => i.Path, i => i.Filename, i => i.Height, i => i.Width
                ))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Patch image {id} failed {e.Message}");
                    _notifier.Notify(_localizer["Update image failed"]);
                    return StatusCode(500);
                }
            }
            return BadRequest();
        }

        // DELETE /Image/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([RegularExpression(Constants.base64UrlRegex)] string idBase64)
        {
            Guid id = idBase64.Base64UrlDecode();
            try
            {
                var image = await _context.Image.Where(i => i.Id == id).FirstOrDefaultAsync();
                if (image == null)
                {
                    return NotFound();
                }

                _context.Image.Remove(image);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"Delete image {id} failed {e.Message}");
                _notifier.Notify(_localizer["Delete image failed"]);
                return StatusCode(500);
            }
        }
    }
}