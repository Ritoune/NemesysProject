using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bloggy.Models.Interfaces;

namespace Bloggy.Models.Repositories
{
    //WRAP ALL METHODS IN TRY CATCH BLOCKS
    public class MockBloggyRepository : IBloggyRepository
    {
        private List<BlogPost> _posts;
        private List<Category> _categories;
        private List<Status> _status;

        public MockBloggyRepository()
        {
            if (_categories == null)
            {
                InitializeCategories();
            }

            if (_status == null)
            {
                InitializeCategories();
            }

            if (_posts == null)
            {
                InitializeBlogPosts();
            }

        }
        private void InitializeCategories()
        {
            _categories = new List<Category>()
            {
                new Category()
                {
                    Id = 1,
                    Name = "Comedy"
                },
                new Category()
                {
                    Id = 2,
                    Name = "News"
                }
            };
        }

        private void InitializeStatus()
        {
            _status = new List<Status>()
            {
                new Status()
                {
                    Id = 1,
                    Name = "Opened"
                },
                new Status()
                {
                    Id = 2,
                    Name = "Closed"
                },
                new Status()
                {
                    Id = 3,
                    Name = "Being investigated"
                },
                 new Status()
                {
                    Id = 4,
                    Name = "No action required"
                }
            };
        }

        private void InitializeBlogPosts()
        {
            _posts = new List<BlogPost>()
            {
                new BlogPost()
                {
                    Id = 1,
                    Title = "AGA Today",
                    Content = "Today's AGA is characterized by a series of discussions and debates around ...",
                    CreatedDate = DateTime.UtcNow,
                    SpottedDate = DateTime.UtcNow,
                    ImageUrl = "/images/seed1.jpg",
                    CategoryId = 1,
                    StatusId = 1,
                    Location = "parking",

                },
                new BlogPost()
                {
                    Id = 2,
                    Title = "Traffic is incredible",
                    Content = "Today's traffic can't be described using words. Only an image can do that ...",
                    CreatedDate = DateTime.UtcNow.AddDays(-1),
                    SpottedDate = DateTime.UtcNow.AddDays(-1),
                    ImageUrl = "/images/seed2.jpg",
                    CategoryId = 2,
                    StatusId = 1,
                    Location = "At the entrance",
                },
                new BlogPost()
                {
                    Id = 3,
                    Title = "When is Spring really starting?",
                    Content = "Clouds clouds all around us. I thought spring started already, but ...",
                    CreatedDate = DateTime.UtcNow.AddDays(-2),
                    SpottedDate = DateTime.UtcNow.AddDays(-2),
                    ImageUrl = "/images/seed3.jpg",
                    CategoryId = 2,
                    StatusId = 1,
                    Location = "In the library",
                }
            };
        }

        public IEnumerable<BlogPost> GetAllBlogPosts()
        {
            List<BlogPost> result = new List<BlogPost>();
            foreach (var post in _posts)
            {
                post.Category = _categories.FirstOrDefault(c => c.Id == post.CategoryId);
                //post.Status = _status.FirstOrDefault(c => c.Id == post.StatusId);
                result.Add(post);
            }
            return result;
        }

        public IEnumerable<BlogPost> GetAllBlogPostsForStatus()
        {
            List<BlogPost> result = new List<BlogPost>();
            foreach (var post in _posts)
            {
                //post.Category = _categories.FirstOrDefault(c => c.Id == post.CategoryId);
                post.Status = _status.FirstOrDefault(c => c.Id == post.StatusId);
                result.Add(post);
            }
            return result;
        }

        public BlogPost GetBlogPostById(int blogPostId)
        {
            var post = _posts.FirstOrDefault(p => p.Id == blogPostId); //if not found, it returns null
            var category = _categories.FirstOrDefault(c => c.Id == post.CategoryId);
            post.Category = category;
            return post;
        }

        public BlogPost GetBlogPostByIdForStatus(int blogPostId)
        {
            var post = _posts.FirstOrDefault(p => p.Id == blogPostId); //if not found, it returns null
            var status = _status.FirstOrDefault(c => c.Id == post.StatusId);
            post.Status = status;
            return post;
        }

        public void CreateBlogPost(BlogPost blogPost)
        {
            blogPost.Id = _posts.Count + 1;
            _posts.Add(blogPost);
        }

        public void UpdateBlogPost(BlogPost blogPost)
        {
            var existingBlogPost = _posts.FirstOrDefault(p => p.Id == blogPost.Id);
            if (existingBlogPost != null)
            {
                //No need to update CreatedDate (id of course won't be changed)
                existingBlogPost.ImageUrl = blogPost.ImageUrl;
                existingBlogPost.Title = blogPost.Title;
                existingBlogPost.Content = blogPost.Content;
                existingBlogPost.SpottedDate = blogPost.SpottedDate;
                existingBlogPost.UpdatedDate = blogPost.UpdatedDate;
                existingBlogPost.CategoryId = blogPost.CategoryId;
                existingBlogPost.StatusId = blogPost.StatusId;
                existingBlogPost.UserId = blogPost.UserId;
                existingBlogPost.Location = blogPost.Location;
            }
        }


        public IEnumerable<Category> GetAllCategories()
        {
            return _categories;
        }

        public IEnumerable<Status> GetAllStatus()
        {
            return _status;
        }

        public Category GetCategoryById(int categoryId)
        {
            return _categories.FirstOrDefault(c => c.Id == categoryId); //if not found, it returns null
        }

        public Status GetStatusById(int statusId)
        {
            return _status.FirstOrDefault(c => c.Id == statusId); //if not found, it returns null
        }
    }
}
