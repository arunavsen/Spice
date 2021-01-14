using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModel
{
    public class MenuItemViewModel
    {
        public MenuItem MenuItem { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<SubCategory> SubCategories { get; set; }
    }
}
