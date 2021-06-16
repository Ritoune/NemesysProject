using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bloggy.Models.Interfaces
{
    public interface IBloggyRepository
    {
        IEnumerable<BlogPost> GetAllBlogPosts();
        public IEnumerable<Investigation> GetAllInvestigations();
        public Investigation GetInvestigationById(int investigationId);
        public void CreateInvestigation(Investigation investigation);
        public void UpdateInvestigation(Investigation investigation);
        public void DeleteInvestigationById(int id);
        public void DeleteReport(BlogPost post);
        public Investigation GetInvestigationByIdReport(int ReportId);

        IEnumerable<BlogPost> GetAllBlogPostsForStatus();
        BlogPost GetBlogPostById(int blogPostId);
        BlogPost GetBlogPostByIdForStatus(int blogPostId);

        void CreateBlogPost(BlogPost newBlogPost);

        void UpdateBlogPost(BlogPost updatedBlogPost);

        IEnumerable<Category> GetAllCategories();
        IEnumerable<Status> GetAllStatus();
        Category GetCategoryById(int categoryId);
        Status GetStatusById(int statusId);

        public Upvotes GetUpVoteByReportIdAndUserId(int reportId, string userId);
        public void CreateUpVote(Upvotes upvote);
    }

}
