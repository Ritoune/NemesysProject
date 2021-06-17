using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bloggy.ViewModels
{
    public class HallOfFameListViewModel
    {
        public IEnumerable<HallOfFameViewModel> HallOfFames { get; set; }
    }
}
