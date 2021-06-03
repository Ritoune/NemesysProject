using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Bloggy.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string AuthorAlias { get; set; }
    }

}
