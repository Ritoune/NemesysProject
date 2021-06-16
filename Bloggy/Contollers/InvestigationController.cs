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
    public class InvestigationController : Controller
    {
        private readonly IBloggyRepository _bloggyRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<BlogPostController> _logger;

        public InvestigationController(
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
                var model = new InvestigationListViewModel()
                {
                    TotalEntries = _bloggyRepository.GetAllInvestigations().Count(),
                    Investigations = _bloggyRepository
                    .GetAllInvestigations()
                    .OrderByDescending(b => b.DateOfAction)
                    .Select(b => new InvestigationViewModel
                    {
                        
                        Id = b.Id,
                        DescriptionOfInvestigation = b.DescriptionOfInvestigation,
                        DateOfAction = b.DateOfAction,
                        UserId = b.UserId,
                        BlogPost = new BlogPostViewModel()
                        {
                            Id = b.BlogPostId,
                            Title = b.BlogPost.Title,
                            SpottedDate = b.BlogPost.SpottedDate,
                            CreatedDate = b.BlogPost.CreatedDate,
                            Location = b.BlogPost.Location,
                            Content = b.BlogPost.Content,
                            Category = new CategoryViewModel()
                            {
                                Id = b.BlogPost.Category.Id,
                                Name = b.BlogPost.Category.Name
                            },
                            Status = new StatusViewModel()
                            {
                                Id = b.BlogPost.Status.Id,
                                Name = b.BlogPost.Status.Name
                            },
                            Author = new AuthorViewModel()
                            {
                                Id = b.BlogPost.UserId,
                                Name = (_userManager.FindByIdAsync(b.BlogPost.UserId).Result != null) ? _userManager.FindByIdAsync(b.BlogPost.UserId).Result.UserName : "Anonymous"
                            },

                            ImageUrl = b.BlogPost.ImageUrl,

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
    

        [HttpGet]
        [Authorize]
        public IActionResult Create(int id)
        {
            try
            {
                //Load all categories and create a list of CategoryViewModel
                var blogpostList = _bloggyRepository.GetAllBlogPosts().Select(c => new BlogPostViewModel()
                {
                    Id = c.Id,
                    Title = c.Title,
                    Content = c.Content,
                    CreatedDate = c.CreatedDate,
                    SpottedDate = c.SpottedDate,
                    ImageUrl = c.ImageUrl,
                    ReadCount = c.ReadCount,
                    StatusId = c.StatusId,
                    CategoryId = c.CategoryId,
                    Location = c.Location,
                    UserId = _userManager.GetUserId(User)
                }).ToList();

                //Load all categories and create a list of CategoryViewModel
                var statusList = _bloggyRepository.GetAllStatus().Select(c => new StatusViewModel()
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList();

                //Pass the list into an EditBlogPostViewModel, which is used by the View (all other properties may be left blank, unless you want to add other default values
                var model = new EditInvestigationViewModel()
                {
                    BlogPostList = blogpostList,
                    StatusList = statusList
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
    public async Task<IActionResult> Create([FromRoute] int id, [Bind("DescriptionOfInvestigation, BlogPostId, StatusId")] EditInvestigationViewModel newInvestigation)
    {
        try
        {
            if (ModelState.IsValid)
            {
                    //var currentUser = await _userManager.GetUserAsync(User);
                    Investigation investigation = new Investigation()
                    {
                        DescriptionOfInvestigation = newInvestigation.DescriptionOfInvestigation,
                        UserId = _userManager.GetUserId(User),
                        DateOfAction = DateTime.UtcNow,
                        BlogPostId = id
                    };

                    _bloggyRepository.CreateInvestigation(investigation);
                    var modelToUpdate = _bloggyRepository.GetBlogPostById(id);
                    modelToUpdate.StatusId = newInvestigation.StatusId;
                    _bloggyRepository.UpdateBlogPost(modelToUpdate);

                    return RedirectToAction("Index");
            }
            else
            {
                    //Load all categories and create a list of CategoryViewModel
                    var blogpostList = _bloggyRepository.GetAllBlogPosts().Select(c => new BlogPostViewModel()
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Content = c.Content,
                        CreatedDate = c.CreatedDate,
                        SpottedDate = c.SpottedDate,
                        ImageUrl = c.ImageUrl,
                        ReadCount = c.ReadCount,
                        StatusId = c.StatusId,
                        CategoryId = c.CategoryId,
                        Location = c.Location,
                        UserId = _userManager.GetUserId(User)
                    }).ToList();

                    //Pass the list into an EditBlogPostViewModel, which is used by the View (all other properties may be left blank, unless you want to add other default values
                    var model = new EditInvestigationViewModel()
                    {
                        BlogPostList = blogpostList
                    };

                    var statusList = _bloggyRepository.GetAllStatus().Select(c => new StatusViewModel()
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList();
                    model.StatusList = statusList;

                    return View(model);
                }
            }
        catch (DbUpdateException e)
        {
            SqlException s = e.InnerException as SqlException;
            if (s != null)
            {
                switch (s.Number)
                {
                    case 547:  //Unique constraint error
                        {
                            ModelState.AddModelError(string.Empty, string.Format("Foreign key for category with Id '{0}' does not exists.", newInvestigation.BlogPostId));
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
                    var blogpostList = _bloggyRepository.GetAllBlogPosts().Select(c => new BlogPostViewModel()
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Content = c.Content,
                        CreatedDate = c.CreatedDate,
                        SpottedDate = c.SpottedDate,
                        ImageUrl = c.ImageUrl,
                        ReadCount = c.ReadCount,
                        StatusId = c.StatusId,
                        CategoryId = c.CategoryId,
                        Location = c.Location,
                        UserId = _userManager.GetUserId(User)
                    }).ToList();

                    //Re-attach to view model before sending back to the View (this is necessary so that the View can repopulate the drop down and pre-select according to the CategoryId
                    newInvestigation.BlogPostList = blogpostList;

            }

            return View(newInvestigation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message, ex.Data);
            return View("Error");
        }
    }
}
}
