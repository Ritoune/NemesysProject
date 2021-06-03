using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bloggy.Models.Interfaces
{
    public interface IBloggyRepository
    {
        IEnumerable<BlogPost> GetAllBlogPosts();
        BlogPost GetBlogPostById(int blogPostId);

        void CreateBlogPost(BlogPost newBlogPost);

        void UpdateBlogPost(BlogPost updatedBlogPost);

        IEnumerable<Category> GetAllCategories();
        Category GetCategoryById(int categoryId);
    }

}
