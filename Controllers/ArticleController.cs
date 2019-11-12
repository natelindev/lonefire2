using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
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
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _aus;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileIoHelper _ioHelper;
        private readonly INotifier _notifier;
        private readonly IConfiguration _config;
        private readonly ILogger<ArticleController> _logger;
        private readonly IStringLocalizer _localizer;

        public ArticleController(
        ApplicationDbContext context,
            IAuthorizationService aus,
            UserManager<ApplicationUser> userManager,
            IFileIoHelper ioHelper,
            INotifier notifier,
            IConfiguration config,
            IStringLocalizer localizer,
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
                var articles = await _context.Article.Where(a => (a.Status == Status.Approved || a.Owner == userId))
                                .ToListAsync();
                return Ok(articles);
            }
            catch (Exception)
            {
                _logger.LogError("Get all articles Failed");
                _notifier.Notify(_localizer["Get all articles Failed"]);
                return StatusCode(500);
            }
        }

        // GET: /Article/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var userId = _userManager.GetUserId(User).ToGuid();
                var article = await _context.Article.Where(a => (a.Status == Status.Approved || a.Owner == userId) && a.Id == id)
                                .FirstOrDefaultAsync();

                if (article == null)
                {
                    return NotFound();
                }
                article.ViewCount++;
                await _context.SaveChangesAsync();

                return Ok(article);
            }
            catch (Exception)
            {
                _logger.LogError($"Get article {id} failed");
                _notifier.Notify(_localizer["Get article failed"]);
                return StatusCode(500);
            }
        }

        // GET: /Article/{title}
        [HttpGet("{title}")]
        [AllowAnonymous]
        public async Task<ActionResult<Article>> Get([FromQuery] string title)
        {
            try
            {
                var userId = _userManager.GetUserId(User).ToGuid();
                var article = await _context.Article.Where(a => (a.Status == Status.Approved ||
                a.Owner == userId) && a.Title == title || a.TitleZh == title).FirstOrDefaultAsync();
                return Ok(article);
            }
            catch (Exception)
            {
                _logger.LogError($"Get article {title} Failed");
                _notifier.Notify(_localizer["Get article failed"]);
                return StatusCode(500);
            }
        }

        // GET: /Article/{id}/Comments
        [HttpGet("{id}/Comments")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRelatedArticles(Guid id)
        {
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
            catch (Exception)
            {
                _logger.LogError($"Get aritlce comments for {id} failed");
                _notifier.Notify(_localizer["Get aritlce comments failed"]);
                return StatusCode(500);
            }
        }

        // GET: /Article/{id}/related
        [HttpGet("{id}/related")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRelatedArticles(Guid id, int number)
        {
            //Randomly pick articles
            //TODO: Actually implement the article recommendation algorithm
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
                    .Where(a => a.Title != title && a.TitleZh != title && (a.Status == Status.Approved || a.Owner == userId))
                    .OrderBy(s => random.Next()).Take(number).ToListAsync();

                return Ok(aritlces);
            }
            catch (Exception)
            {
                _logger.LogError($"Get related articles for {id} failed");
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
                    article.Owner = article.Owner;
                    article.Status = Status.Approved;
                }
                else
                {
                    //Use current user as author
                    article.Owner = userId;
                    article.Status = Status.Submitted;
                }

                _context.Add(article);
                await _context.SaveChangesAsync();
                return Created($"Article/{article.Id}", article);
            }
            catch (Exception)
            {
                _logger.LogError($"Post article {article.Title ?? article.TitleZh} failed");
                _notifier.Notify(_localizer["Create article failed"]);
                return StatusCode(500);
            }
        }

        // POST: /Article/{id}/like
        [HttpPost("{id}/like")]
        [AllowAnonymous]
        public async Task<IActionResult> Like(Guid id)
        {
            try
            {
                var userId = _userManager.GetUserId(User).ToGuid();
                var article = await _context.Article.Where(a => (a.Status == Status.Approved || a.Owner == userId) && a.Id == id)
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
            catch (Exception)
            {
                _logger.LogError($"Like article {id} Failed");
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
                 a => a.Status, a => a.HeaderImg,
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
                 a => a.Status, a => a.HeaderImg
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
            return BadRequest();
        }

        // PUT not supported, as it would break Tag, Image, Comment connection

        // DELETE: Article/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
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

                    var tags = article.Tags;
                    var images = article.Images;

                    // Remove article
                    _context.Article.Remove(article);
                    await _context.SaveChangesAsync();

                    // Remove Tags
                    foreach (var tag in tags)
                    {
                        tag.TagCount--;
                        if (tag.TagCount == 0)
                        {
                            _context.Tag.Remove(tag);
                        }
                    }
                    await _context.SaveChangesAsync();

                    // Remove Images
                    foreach (var image in images)
                    {
                        _context.Image.Remove(image);
                    }
                    await _context.SaveChangesAsync();

                    // Commit transaction if all commands succeed, transaction will auto-rollback
                    // when disposed if either commands fails
                    transaction.Commit();
                }
                catch (Exception)
                {
                    _logger.LogError($"Delete article {id} failed");
                    _notifier.Notify(_localizer["Delete article failed"]);
                    return StatusCode(500);
                }
            }

            return NoContent();
        }

    }
}
