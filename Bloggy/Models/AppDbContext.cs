using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bloggy.Models
{
    public class AppDbContext: IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            //This will pass any options passed in the constructor to the base class DbContext
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }

        public DbSet<Status> Status { get; set; }
        public DbSet<Investigation> Investigations { get; set; }
        public DbSet<Upvotes> Upvotes { get; set; }
    }

}
