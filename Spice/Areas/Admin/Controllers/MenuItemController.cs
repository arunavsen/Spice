﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModel;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
            MenuItemVM=new MenuItemViewModel()
            {
                Categories = _db.Categories,
                MenuItem = new MenuItem()
            };
        }

        // GET - INDEX
        public async Task<IActionResult> Index()
        {
            var menuItems = await _db.MenuItems
                .Include(m=>m.Category)
                .Include(m=>m.SubCategory)
                .ToListAsync();

            return View(menuItems);
        }

        // GET - CREATE
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }

            _db.MenuItems.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();

            // Work on the image saving section
            string webRootPath = _hostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuITemFromDb = await _db.MenuItems.FindAsync(MenuItemVM.MenuItem.Id);

            if (files.Count>0)
            {
                // File has been uploaded
                var uploads = Path.Combine(webRootPath,"images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var filesStream = new FileStream(Path.Combine(uploads,MenuItemVM.MenuItem.Id+extension),FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }

                menuITemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                // No file was uploaded. So use default.
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads,webRootPath+@"\images\"+MenuItemVM.MenuItem.Id+".png");
                menuITemFromDb.Image = @"\images\" + menuITemFromDb.Id + ".png";
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}