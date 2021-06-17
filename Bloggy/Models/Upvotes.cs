using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bloggy.Models
{
    public class Upvotes
    {
        public string UserId { get; set; }
        public int Id { get; set; }
        public int BlogPostId { get; set; }
    }
}
