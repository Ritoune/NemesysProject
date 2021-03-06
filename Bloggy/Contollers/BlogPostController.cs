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

        //This action displays all reports when you click on "Reports".
        public IActionResult Index()
        {
            try
            {
                //Creation of a new BlogPostListViewModel, which contains all reports, and that will be displayed
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
                        SpottedDate = b.SpottedDate,
                        Content = b.Content,
                        ImageUrl = b.ImageUrl,
                        ReadCount = b.ReadCount,
                        Title = b.Title,
                        HasInvestigation = b.HasInvestigation,
                        Category = new CategoryViewModel()
                        {
                            Id = b.Category.Id,
                            Name = b.Category.Name
                        },
                        Status = new StatusViewModel()
                        {
                            Id = b.Status.Id,
                            Name = b.Status.Name
                        },
                        Location = b.Location,
                        Author = new AuthorViewModel()
                        {
                            Id = b.UserId,
                            Name = (_userManager.FindByIdAsync(b.UserId).Result != null) ? _userManager.FindByIdAsync(b.UserId).Result.UserName : "Anonymous"
                        },

                    }),
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

        //This action displays all the informations about a report, when you click on Details 
        public IActionResult Details(int id)
        {
            try
            {
                //Creation of variable which contains a report and all of his informations
                var post = _bloggyRepository.GetBlogPostById(id);

                // Verify if he already Like the post or not
                var getUpVote = _bloggyRepository.GetUpVoteByReportIdAndUserId(id, _userManager.GetUserId(User));
                bool existingUpVote = false;
                if (getUpVote != null)
                {
                    existingUpVote = true;
                }

                //If there is no corresponding report, it displays a view of error
                if (post == null)
                    return NotFound();
                //
                else
                {
                    //Creation of a new BlogPostViewModel, which contains all the informations about a report 
                    var model = new BlogPostViewModel()
                    {
                        Id = post.Id,
                        CreatedDate = post.CreatedDate,
                        SpottedDate = post.SpottedDate,
                        ImageUrl = post.ImageUrl,
                        ReadCount = post.ReadCount,
                        Title = post.Title,
                        Content = post.Content,
                        HasInvestigation = post.HasInvestigation,
                        HasUpvote = existingUpVote,
                        Category = new CategoryViewModel()
                        {
                            Id = post.Category.Id,
                            Name = post.Category.Name
                        },
                        Status = new StatusViewModel()
                        {
                            Id = post.Status.Id,
                            Name = post.Status.Name
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

        //This action creates all informations we need to display the Create View
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

        //This action creates a new report with the informations chosen by the user in the view BlogPost/Create.cshtml
        //In Bind : all parameters that will change during the creation of the report 
        [HttpPost]
        [Authorize]
        public IActionResult Create([Bind("Title, Content, ImageToUpload, Location, StatusId, SpottedDate, CategoryId, HasInvestigation")] EditBlogPostViewModel newBlogPost)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //This part is used for the display of the picture
                    string fileName = "";
                    if (newBlogPost.ImageToUpload != null)
                    {
                        var extension = "." + newBlogPost.ImageToUpload.FileName.Split('.')[newBlogPost.ImageToUpload.FileName.Split('.').Length - 1];
                        fileName = Guid.NewGuid().ToString() + extension;
                        var path = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\blogposts\\" + fileName;
                        using (var bits = new FileStream(path, FileMode.Create))
                        {
                            newBlogPost.ImageToUpload.CopyTo(bits);
                        }
                    }

                    //Creation of a new entity BlogPost with all his informations
                    BlogPost blogPost = new BlogPost()
                    {
                        Title = newBlogPost.Title,
                        Content = newBlogPost.Content,
                        CreatedDate = DateTime.UtcNow,
                        SpottedDate = DateTime.UtcNow,
                        ImageUrl = "/images/blogposts/" + fileName,
                        ReadCount = 0,
                        StatusId = 1,
                        CategoryId = newBlogPost.CategoryId,
                        Location = newBlogPost.Location,
                        UserId = _userManager.GetUserId(User),
                        HasInvestigation = false
                    };

                    //Creation of the report
                    _bloggyRepository.CreateBlogPost(blogPost);

                    //View "Index"
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

                    //Re-attach to view model before sending back to the View (this is necessary so that the View can repopulate the drop down and pre-select according to the CategoryId)
                    newBlogPost.CategoryList = categoryList;

                    //Load all status and create a list of StatusViewModel
                    var statusList = _bloggyRepository.GetAllStatus().Select(c => new StatusViewModel()
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList();

                    //Re-attach to view model before sending back to the View (this is necessary so that the View can repopulate the drop down and pre-select according to the StatusId)
                    newBlogPost.StatusList = statusList;

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

                    //Re-attach to view model before sending back to the View (this is necessary so that the View can repopulate the drop down and pre-select according to the CategoryId)
                    newBlogPost.CategoryList = categoryList;

                    //Load all status and create a list of StatusViewModel
                    var statusList = _bloggyRepository.GetAllStatus().Select(c => new StatusViewModel()
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList();

                    //Re-attach to view model before sending back to the View (this is necessary so that the View can repopulate the drop down and pre-select according to the StatusId)
                    newBlogPost.StatusList = statusList;
                }

                return View(newBlogPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

        //This action loads all informations we need to edit a report 
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                //Creation of a variable which contains a report 
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
                            SpottedDate = existingBlogPost.SpottedDate,
                            ImageUrl = existingBlogPost.ImageUrl,
                            CategoryId = existingBlogPost.CategoryId,
                            Location = existingBlogPost.Location,
                            HasInvestigation = existingBlogPost.HasInvestigation,
                        };

                        //Load all categories and create a list of CategoryViewModel
                        var categoryList = _bloggyRepository.GetAllCategories().Select(c => new CategoryViewModel()
                        {
                            Id = c.Id,
                            Name = c.Name
                        }).ToList();

                        //Attach to view model - view will pre-select according to the value in CategoryId
                        model.CategoryList = categoryList;

                        //Load all status and create a list of StatusViewModel
                        var statusList = _bloggyRepository.GetAllStatus().Select(c => new StatusViewModel()
                        {
                            Id = c.Id,
                            Name = c.Name
                        }).ToList();

                        //Attach to view model - view will pre-select according to the value in StatusId)
                        model.StatusList = statusList;

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

        //This action edits a report with all informations chosen by the user in the view BlogPost/Edit.cshtml
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, [Bind("Id, Title, Content, ImageToUpload, CategoryId, Location, SpottedDate")] EditBlogPostViewModel updatedBlogPost)
        {
            try {
                //Creation of a variable which contains all previous informations about the report 
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
                        //Display picture
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

                        //Informations update
                        modelToUpdate.Title = updatedBlogPost.Title;
                        modelToUpdate.Content = updatedBlogPost.Content;
                        modelToUpdate.ImageUrl = imageUrl;
                        modelToUpdate.UpdatedDate = DateTime.Now;
                        modelToUpdate.SpottedDate = DateTime.Now;
                        modelToUpdate.CategoryId = updatedBlogPost.CategoryId;
                        modelToUpdate.UserId = _userManager.GetUserId(User);
                        modelToUpdate.Location = updatedBlogPost.Location;

                        //Edit report
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

                    //Load all status and create a list of StatusViewModel
                    var statusList = _bloggyRepository.GetAllStatus().Select(c => new StatusViewModel()
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList();

                    //Re-attach to view model before sending back to the View (this is necessary so that the View can repopulate the drop down and pre-select according to the StatusId)
                    updatedBlogPost.StatusList = statusList;

                    return View(updatedBlogPost);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

        //This action deletes a report 
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = _bloggyRepository.GetBlogPostById(id);
            _bloggyRepository.DeleteReport(report);

            return RedirectToAction("Index");
        }

        //This action creates the Hall Of Fame table 
        [HttpGet]
        [Authorize]
        public IActionResult HallOfFame()
        {
            try
            {
                
                //creation a variable HallOfFameListViewModel which contains all lines of the table 
                var model = new HallOfFameListViewModel();
                model = new HallOfFameListViewModel()
                {
                    HallOfFames = _bloggyRepository.GetHallOfFames().OrderByDescending(b=>b.NumberOfReports),
                  
                };
                //Creation of the Author of each line from de table Hall Of Fame
                foreach(var fame in model.HallOfFames)
                {
                    fame.Author = new AuthorViewModel()
                    {
                        Id = fame.UserId,
                        Name = (_userManager.FindByIdAsync(fame.UserId).Result != null) ? _userManager.FindByIdAsync(fame.UserId).Result.UserName : "Anonymous"
                    };
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }

        //This action gives to the user the possibility to upvote only one time a report 
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UpVote(int id)
        {
            try
            {
                Upvotes existingUpVote = _bloggyRepository.GetUpVoteByReportIdAndUserId(id, _userManager.GetUserId(User));

                BlogPost reportToUpdate = _bloggyRepository.GetBlogPostById(id);
                if (reportToUpdate == null)
                {
                    return NotFound();
                }

                //if the user didn't yet upvoted this report, he can upvotes
                if (existingUpVote == null)
                {
                    int NumberOfVote = reportToUpdate.Votes;

                    reportToUpdate.Votes += 1;
                    _bloggyRepository.UpdateBlogPost(reportToUpdate);

                    //creation of a new entity Upvotes
                    Upvotes upvote = new Upvotes()
                    {
                        BlogPostId = id,
                        UserId = _userManager.GetUserId(User)
                    };

                    //Creation of the upvote in the global context
                    _bloggyRepository.CreateUpVote(upvote);
                }

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }
    }
}
