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

        //This action displays all the investigations when you click on "Investigations"
        public IActionResult Index()
        {
            try
            {
                //Creation of a new entity InvestigationListViewModel which contains all InvestigationViewModel
                var model = new InvestigationListViewModel()
                {
                    TotalEntries = _bloggyRepository.GetAllInvestigations().Count(),

                    //Get all investigation from the most recent to the oldest
                    Investigations = _bloggyRepository
                    .GetAllInvestigations()
                    .OrderByDescending(b => b.DateOfAction)
                    .Select(b => new InvestigationViewModel
                    {
                        
                        Id = b.Id,
                        DescriptionOfInvestigation = b.DescriptionOfInvestigation,
                        DateOfAction = b.DateOfAction,
                        UserId = b.UserId,
                        Author = new AuthorViewModel()
                        {
                            Id = b.UserId,
                            Name = (_userManager.FindByIdAsync(b.UserId).Result != null) ? _userManager.FindByIdAsync(b.UserId).Result.UserName : "Anonymous"
                        },
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

        //This action loads and display all information about an investigation when you click on "Details"
        public IActionResult Details(int id)
        {
            try
            {
                //Creation of a variable which contains all the informations about the investigation
                var investigation = _bloggyRepository.GetInvestigationById(id);
                if (investigation == null)
                    return NotFound();
                else
                {
                    //Creation of a new entity InvestigationViewModel which contains all informations we need about the investigation
                    var model = new InvestigationViewModel()
                    {
                        Id = investigation.Id,
                        DescriptionOfInvestigation = investigation.DescriptionOfInvestigation,
                        DateOfAction = investigation.DateOfAction,
                        UserId = investigation.UserId,
                        Author = new AuthorViewModel()
                        {
                            Id = investigation.UserId,
                            Name = (_userManager.FindByIdAsync(investigation.UserId).Result != null) ? _userManager.FindByIdAsync(investigation.UserId).Result.UserName : "Anonymous"
                        },
                        BlogPostId = investigation.BlogPostId,
                        BlogPost = new BlogPostViewModel()
                        {
                            Id = investigation.BlogPost.Id,
                            CreatedDate = investigation.BlogPost.CreatedDate,
                            SpottedDate = investigation.BlogPost.SpottedDate,
                            ImageUrl = investigation.BlogPost.ImageUrl,
                            ReadCount = investigation.BlogPost.ReadCount,
                            Title = investigation.BlogPost.Title,
                            Content = investigation.BlogPost.Content,
                            HasInvestigation = investigation.BlogPost.HasInvestigation,
                            Category = new CategoryViewModel()
                            {
                                Id = investigation.BlogPost.Category.Id,
                                Name = investigation.BlogPost.Category.Name
                            },
                            Status = new StatusViewModel()
                            {
                                Id = investigation.BlogPost.Status.Id,
                                Name = investigation.BlogPost.Status.Name
                            },
                            Location = investigation.BlogPost.Location,
                            Author = new AuthorViewModel()
                            {
                                Id = investigation.BlogPost.UserId,
                                Name = (_userManager.FindByIdAsync(investigation.UserId).Result != null) ? _userManager.FindByIdAsync(investigation.UserId).Result.UserName : "Anonymous"
                            }

                        },
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

        //This action loads all informations we need to create an investigation
        [HttpGet]
        [Authorize]
        public IActionResult Create(int id)
        {
            try
            {
                //Load all reports and create a list of BlogPostViewModel
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

                //Pass the lists into an EditBlogPostViewModel, which is used by the View (all other properties may be left blank, unless you want to add other default values)
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

    
    //This action creates an investigation with a report id and the informations chosen by the user in the view Investigation/Create.cshtml
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromRoute] int id, [Bind("DescriptionOfInvestigation, BlogPostId, StatusId, HasInvestigation")] EditInvestigationViewModel newInvestigation)
    {
        try
        {
            if (ModelState.IsValid)
            {
                    //Creation of a new entity Investigation
                    Investigation investigation = new Investigation()
                    {
                        DescriptionOfInvestigation = newInvestigation.DescriptionOfInvestigation,
                        UserId = _userManager.GetUserId(User),
                        DateOfAction = DateTime.UtcNow,
                        BlogPostId = id
                    };

                    //Creation of the Investigation in the context
                    _bloggyRepository.CreateInvestigation(investigation);

                    //update the StatusId and the HasInvestigation of the report corresponding to the investigation
                    var modelToUpdate = _bloggyRepository.GetBlogPostById(id);
                    modelToUpdate.StatusId = newInvestigation.StatusId;
                    modelToUpdate.HasInvestigation = true;
                    _bloggyRepository.UpdateBlogPost(modelToUpdate);

                    return RedirectToAction("Index");
            }
            else
            {
                    //Load all reports and create a list of BlogPostViewModel
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
                    //Load all reports and create a list of BlogPostViewModel
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

        //This action loads all information we need to display the view Investigation/Edit.cshtml and to edit an investigation
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                //Creation of a new variable which contains all informations about the investigation
                var existingInvestigation = _bloggyRepository.GetInvestigationById(id);
                if (existingInvestigation != null)
                {
                    //Check if the current user has access to this resource
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (existingInvestigation.User.Id == currentUser.Id)
                    {
                        EditInvestigationViewModel model = new EditInvestigationViewModel()
                        {
                            Id = existingInvestigation.Id,
                            DescriptionOfInvestigation = existingInvestigation.DescriptionOfInvestigation,
                            DateOfAction = existingInvestigation.DateOfAction,
                            UserId = existingInvestigation.UserId
                        };


                        //Load all status and create a list of StatusViewModel
                        var statusList = _bloggyRepository.GetAllStatus().Select(c => new StatusViewModel()
                        {
                            Id = c.Id,
                            Name = c.Name
                        }).ToList();

                        //Attach to view model - view will pre-select according to the value in StatusId
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

        //This action edits an investigation with a report id and the informations chosen by the user in the view Investigation/Edit.cshtml
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, [Bind("Id, DescriptionOfInvestigation, DateOfAction, UserId, StatusId")] EditInvestigationViewModel updatedInvestigation)
        {
            try
            {
                //Creation of a variable which contains all informations about the investigation
                var modelToUpdateInv = _bloggyRepository.GetInvestigationById(id);

                //Creation of a variable which contains all informations about the report corresponding to the investigation
                var modelToUpdateReport = _bloggyRepository.GetBlogPostById(modelToUpdateInv.BlogPostId);
                if (modelToUpdateInv == null)
                {
                    return NotFound();
                }

                //Check if the current user has access to this resource
                var currentUser = await _userManager.GetUserAsync(User);
                if (modelToUpdateInv.User.Id == currentUser.Id)
                {
                    if (ModelState.IsValid)
                    {
                        //Edits the attributes of the investigations
                        modelToUpdateInv.DescriptionOfInvestigation = updatedInvestigation.DescriptionOfInvestigation;
                        
                        _bloggyRepository.UpdateInvestigation(modelToUpdateInv);

                        //Edits the attributes of the report corresponding to the investigation 
                        modelToUpdateReport.StatusId = updatedInvestigation.StatusId;
                        _bloggyRepository.UpdateBlogPost(modelToUpdateReport);

                        return RedirectToAction("Index");
                    }
                    else
                        return Unauthorized(); 
                }
                else
                {
                  

                    //Load all status and create a list of StatusViewModel
                    var statusList = _bloggyRepository.GetAllStatus().Select(c => new StatusViewModel()
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList();

                    //Re-attach to view model before sending back to the View (this is necessary so that the View can repopulate the drop down and pre-select according to the StatusId
                    updatedInvestigation.StatusList = statusList;

                    return View(updatedInvestigation);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return View("Error");
            }
        }
    }
}
