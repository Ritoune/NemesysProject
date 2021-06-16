using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bloggy.ViewModels
{
    public class InvestigationViewModel
    {
        public int Id { get; set; }
        public string DescriptionOfInvestigation { get; set; }
        public DateTime DateOfAction { get; set; }
        public string UserId { get; set; }
        public int BlogPostId { get; set; }
        public BlogPostViewModel BlogPost { get; set; }
        public AuthorViewModel Author { get; set; }
    }

}
