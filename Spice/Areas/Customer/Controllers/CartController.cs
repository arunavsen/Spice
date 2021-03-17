using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModel;
using Spice.Utility;

namespace Spice.Areas.Customer.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public OrderDetailsCart DetailsOrderCart { get; set; }

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            DetailsOrderCart = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader()
            };

            DetailsOrderCart.OrderHeader.OrderTotal = 0;

            // Getting the login user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // Getting the user's shopping cart list
            var shoppingCart = _db.ShoppingCarts.Where(m => m.ApplicationUserID == claims.Value);

            if (shoppingCart!=null)
            {
                // Now we have added the User's cart list in our DetailsOrderCart object
                DetailsOrderCart.ShoppingCarts = shoppingCart.ToList();
            }


            foreach (var cart in DetailsOrderCart.ShoppingCarts)
            {
                cart.MenuItem = await _db.MenuItems.FirstOrDefaultAsync(m => m.Id == cart.MenuItemId);
                DetailsOrderCart.OrderHeader.OrderTotal =
                    DetailsOrderCart.OrderHeader.OrderTotal + (cart.MenuItem.Price * cart.Count);
                cart.MenuItem.Description = SD.ConvertToRawHtml(cart.MenuItem.Description);
                if (cart.MenuItem.Description.Length > 100)
                {
                    cart.MenuItem.Description = cart.MenuItem.Description.Substring(0, 99) + "...";
                }
            }

            DetailsOrderCart.OrderHeader.OrderTotalOriginal = DetailsOrderCart.OrderHeader.OrderTotal;

            return View(DetailsOrderCart);
        }
    }
}