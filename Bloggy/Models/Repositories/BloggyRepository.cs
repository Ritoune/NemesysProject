using Bloggy.Models.Interfaces;
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

        public IEnumerable<BlogPost> GetAllBlogPosts()
        {
            try
            {
                //Using Eager loading with Include
                //var app = _appDbContext.BlogPosts.Include(b => b.Category).OrderBy(b => b.CreatedDate);
                //return app.Include(b => b.Category).OrderBy(b => b.CreatedDate);
                return _appDbContext.BlogPosts.Include(b => b.Category).Include(b => b.Status).OrderBy(b => b.CreatedDate);
                //return _appDbContext.BlogPosts.Include(b => b.Status).OrderBy(b => b.CreatedDate);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

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

        public IEnumerable<BlogPost> GetAllBlogPostsForStatus()
        {
            try
            {
                //Using Eager loading with Include
                //return _appDbContext.BlogPosts.Include(b => b.Status).OrderBy(b => b.CreatedDate);
                return _appDbContext.BlogPosts.Include(b => b.Status).OrderBy(b => b.CreatedDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }


        public BlogPost GetBlogPostById(int blogPostId)
        {
            try
            {
                //Using Eager loading with Include
                return _appDbContext.BlogPosts.Include(b => b.Category).Include(b => b.Status).Include(b => b.User).FirstOrDefault(p => p.Id == blogPostId);
               // return _appDbContext.BlogPosts.Include(b => b.Status).Include(b => b.User).FirstOrDefault(p => p.Id == blogPostId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public Investigation GetInvestigationById(int investigationId)
        {
            try
            {
                //Using Eager loading with Include
                return _appDbContext.Investigations.Include(b => b.BlogPost).Include(b => b.BlogPost.Status).Include(b => b.BlogPost.Category).Include(b => b.User).FirstOrDefault(p => p.Id == investigationId);
                // return _appDbContext.BlogPosts.Include(b => b.Status).Include(b => b.User).FirstOrDefault(p => p.Id == blogPostId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public Investigation GetInvestigationByIdReport(int ReportId)
        {
            try
            {
                //Using Eager loading with Include
                return _appDbContext.Investigations.FirstOrDefault(p => p.BlogPostId == ReportId);
                // return _appDbContext.BlogPosts.Include(b => b.Status).Include(b => b.User).FirstOrDefault(p => p.Id == blogPostId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public BlogPost GetBlogPostByIdForStatus(int blogPostId)
        {
            try
            {
                //Using Eager loading with Include
                //return _appDbContext.BlogPosts.Include(b => b.Category).Include(b => b.User).FirstOrDefault(p => p.Id == blogPostId);
                return _appDbContext.BlogPosts.Include(b => b.Status).Include(b => b.User).FirstOrDefault(p => p.Id == blogPostId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public void DeleteReport(BlogPost post)
        {
            try
            {
                if(post.HasInvestigation == false)
                {
                    _appDbContext.BlogPosts.Remove(post);
                    _appDbContext.SaveChanges();
                }
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

        public Category GetCategoryById(int categoryId)
        {
            try
            {
                //Not loading related blog posts
                return _appDbContext.Categories.FirstOrDefault(c => c.Id == categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

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
    }
}
