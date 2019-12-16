using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using lonefire.Authorization;
using lonefire.Data;
using lonefire.Extensions;
using lonefire.Models;
using lonefire.Models.HelperModels;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;

namespace lonefire.Controllers
{
    [Authorize]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("[controller]")]
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _aus;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileIoHelper _ioHelper;
        private readonly INotifier _notifier;
        private readonly IConfiguration _config;
        private readonly ILogger<ArticleController> _logger;
        private readonly IJsonStringLocalizer _localizer;

        public ArticleController(
        ApplicationDbContext context,
            IAuthorizationService aus,
            UserManager<ApplicationUser> userManager,
            IFileIoHelper ioHelper,
            INotifier notifier,
            IConfiguration config,
            IJsonStringLocalizer localizer,
            ILogger<ArticleController> logger
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

        public string ImageUploadPath => _config.GetValue<string>("UploadPaths.Images");

        // GET: /Article
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userId = _userManager.GetUserId(User).ToGuid();
                var articles = await _context.Article.Where(a => (a.StatusValue == Status.Approved || a.OwnerId == userId))
                                .ToListAsync();
                return Ok(articles);
            }
            catch (Exception e)
            {
                _logger.LogError($"Get all articles Failed {e.Message}");
                _notifier.Notify(_localizer["Get all articles Failed"]);
                return StatusCode(500);
            }
        }

        // GET: /Article/{title}
        // supports thre
        [HttpGet("{title}")]
        [AllowAnonymous]
        public async Task<ActionResult<Article>> Get([FromRoute] string title)
        {
            try
            {
                var userId = _userManager.GetUserId(User).ToGuid();
                
                Guid? id = null;
                Article article = null;
                if (title.Length == 22 && title.IsBase64UrlString())
                {
                    // base64 only
                    id = title.Base64UrlDecode();
                } 
                else if (title.IsGuid())
                {
                    // raw guid
                    id = title.ToGuid();
                }
                else if (title.Length > 22 && title.Substring(title.Length-22, 22).IsBase64UrlString())
                {
                    // title-base64guid
                    id = title.Substring(title.Length - 22, 22).Base64UrlDecode();
                }

                // use id
                try
                {
                    if (id != null)
                    {
                        // use id 
                        article = await _context.Article.Where(a => (a.StatusValue == Status.Approved || a.OwnerId == userId) && a.Id == id)
                                    .FirstOrDefaultAsync();
                    }
                    else
                    {
                        // title only
                        article = await _context.Article.Where(a => (a.StatusValue == Status.Approved ||
                        a.OwnerId == userId) && a.Title == title || a.TitleZh == title).FirstOrDefaultAsync();
                    }

                    if (article == null)
                    {
                        return NoContent();
                    }
                    article.ViewCount++;
                    await _context.SaveChangesAsync();

                    return Ok(article);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Get article {id} failed {e.Message}");
                    _notifier.Notify(_localizer["Get article failed"]);
                    return StatusCode(500);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Get article {title} Failed {e.Message}");
                _notifier.Notify(_localizer["Get article failed"]);
                return StatusCode(500);
            }
        }

        // GET: /Article/{id}/Comments
        [HttpGet("{id}/Comments")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRelatedArticles([RegularExpression(Constants.base64UrlRegex)]string idBase64)
        {
            Guid id = idBase64.Base64UrlDecode();
            try
            {

                var article = await _context.Article.Where(a => a.Id == id).FirstOrDefaultAsync();
                if (article == null)
                {
                    return NotFound();
                }

                var comments = await _context.Comment.Where(c => c.ParentId == article.Id).ToListAsync();

                return Ok(comments);
            }
            catch (Exception e)
            {
                _logger.LogError($"Get aritlce comments for {id} failed {e.Message}");
                _notifier.Notify(_localizer["Get aritlce comments failed"]);
                return StatusCode(500);
            }
        }

        // GET: /Article/{id}/related
        [HttpGet("{id}/related")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRelatedArticles([RegularExpression(Constants.base64UrlRegex)] string idBase64, int number)
        {
            //Randomly pick articles
            //TODO: Actually implement the article recommendation algorithm
            Guid id = idBase64.Base64UrlDecode();
            try
            {
                var originArticle = await _context.Article.Where(a => a.Id == id).FirstOrDefaultAsync();
                if (originArticle == null)
                {
                    return NotFound();
                }

                var userId = _userManager.GetUserId(User).ToGuid();
                string title = originArticle.Title ?? originArticle.TitleZh;
                var random = new Random();
                var aritlces = await _context.Article
                    .Where(a => a.Title != title && a.TitleZh != title && (a.StatusValue == Status.Approved || a.OwnerId == userId))
                    .OrderBy(s => random.Next()).Take(number).ToListAsync();

                return Ok(aritlces);
            }
            catch (Exception e)
            {
                _logger.LogError($"Get related articles for {id} failed {e.Message}");
                _notifier.Notify(_localizer["Get related articles failed"]);
                return StatusCode(500);
            }
        }

        // POST: /Article
        // Should be called after the Images have been uploaded
        // Should be called after the Tags have been created
        // New Article Should not have Comments or Likecount, Viewcount
        [HttpPost]
        public async Task<IActionResult> Post([Bind("Title,TitleZh,Owner,Content,ContentZh,HeaderImg,Images,Tags")]Article article)
        {
            try
            {
                var isAuthorized = await _aus.AuthorizeAsync(User, article,
                                                ArticleOperations.Post);
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }

                var canApprove = await _aus.AuthorizeAsync(User,
                                                article,
                                                ArticleOperations.Censor);

                var userId = _userManager.GetUserId(User).ToGuid();

                if (canApprove.Succeeded)
                {
                    //Only Mod can change article owner & Does not need Approving
                    article.Status = Status.Approved;
                }
                else
                {
                    //Use current user as author
                    article.OwnerId = userId;
                    article.Status = Status.Submitted;
                }

                _context.Add(article);
                await _context.SaveChangesAsync();
                return Created($"Article/{article.Id}", article);
            }
            catch (Exception e)
            {
                _logger.LogError($"Post article {article.Title ?? article.TitleZh} failed {e.Message}");
                _notifier.Notify(_localizer["Create article failed"]);
                return StatusCode(500);
            }
        }

        // POST: /Article/{id}/like
        [HttpPost("{id}/like")]
        [AllowAnonymous]
        public async Task<IActionResult> Like([RegularExpression(Constants.base64UrlRegex)] string idBase64)
        {
            Guid id = idBase64.Base64UrlDecode();
            try
            {
                var userId = _userManager.GetUserId(User).ToGuid();
                var article = await _context.Article.Where(a => (a.StatusValue == Status.Approved || a.OwnerId == userId) && a.Id == id)
                                    .FirstOrDefaultAsync();

                if (article == null)
                {
                    return NotFound();
                }

                article.LikeCount++;
                _context.Update(article);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"Like article {id} Failed {e.Message}");
                _notifier.Notify(_localizer["Like article failed"]);
                return StatusCode(500);
            }
        }

        // PATCH: /Article/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] Article article)
        {
            var articleToUpdate = await _context.Article.FirstOrDefaultAsync(a => a.Id == id);

            if (articleToUpdate == null)
            {
                return NotFound();
            }

            var isAuthorized = await _aus.AuthorizeAsync(
                                                  User, articleToUpdate,
                                                  ArticleOperations.Patch);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            //Only admin can change owner, LikeCount, ViewCount
            if (User.IsInRole(Constants.AdministratorsRole))
            {
                if (await TryUpdateModelAsync(articleToUpdate, "",
                 a => a.Title, a => a.TitleZh, a => a.Content, a => a.ContentZh,
                 a => a.StatusValue, a => a.HeaderImageId,
                 a => a.ViewCount, a => a.LikeCount, a => a.Owner
                ))
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                        return NoContent();
                    }
                    catch (Exception)
                    {
                        _logger.LogError($"Patch article {id} failed");
                        _notifier.Notify(_localizer["Update article failed"]);
                        return StatusCode(500);
                    }
                }
            }
            else
            {
                if (await TryUpdateModelAsync(articleToUpdate, "",
                 a => a.Title, a => a.TitleZh, a => a.Content, a => a.ContentZh,
                 a => a.StatusValue, a => a.HeaderImageId
                ))
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                        return NoContent();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Patch article {id} failed {e.Message}");
                        _notifier.Notify(_localizer["Update article failed"]);
                        return StatusCode(500);
                    }
                }
            }
            return BadRequest();
        }

        // PUT not supported, as it would break Tag, Image, Comment connection

        // DELETE: Article/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([RegularExpression(Constants.base64UrlRegex)] string idBase64)
        {
            Guid id = idBase64.Base64UrlDecode();
            var article = await _context.Article.FirstOrDefaultAsync(a => a.Id == id);

            var isAuthorized = await _aus.AuthorizeAsync(
                                                 User, article,
                                                 ArticleOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            //Transaction to delete all Tags, Images, Comments
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    var tags = article.ArticleTags;
                    var images = article.ArticleImages;

                    // Remove article
                    _context.Article.Remove(article);
                    await _context.SaveChangesAsync();

                    // Remove Tags
                    foreach (var tag in tags)
                    {
                        _context.ArticleTag.Remove(tag);
                    }
                    await _context.SaveChangesAsync();

                    // Remove Images
                    foreach (var image in images)
                    {
                        _context.ArticleImage.Remove(image);
                    }
                    await _context.SaveChangesAsync();

                    // Commit transaction if all commands succeed, transaction will auto-rollback
                    // when disposed if either commands fails
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Delete article {id} failed {e.Message}");
                    _notifier.Notify(_localizer["Delete article failed"]);
                    return StatusCode(500);
                }
            }

            return NoContent();
        }

    }
}
