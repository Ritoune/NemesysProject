using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Bloggy.Models
{
    public class DbInitializer
    {
        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                roleManager.CreateAsync(new IdentityRole("User")).Wait();
                roleManager.CreateAsync(new IdentityRole("Administrator")).Wait();
            }
        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new ApplicationUser()
                {
                    Email = "testuser@testmail.com",
                    NormalizedEmail = "TESTUSER@TESTMAIL.COM",
                    UserName = "testuser@testmail.com",
                    NormalizedUserName = "TESTUSER@TESTMAIL.COM",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D") //to track important profile updates (e.g. password change)
                };

                //Add to store
                IdentityResult result = userManager.CreateAsync(user, "Bl0ggyRules!").Result;
                if (result.Succeeded)
                {
                    //Add to role
                    userManager.AddToRoleAsync(user, "User").Wait();
                }


                var admin = new ApplicationUser()
                {
                    Email = "testadmin@testmail.com",
                    NormalizedEmail = "TESTADMIN@TESTMAIL.COM",
                    UserName = "testadmin@testmail.com",
                    NormalizedUserName = "TESTADMIN@TESTMAIL.COM",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D") //to track important profile updates (e.g. password change)
                };

                //Add to store
                result = userManager.CreateAsync(admin, "Bl0ggyRules!").Result;
                if (result.Succeeded)
                {
                    //Add to role
                    userManager.AddToRoleAsync(admin, "Administrator").Wait();
                }
            }
        }

        public static void SeedData(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            if (!context.Categories.Any())
            {
                context.AddRange
                (
                    new Category()
                    {
                        Name = "Unsafe act"
                    },
                    new Category ()
                    {
                        Name = "Condition"
                    },
                    new Category()
                    {
                        Name = "Equipment"
                    },
                    new Category()
                    {
                        Name = "Structure"
                    },
                    new Category()
                    {
                        Name = "Other"
                    }
                );
                context.SaveChanges();
            }

            if (!context.Status.Any())
            {
                context.AddRange
                (
                    new Status()
                    {
                        Name = "Opened"
                    },
                    new Status()
                    {
                        Name = "Closed"
                    },
                    new Status()
                    {
                        Name = "Being investigating"
                    },
                    new Status()
                    {
                        Name = "No action required"
                    }
                   
                );
                context.SaveChanges();
            }



            if (!context.BlogPosts.Any())
            {
                //Grabbing first one
                var user = userManager.GetUsersInRoleAsync("User").Result.FirstOrDefault();

                context.AddRange
                (
                    new BlogPost()
                    {
                        Title = "Dangerous act",
                        Content = "A man is risking his life ",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        SpottedDate = DateTime.UtcNow,
                        ImageUrl = "/images/UnsafeAct3.jpg",
                        CategoryId = 1,
                        StatusId = 1,
                        UserId = user.Id,
                        Location = "In the parking",
                        HasInvestigation = false
                    },
                    new BlogPost()
                    {
                        Title = "The ladder will break ",
                        Content = "Horror scene",
                        CreatedDate = DateTime.UtcNow.AddDays(-1),
                        UpdatedDate = DateTime.UtcNow.AddDays(-1),
                        SpottedDate = DateTime.UtcNow.AddDays(-1),
                        ImageUrl = "/images/UnsafeAct2.jpg",
                        CategoryId = 1,
                        StatusId = 1,
                        UserId = user.Id,
                        Location = "At the entrance",
                        HasInvestigation = false
                    },
                    new BlogPost()
                    {
                        Title = "When will he stop it ?",
                        Content = "A man in a dangerous position",
                        CreatedDate = DateTime.UtcNow.AddDays(-2),
                        UpdatedDate = DateTime.UtcNow.AddDays(-2),
                        SpottedDate = DateTime.UtcNow.AddDays(-2),
                        ImageUrl = "/images/UnsafeAct1.jpg",
                        CategoryId = 1,
                        StatusId = 1,
                        UserId = user.Id,
                        Location = "In the library",
                        HasInvestigation = false
                    },
                    new BlogPost()
                    {
                        Title = "Failed fire extinguisher",
                        Content = "The fire protection system does not work ",
                        CreatedDate = DateTime.UtcNow.AddDays(-2),
                        UpdatedDate = DateTime.UtcNow.AddDays(-2),
                        SpottedDate = DateTime.UtcNow.AddDays(-2),
                        ImageUrl = "/images/Condition1.jpg",
                        CategoryId = 2,
                        StatusId = 1,
                        UserId = user.Id,
                        Location = "In the stadium",
                        HasInvestigation = false
                    },
                    new BlogPost()
                    {
                        Title = "Computer broken",
                        Content = "This computer does not work",
                        CreatedDate = DateTime.UtcNow.AddDays(-2),
                        UpdatedDate = DateTime.UtcNow.AddDays(-2),
                        SpottedDate = DateTime.UtcNow.AddDays(-2),
                        ImageUrl = "/images/Equipment1.jpg",
                        CategoryId = 3,
                        StatusId = 1,
                        UserId = user.Id,
                        Location = "Classroom 404",
                        HasInvestigation = false
                    },
                    new BlogPost()
                    {
                        Title = "Dangerous building",
                        Content = "Please, repair this building",
                        CreatedDate = DateTime.UtcNow.AddDays(-2),
                        UpdatedDate = DateTime.UtcNow.AddDays(-2),
                        SpottedDate = DateTime.UtcNow.AddDays(-2),
                        ImageUrl = "/images/Structure 2.jpg",
                        CategoryId = 4,
                        StatusId = 1,
                        UserId = user.Id,
                        Location = "In the stadium",
                        HasInvestigation = false
                    }


                ); 
                context.SaveChanges();
            }
        }
    }

}
