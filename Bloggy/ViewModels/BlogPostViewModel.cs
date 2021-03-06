using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bloggy.ViewModels
{
    public class BlogPostViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime SpottedDate { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public int ReadCount { get; set; }
        public int StatusId { get; set; }
        public int CategoryId { get; set; }
        public string UserId { get; set; }
        public bool HasInvestigation { get; set; }
        public bool HasUpvote { get; set; }
        public CategoryViewModel Category { get; set; }
        public StatusViewModel Status { get; set; }

        public string Location { get; set; }
        public AuthorViewModel Author { get; set; }
    }

}
