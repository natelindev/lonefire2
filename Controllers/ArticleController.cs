using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using lonefire.Authorization;
using lonefire.Data;
using lonefire.Extensions;
using lonefire.Models;
using lonefire.Models.UtilModels;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace lonefire.Controllers
{
    [Authorize]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("Api/[controller]")]
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotifier notifier;
        private readonly IAuthorizationService _aus;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileIoHelper _ioHelper;
        private readonly INotifier _notifier;
        private readonly IConfiguration _config;
        private readonly ILogger<ArticleController> _logger;

        public ArticleController(
        ApplicationDbContext context,
            IAuthorizationService aus,
            UserManager<ApplicationUser> userManager,
            IFileIoHelper ioHelper,
            INotifier notifier,
            IConfiguration config,
            ILogger<ArticleController> logger
            )
        {
            _aus = aus;
            _userManager = userManager;
            _context = context;
            _ioHelper = ioHelper;
            _notifier = notifier;
            _config = config;
            _logger = logger;
        }

        public string ImageUploadPath => _config.GetValue<string>("UploadPaths.Images");

        // GET: api/Article/{id}
        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Article>> Get(Guid id)
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
                _logger.LogError($"Read Aritle {id} Failed");
                return StatusCode(500);
            }
        }

        // GET: Api/Article/{title}
        [HttpGet]
        public async Task<ActionResult<Article>> Get([FromQuery] string title)
        {
            var userId = _userManager.GetUserId(User).ToGuid();
            var article = await _context.Article.Where(a => (a.Status == Status.Approved ||
            a.Owner == userId) && a.Title == title || a.TitleZh == title).FirstOrDefaultAsync();
            return Ok(article);
        }

        // GET: Api/Article/Like
        [HttpGet]
        public async Task<IActionResult> Like(Guid id)
        {
            var userId = _userManager.GetUserId(User).ToGuid();
            var article = await _context.Article.Where(a => (a.Status == Status.Approved || a.Owner == userId) && a.Id == id)
                                .FirstOrDefaultAsync();

            if(article == null)
            {
                return NotFound();
            }

            article.LikeCount++;
            _context.Update(article);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // PATCH: api/Article/{id}
        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> Patch(Guid id,[FromBody] Article article)
        {
            var articleToUpdate = await _context.Article.FirstOrDefaultAsync(a => a.Id == id);

            if (articleToUpdate == null)
            {
                return NotFound();
            }

            var isAuthorized = await _aus.AuthorizeAsync(
                                                  User, articleToUpdate,
                                                  opeartion);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

           
        }


        // GET: Article/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Article/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Title,Author,Tag,Content")]Article article, Status? Status, IFormFile headerImg, IList<IFormFile> contentImgs)
        {
            if (ModelState.IsValid)
            {
                var isAuthorized = await _aus.AuthorizeAsync(User, article,
                                                ArticleOperations.Create);
                if (!isAuthorized.Succeeded)
                {
                    return new ChallengeResult();
                }

                var canApprove = await _aus.AuthorizeAsync(User,
                                                article,
                                                ArticleOperations.Approve);

                var uid = _userManager.GetUserId(User);

                if (canApprove.Succeeded)
                {
                    //Only Mod can change Article Author & Does not need Approving
                    article.Author = article.Author ?? uid;
                    article.Status = Status ?? Status.Approved;
                }
                else
                {
                    //Use current user as author
                    article.Author = uid;
                }

                //save the Images
                if (headerImg != null || contentImgs.Count > 0)
                {
                    List<string> ArticleImgs = new List<string>();
                    if (headerImg != null)
                    {

                        article.HeaderImg = headerImg.FileName;
                        ArticleImgs.Add(headerImg.FileName);
                        var res = await _imageController.CreateAsync(article.Title, new List<IFormFile> { headerImg });
                        if (res)
                        {
                            _notifier.ToastSuccess("标题图片上传成功");
                        }
                        else
                        {
                            _notifier.ToastError("标题图片上传失败");
                            return RedirectToAction(nameof(Index));
                        }
                    }

                    if (contentImgs != null && contentImgs.Count > 0)
                    {
                        foreach (var img in contentImgs)
                        {
                            ArticleImgs.Add(img.FileName);
                        }
                        var res = await _imageController.CreateAsync(article.Title, contentImgs);
                        if (res)
                        {
                            _notifier.ToastSuccess("内容图片上传成功");
                        }
                        else
                        {
                            _notifier.ToastError("内容图片上传失败");
                            return RedirectToAction(nameof(Index));
                        }
                    }

                    if (ArticleImgs.Count > 0)
                        article.MediaSerialized = JsonConvert.SerializeObject(ArticleImgs);
                }

                //Add the tags
                if (!string.IsNullOrWhiteSpace(article.Tag))
                {
                    var tags = article.Tag.Split(',').ToList();
                    foreach (var tag in tags)
                    {
                        var old_tag = await _context.Tag.Where(t => t.TagName == tag).FirstOrDefaultAsync();
                        if (old_tag != null)
                        {
                            //existing tag
                            old_tag.TagCount++;
                        }
                        else
                        {
                            //new tag
                            _context.Add(new Tag() { TagName = tag, TagCount = 1 });
                        }
                    }
                }

                _context.Add(article);
                await _context.SaveChangesAsync();
                _notifier.ToastSuccess("文章创建成功");
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        [Route]
        public async Task<IActionResult> Edit(Guid id)
        {
            var article = await _context.Article.SingleOrDefaultAsync(a => a.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            var isAuthorized = await _aus.AuthorizeAsync(
                                                  User, article,
                                                  ArticleOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return View(article);
        }

        // POST: Article/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articleToUpdate = await _context.Article.SingleOrDefaultAsync(a => a.Id == id);

            if (articleToUpdate == null)
            {
                return NotFound();
            }

            var isAuthorized = await _aus.AuthorizeAsync(
                                                  User, articleToUpdate,
                                                  ArticleOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Rename the dir when Title Changes
                    //TODO: use regex to validate this Title
                    if (articleToUpdate.Title != HttpContext.Request.Form["Title"])
                    {
                        _ioHelper.MoveImgDir(articleToUpdate.Title, HttpContext.Request.Form["Title"]);
                    }

                    //Only admin can change author
                    if (User.IsInRole(Constants.AdministratorsRole))
                    {
                        await TryUpdateModelAsync(articleToUpdate, "",
                         a => a.Title, a => a.Content, a => a.Tag, a => a.Author, a => a.MediaSerialized
                        );
                    }
                    else
                    {
                        await TryUpdateModelAsync(articleToUpdate, "",
                         a => a.Title, a => a.Content, a => a.Tag, a => a.MediaSerialized
                        );
                    }

                    if (articleToUpdate.Status == Status.Approved)
                    {
                        // Reset to submitted status after update(if not mod)
                        var canApprove = await _aus.AuthorizeAsync(User,
                                                articleToUpdate,
                                                ArticleOperations.Approve);

                        if (!canApprove.Succeeded)
                        {
                            articleToUpdate.Status = Status.Submitted;
                        }
                    }

                    //prevent empty author
                    articleToUpdate.Author = articleToUpdate.Author ?? _userManager.GetUserId(User);

                    //Tag Update
                    if (articleToUpdate.Tag != HttpContext.Request.Form["Tag"])
                    {
                        List<string> old_tags = new List<string>();
                        List<string> new_tags = new List<string>();
                        if (!string.IsNullOrWhiteSpace(articleToUpdate.Tag))
                        {
                            old_tags = articleToUpdate.Tag.Split(',').ToList();
                        }
                        if (!string.IsNullOrWhiteSpace(HttpContext.Request.Form["Tag"]))
                        {
                            new_tags = ((string)HttpContext.Request.Form["Tag"]).Split(',').ToList();
                        }
                        foreach (var o_tag in old_tags)
                        {
                            //Check if in the new
                            var res = new_tags.FirstOrDefault(t => t == o_tag);
                            if (res == null)
                            {
                                //Not in new
                                //Reduce tagCount
                                var tag = await _context.Tag.Where(t => t.TagName == o_tag).FirstOrDefaultAsync();
                                --tag.TagCount;
                                if (tag.TagCount == 0)
                                {
                                    // Delete on count to zero
                                    _context.Tag.Remove(tag);
                                }
                            }
                            else
                            {
                                //Remove from new
                                new_tags.Remove(res);
                            }
                        }
                        foreach (var n_tag in new_tags)
                        {
                            //Add all new tags
                            _context.Add(new Tag() { TagName = n_tag, TagCount = 1 });
                        }
                    }

                    await _context.SaveChangesAsync();
                    _notifier.ToastSuccess("文章更新成功");
                }
                catch (Exception)
                {
                    _notifier.ToastError("文章更新失败");

                    if (!ArticleExists(id ?? 0))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(articleToUpdate);
        }

        // POST: Article/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Article.SingleOrDefaultAsync(m => m.Id == id);

            var isAuthorized = await _aus.AuthorizeAsync(
                                                 User, article,
                                                 ArticleOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            //delete Image in db
            var res = await _imageController.DeleteByPath(article.Title);
            if (!res)
            {
                _notifier.Notify("删除数据库图片时失败");
                return RedirectToAction(nameof(Index));
            }

            //delete the Images
            _ioHelper.DeleteImgDir(article.Title);

            //Comments were casecade deleted.

            //Reduce Tag Count
            if (!string.IsNullOrWhiteSpace(article.Tag))
            {
                var tags = article.Tag.Split(',').ToList();
                foreach (var tag in tags)
                {
                    var ta = await _context.Tag.Where(t => t.TagName == tag).FirstOrDefaultAsync();
                    ta.TagCount--;
                    if (ta.TagCount == 0)
                    {
                        _context.Tag.Remove(ta);
                    }
                }
            }

            //delete the article
            _context.Article.Remove(article);

            await _context.SaveChangesAsync();
            _notifier.ToastSuccess("文章删除成功");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AjaxLikeArticle(Guid? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var articleToUpdate = await _context.Article.SingleOrDefaultAsync(a => a.Id == Id);

            if (articleToUpdate == null)
            {
                return NotFound();
            }

            ++articleToUpdate.LikeCount;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogInformation("User Like Article Failed For " + Id);
                _logger.LogInformation(e.Message);
            }

            return Ok();
        }

        private bool ArticleExists(Guid id)
        {
            return _context.Article.Any(a => a.Id == id);
        }

        #region Helper

        public async Task<List<Article>> GetRelatedArticles(Article article)
        {
            //Randomly pick 3 articles
            //TODO: Actually implement this.
            var random = new Random();
            return await _context.Article.Where(a => !a.Title.Contains(Constants.ReservedTag) && a.Title != article.Title && a.Status == Status.Approved).OrderBy(s => random.Next()).Take(3).ToListAsync();
        }

        #endregion
    }
}
