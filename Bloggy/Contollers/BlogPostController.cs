using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bloggy.Models;
using Bloggy.Models.Interfaces;
using Bloggy.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bloggy.Contollers
{
    public class BlogPostController : Controller
    {
        private readonly IBloggyRepository _bloggyRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<BlogPostController> _logger;

        public BlogPostController(
            IBloggyRepository bloggyRepository, 
            UserManager<ApplicationUser> userManager,
            ILogger<BlogPostController> logger)
        {
            _bloggyRepository = bloggyRepository;
            _userManager = userManager;
            _logger = logger;
        }


        public IActionResult Index()
        {
            try
            {
                var model = new BlogPostListViewModel()
                {
                    TotalEntries = _bloggyRepository.GetAllBlogPosts().Count(),
                    BlogPosts = _bloggyRepository
                    .GetAllBlogPosts()
                    .OrderByDescending(b => b.CreatedDate)
                    .Select(b => new BlogPostViewModel
                    {
                        Id = b.Id,
                        CreatedDate = b.CreatedDate,
                        Content = b.Content,
                        ImageUrl = b.ImageUrl,
                        ReadCount = b.ReadCount,
                        Title = b.Title,
                        Category = new CategoryViewModel()
                        {
                            Id = b.Category.Id,
                            Name = b.Category.Name
                        },
                        Location = b.Location,
                        Author = new AuthorViewModel()
                        {
                            Id = b.UserId,
                            Name = (_userManager.FindByIdAsync(b.UserId).Result != null) ? _userManager.FindByIdAsync(b.UserId).Result.UserName : "Anonymous"
                        }


                    })
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

        public IActionResult Details(int id)
        {
            try
            {
                var post = _bloggyRepository.GetBlogPostById(id);
                if (post == null)
                    return NotFound();
                else
                {
                    var model = new BlogPostViewModel()
                    {
                        Id = post.Id,
                        CreatedDate = post.CreatedDate,
                        ImageUrl = post.ImageUrl,
                        ReadCount = post.ReadCount,
                        Title = post.Title,
                        Content = post.Content,
                        Category = new CategoryViewModel()
                        {
                            Id = post.Category.Id,
                            Name = post.Category.Name
                        },
                        Location = post.Location,
                        Author = new AuthorViewModel()
                        {
                            Id = post.UserId,
                            Name = (_userManager.FindByIdAsync(post.UserId).Result != null) ? _userManager.FindByIdAsync(post.UserId).Result.UserName : "Anonymous"
                        }
                    };

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }

        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            try
            {
                //Load all categories and create a list of CategoryViewModel
                var categoryList = _bloggyRepository.GetAllCategories().Select(c => new CategoryViewModel()
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList();

                //Pass the list into an EditBlogPostViewModel, which is used by the View (all other properties may be left blank, unless you want to add other default values
                var model = new EditBlogPostViewModel()
                {
                    CategoryList = categoryList
                };

                //Pass model to View
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create([Bind("Title, Content, ImageToUpload, CategoryId, Location, StatusId")] EditBlogPostViewModel newBlogPost)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string fileName = "";
                    if (newBlogPost.ImageToUpload != null)
                    {
                        //At this point you should check size, extension etc...
                        //Then persist using a new name for consistency (e.g. new Guid)
                        var extension = "." + newBlogPost.ImageToUpload.FileName.Split('.')[newBlogPost.ImageToUpload.FileName.Split('.').Length - 1];
                        fileName = Guid.NewGuid().ToString() + extension;
                        var path = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\blogposts\\" + fileName;
                        using (var bits = new FileStream(path, FileMode.Create))
                        {
                            newBlogPost.ImageToUpload.CopyTo(bits);
                        }
                    }

                    BlogPost blogPost = new BlogPost()
                    {
                        Title = newBlogPost.Title,
                        Content = newBlogPost.Content,
                        CreatedDate = DateTime.UtcNow,
                        ImageUrl = "/images/blogposts/" + fileName,
                        ReadCount = 0,
                        StatusId = 1,
                        CategoryId = newBlogPost.CategoryId,
                        Location = newBlogPost.Location,
                        UserId = _userManager.GetUserId(User)
                    };

                    _bloggyRepository.CreateBlogPost(blogPost);
                    return RedirectToAction("Index");
                }
                else
                {
                    //Load all categories and create a list of CategoryViewModel
                    var categoryList = _bloggyRepository.GetAllCategories().Select(c => new CategoryViewModel()
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList();

                    //Re-attach to view model before sending back to the View (this is necessary so that the View can repopulate the drop down and pre-select according to the CategoryId
                    newBlogPost.CategoryList = categoryList;

                    return View(newBlogPost);
                }
            }
            catch(DbUpdateException e)
            {
                SqlException s = e.InnerException as SqlException;
                if (s != null)
                {
                    switch (s.Number)
                    {
                        case 547:  //Unique constraint error
                            {
                                ModelState.AddModelError(string.Empty, string.Format("Foreign key for category with Id '{0}' does not exists.", newBlogPost.CategoryId));
                                break;
                            }
                        default:
                            {
                                ModelState.AddModelError(string.Empty,
                                 "A database error occured - please contact your system administrator.");
                                break;
                            }
                    }
                    //Load all categories and create a list of CategoryViewModel
                    var categoryList = _bloggyRepository.GetAllCategories().Select(c => new CategoryViewModel()
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList();

                    //Re-attach to view model before sending back to the View (this is necessary so that the View can repopulate the drop down and pre-select according to the CategoryId
                    newBlogPost.CategoryList = categoryList;
                }

                return View(newBlogPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var existingBlogPost = _bloggyRepository.GetBlogPostById(id);
                if (existingBlogPost != null)
                {
                    //Check if the current user has access to this resource
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (existingBlogPost.User.Id == currentUser.Id)
                    {
                        EditBlogPostViewModel model = new EditBlogPostViewModel()
                        {
                            Id = existingBlogPost.Id,
                            Title = existingBlogPost.Title,
                            Content = existingBlogPost.Content,
                            ImageUrl = existingBlogPost.ImageUrl,
                            CategoryId = existingBlogPost.CategoryId,
                            Location = existingBlogPost.Location,
                        };

                        //Load all categories and create a list of CategoryViewModel
                        var categoryList = _bloggyRepository.GetAllCategories().Select(c => new CategoryViewModel()
                        {
                            Id = c.Id,
                            Name = c.Name
                        }).ToList();

                        //Attach to view model - view will pre-select according to the value in CategoryId
                        model.CategoryList = categoryList;

                        return View(model);
                    }
                    else
                        return Unauthorized();
                }
                else
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, [Bind("Id, Title, Content, ImageToUpload, CategoryId, Location")] EditBlogPostViewModel updatedBlogPost)
        {
            try {
                var modelToUpdate = _bloggyRepository.GetBlogPostById(id);
                if (modelToUpdate == null)
                {
                    return NotFound();
                }

                //Check if the current user has access to this resource
                var currentUser = await _userManager.GetUserAsync(User);
                if (modelToUpdate.User.Id == currentUser.Id)
                {
                    if (ModelState.IsValid)
                    {

                        string imageUrl = "";

                        if (updatedBlogPost.ImageToUpload != null)
                        {
                            string fileName = "";
                            //At this point you should check size, extension etc...
                            //Then persist using a new name for consistency (e.g. new Guid)
                            var extension = "." + updatedBlogPost.ImageToUpload.FileName.Split('.')[updatedBlogPost.ImageToUpload.FileName.Split('.').Length - 1];
                            fileName = Guid.NewGuid().ToString() + extension;
                            var path = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\blogposts\\" + fileName;
                            using (var bits = new FileStream(path, FileMode.Create))
                            {
                                updatedBlogPost.ImageToUpload.CopyTo(bits);
                            }
                            imageUrl = "/images/blogposts/" + fileName;
                        }
                        else
                            imageUrl = modelToUpdate.ImageUrl;

                        modelToUpdate.Title = updatedBlogPost.Title;
                        modelToUpdate.Content = updatedBlogPost.Content;
                        modelToUpdate.ImageUrl = imageUrl;
                        modelToUpdate.UpdatedDate = DateTime.Now;
                        modelToUpdate.CategoryId = updatedBlogPost.CategoryId;
                        modelToUpdate.UserId = _userManager.GetUserId(User);
                        modelToUpdate.Location = updatedBlogPost.Location;

                        _bloggyRepository.UpdateBlogPost(modelToUpdate);

                        return RedirectToAction("Index");
                    }
                    else
                        return Unauthorized(); //or redirect to error controller with 401/403 actions
                }
                else
                {
                    //Load all categories and create a list of CategoryViewModel
                    var categoryList = _bloggyRepository.GetAllCategories().Select(c => new CategoryViewModel()
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList();

                    //Re-attach to view model before sending back to the View (this is necessary so that the View can repopulate the drop down and pre-select according to the CategoryId
                    updatedBlogPost.CategoryList = categoryList;

                    return View(updatedBlogPost);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

        [HttpGet]
        [Authorize(Roles ="Administrator")]
        public IActionResult Dashboard()
        {
            try
            {
                ViewBag.Title = "Bloggy Dashboard";

                var model = new BlogDashboardViewModel();
                model.TotalRegisteredUsers = _userManager.Users.Count();
                model.TotalEntries = _bloggyRepository.GetAllBlogPosts().Count();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }
    }
}
