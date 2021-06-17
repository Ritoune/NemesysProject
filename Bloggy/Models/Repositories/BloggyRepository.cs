using Bloggy.Models.Interfaces;
using Bloggy.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bloggy.Models.Repositories
{
    public class BloggyRepository : IBloggyRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger _logger;

        public BloggyRepository(AppDbContext appDbContext, ILogger<BloggyRepository> logger)
        {
            try
            {
                _appDbContext = appDbContext;
                _logger = logger;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        //This method return all reports
        public IEnumerable<BlogPost> GetAllBlogPosts()
        {
            try
            {
                return _appDbContext.BlogPosts.Include(b => b.Category).Include(b => b.Status).OrderBy(b => b.CreatedDate);
                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        //This method returns all investigations
        public IEnumerable<Investigation> GetAllInvestigations()
        {
            try
            {
                //Using Eager loading with Include
                //var app = _appDbContext.BlogPosts.Include(b => b.Category).OrderBy(b => b.CreatedDate);
                //return app.Include(b => b.Category).OrderBy(b => b.CreatedDate);
                return _appDbContext.Investigations.Include(b => b.BlogPost).Include(b => b.BlogPost.Status).Include(b => b.BlogPost.Category).OrderBy(b => b.DateOfAction);
                //return _appDbContext.BlogPosts.Include(b => b.Status).OrderBy(b => b.CreatedDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }



        //This method return the report corresponding to an id 
        public BlogPost GetBlogPostById(int blogPostId)
        {
            try
            {
                return _appDbContext.BlogPosts.Include(b => b.Category).Include(b => b.Status).Include(b => b.User).FirstOrDefault(p => p.Id == blogPostId);
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        //This method returns the investigation corresponding to an id
        public Investigation GetInvestigationById(int investigationId)
        {
            try
            {
                return _appDbContext.Investigations.Include(b => b.BlogPost).Include(b => b.BlogPost.Status).Include(b => b.BlogPost.Category).Include(b => b.User).FirstOrDefault(p => p.Id == investigationId);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        //This method returns the investigation corresponding to a report id
        public Investigation GetInvestigationByIdReport(int ReportId)
        {
            try
            {
                return _appDbContext.Investigations.FirstOrDefault(p => p.BlogPostId == ReportId);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }


        //This methode deletes a report
        public void DeleteReport(BlogPost post)
        {
            try
            {
                //If there is no investigation corresponding to the report, it deletes only the report
                if(post.HasInvestigation == false)
                {
                    _appDbContext.BlogPosts.Remove(post);
                    _appDbContext.SaveChanges();
                }
                //If there is an investigation corresponding to the report, it deletesthe report and the investigation
                else
                {
                    var investigation = _appDbContext.Investigations.FirstOrDefault(p => p.BlogPostId == post.Id);
                    _appDbContext.Investigations.Remove(investigation);
                    _appDbContext.BlogPosts.Remove(post);

                    _appDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        //This method deletes an investigation from an investigation id
        public void DeleteInvestigationById(int id)
        {
            try
            {
                var investigation = _appDbContext.Investigations.FirstOrDefault(p => p.Id == id);

                _appDbContext.Investigations.Remove(investigation);
                 _appDbContext.SaveChanges();
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        //This methodes creates a report
        public void CreateBlogPost(BlogPost blogPost)
        {
            try
            {
                _appDbContext.BlogPosts.Add(blogPost);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        //This method creates an investigation
        public void CreateInvestigation(Investigation investigation)
        {
            try
            {
                _appDbContext.Investigations.Add(investigation);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        //This method updates a report
        public void UpdateBlogPost(BlogPost blogPost)
        {
            try
            {
                var existingBlogPost = _appDbContext.BlogPosts.SingleOrDefault(bp => bp.Id == blogPost.Id);
                if (existingBlogPost != null)
                {
                    existingBlogPost.Title = blogPost.Title;
                    existingBlogPost.Content = blogPost.Content;
                    existingBlogPost.UpdatedDate = blogPost.UpdatedDate;
                    existingBlogPost.SpottedDate = blogPost.SpottedDate;
                    existingBlogPost.ImageUrl = blogPost.ImageUrl;
                    existingBlogPost.CategoryId = blogPost.CategoryId;
                    existingBlogPost.UserId = blogPost.UserId;
                    existingBlogPost.Location = blogPost.Location;

                    _appDbContext.Entry(existingBlogPost).State = EntityState.Modified;
                    _appDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        //This method updates an investigation
        public void UpdateInvestigation(Investigation investigation)
        {
            try
            {
                var existingInvestigation = _appDbContext.Investigations.SingleOrDefault(bp => bp.Id == investigation.Id);
                if (existingInvestigation != null)
                {
                    existingInvestigation.DescriptionOfInvestigation = investigation.DescriptionOfInvestigation;
                   // existingInvestigation.DateOfAction = investigation.DateOfAction;

                    _appDbContext.Entry(existingInvestigation).State = EntityState.Modified;
                    _appDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        //This method return all categories
        public IEnumerable<Category> GetAllCategories()
        {
            try
            {
                //Not loading related blog posts
                return _appDbContext.Categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        //This method returns a category corresponding to a catgory id
        public Category GetCategoryById(int categoryId)
        {
            try
            {
                return _appDbContext.Categories.FirstOrDefault(c => c.Id == categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        //This method returns all status
        public IEnumerable<Status> GetAllStatus()
        {
            try
            {
                //Not loading related blog posts
                return _appDbContext.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        //This method returns the status corresponding to a status id
        public Status GetStatusById(int statusId)
        {
            try
            {
                //Not loading related blog posts
                return _appDbContext.Status.FirstOrDefault(c => c.Id == statusId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        //This method returns an upvote corresponding to a report id and a user id
        public Upvotes GetUpVoteByReportIdAndUserId(int reportId, string userId)
        {
            try
            {
                //Not loading related report posts
                return _appDbContext.Upvotes.FirstOrDefault(c => c.BlogPostId == reportId && c.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        //This method creates an upvote
        public void CreateUpVote(Upvotes upvote)
        {
            try
            {
                _appDbContext.Upvotes.Add(upvote);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        //This methods return all data for the Hall Of Fame
        public IEnumerable<HallOfFameViewModel> GetHallOfFames()
        {
                try
                {
                    IEnumerable<BlogPost> AllReports = _appDbContext.BlogPosts.Include(b => b.User).Where(c => c.CreatedDate > DateTime.UtcNow.AddYears(-1));
                    IEnumerable<string> ListUsersId = AllReports.Select(d => d.UserId).Distinct();
                    
                    //Creation of new list of HallOfFameViewModel entities, which are the lines of the tables 
                    List<HallOfFameViewModel> HallOfFames = new List<HallOfFameViewModel>();

                    //Creation of the entities HallOfFameViewModel
                    foreach(var userId in ListUsersId)
                    {
                        var HallOfFame = new HallOfFameViewModel()
                        {
                            UserId = userId,
                            NumberOfReports = 0
                        };
                        HallOfFames.Add(HallOfFame);
                    }

                    //Here we change the number of reports for each Hall Of Fame entities
                    foreach(var fame in HallOfFames)
                    {
                        foreach(var report in AllReports)
                        {
                            if(report.UserId==fame.UserId)
                            {
                                fame.NumberOfReports += 1;
                            }
                        }
                    }
                    return HallOfFames;

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    throw;
                }
        }
    }
}

