using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Bloggy.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public int ReadCount { get; set; }

        //Foreign Key - navigation property name + key property name
        public int CategoryId { get; set; }
        //Reference navigation property
        public Category Category { get; set; }

        public int StatusId { get; set; }

        //Foreign Key - navigation property name + key property name
        public string UserId { get; set; }
        //Reference navigation property
        public ApplicationUser User { get; set; }

        


    }

}
