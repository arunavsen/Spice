using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModel;

namespace Spice.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel indexVM = new IndexViewModel()
            {
                MenuItems = await _db.MenuItems.Include(c=>c.Category).Include(c=>c.SubCategory).ToListAsync(),
                Coupons = await _db.Coupons.Where(c=>c.IsActive==true).ToListAsync(),
                Categories = await _db.Categories.ToListAsync()
            };
            
            return View(indexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuItem = await _db.MenuItems
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .Where(m => m.Id == id).FirstOrDefaultAsync();

            var shoppingCart= new ShoppingCart()
            {
                MenuItem = menuItem,
                MenuItemId = menuItem.Id
            };

            return View(shoppingCart);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
