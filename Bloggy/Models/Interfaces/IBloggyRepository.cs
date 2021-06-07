using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bloggy.Models.Interfaces
{
    public interface IBloggyRepository
    {
        IEnumerable<BlogPost> GetAllBlogPosts();
        IEnumerable<BlogPost> GetAllBlogPostsForStatus();
        BlogPost GetBlogPostById(int blogPostId);
        BlogPost GetBlogPostByIdForStatus(int blogPostId);

        void CreateBlogPost(BlogPost newBlogPost);

        void UpdateBlogPost(BlogPost updatedBlogPost);

        IEnumerable<Category> GetAllCategories();
        IEnumerable<Status> GetAllStatus();
        Category GetCategoryById(int categoryId);
        Status GetStatusById(int statusId);
    }

}
