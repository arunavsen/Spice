using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModel;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class SubcategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        [TempData]
        public string StatusMessage { get; set; }
        public SubcategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET - INDEX
        public async Task<IActionResult> Index()
        {
            var subCategory = await _db.SubCategories.Include(m => m.Category).ToListAsync();
            return View(subCategory);
        }

        // GET - CREATE
        public async Task<IActionResult> Create()
        {
            SubcategoryAndCategoryViewModel model = new SubcategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = new SubCategory(),
                SubCategoryList = await _db.SubCategories.OrderBy(m => m.Name)
                                                         .Select(m => m.Name)
                                                         .Distinct()
                                                         .ToListAsync()
            };

            return View(model);
        }

        // POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubcategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isSubCategoryExist = _db.SubCategories.Include(p => p.Category).Where(p =>
                      p.Name == model.SubCategory.Name && p.CategoryId == model.SubCategory.CategoryId);

                if (isSubCategoryExist.Count() > 0)
                {
                    // Error
                    StatusMessage = "Error: This sub-category is already exist in the " +
                                    isSubCategoryExist.First().Category.Name + " category";
                }
                else
                {
                    _db.SubCategories.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            SubcategoryAndCategoryViewModel modelVM = new SubcategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategories.OrderBy(m => m.Name)
                                                        .Select(m => m.Name)
                                                        .Distinct()
                                                        .ToListAsync(),
                StatusMessege = StatusMessage
            };

            return View(modelVM);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await _db.SubCategories.Where(m => m.CategoryId == id).ToListAsync();

            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        // GET - EDIT
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategories.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubcategoryAndCategoryViewModel model = new SubcategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategories.OrderBy(m => m.Name)
                    .Select(m => m.Name)
                    .Distinct()
                    .ToListAsync()
            };

            return View(model);
        }

        // POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubcategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isSubCategoryExist = _db.SubCategories.Include(p => p.Category).Where(p =>
                    p.Name == model.SubCategory.Name && p.CategoryId == model.SubCategory.CategoryId);

                if (isSubCategoryExist.Count() > 0)
                {
                    // Error
                    StatusMessage = "Error: This sub-category is already exist in the " +
                                    isSubCategoryExist.First().Category.Name + " category";
                }
                else
                {
                    var subCategoryFromDb = await _db.SubCategories.FindAsync(model.SubCategory.Id);
                    subCategoryFromDb.Name = model.SubCategory.Name;

                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            SubcategoryAndCategoryViewModel modelVM = new SubcategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategories.OrderBy(m => m.Name)
                    .Select(m => m.Name)
                    .Distinct()
                    .ToListAsync(),
                StatusMessege = StatusMessage
            };

            return View(modelVM);
        }

        // GET - DETAILS
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategories.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubcategoryAndCategoryViewModel model = new SubcategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategories.OrderBy(m => m.Name)
                    .Select(m => m.Name)
                    .Distinct()
                    .ToListAsync()
            };

            return View(model);
        }

        // GET - DELETE
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategory = await _db.SubCategories.Include(m=>m.Category).FirstOrDefaultAsync(m=>m.Id==id);
            if (subcategory == null)
            {
                return NotFound();
            }

            return View(subcategory);
        }

        // POST - DELETE
        [HttpPost]
        public async Task<IActionResult> Delete(SubCategory subCategory)
        {
            if (subCategory != null)
            {
                _db.SubCategories.Remove(subCategory);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }
    }
}