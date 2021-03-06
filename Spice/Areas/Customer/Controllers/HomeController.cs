using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart cartObj)
        {
            cartObj.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObj.ApplicationUserID = claim.Value;

                ShoppingCart cartFromDb = await _db.ShoppingCarts.Where(m =>
                        m.ApplicationUserID == cartObj.ApplicationUserID && m.MenuItemId == cartObj.MenuItemId)
                    .FirstOrDefaultAsync();

                if (cartFromDb == null) // User has not added that menuItem in his cart
                {
                    _db.ShoppingCarts.Add(cartObj);
                }
                else// User already has that item in his cart. So we will increase the count.
                {
                    cartFromDb.Count = cartFromDb.Count + cartObj.Count;
                }
                await _db.SaveChangesAsync();

                // Now we want to get the total MenuItem count so that we can show it to the cart. We can do it by using session, It is super easy. We can use it throughout the application while user will be logged in.

                //First take the count of total menuItems
                var count = _db.ShoppingCarts.Where(m => m.ApplicationUserID == cartObj.ApplicationUserID)
                    .ToList().Count;

                HttpContext.Session.SetInt32("ssCartCount",count);

                return RedirectToAction("Index");
            }
            else // If modelState is not valid then return the same menuItem Detail view.
            {
                var menuItem = await _db.MenuItems
                    .Include(m => m.Category)
                    .Include(m => m.SubCategory)
                    .Where(m => m.Id == cartObj.MenuItemId).FirstOrDefaultAsync();

                var shoppingCart = new ShoppingCart()
                {
                    MenuItem = menuItem,
                    MenuItemId = menuItem.Id
                };

                return View(shoppingCart);
            }
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
