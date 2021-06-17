using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bloggy.ViewModels
{
    public class HallOfFameViewModel
    {
        public int NumberOfReports { get; set; }
        public string UserId { get; set; }
        public AuthorViewModel Author { get; set; }
    }
}
