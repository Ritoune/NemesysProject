using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bloggy.Models
{
    public class Investigation
    {
        public int Id { get; set; }
        public string DescriptionOfInvestigation { get; set; }
        public DateTime DateOfAction { get; set; }
        public string UserId { get; set; }
        //Reference navigation property
        public ApplicationUser User { get; set; }
        public int BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }

    }

}
