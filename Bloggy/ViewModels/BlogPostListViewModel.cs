using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bloggy.Models;

namespace Bloggy.ViewModels
{
    public class BlogPostListViewModel
    {
        public int TotalEntries { get; set; }
        public IEnumerable<BlogPostViewModel> BlogPosts { get; set; }
    }

}
