using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bloggy.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //Collection navigation property
        public List<BlogPost> BlogPosts { get; set; }
    }

}




