using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ArtStore.DATA.EF.Models;
using Microsoft.AspNetCore.Authorization;

namespace ArtStore.UI.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
  
            private readonly ArtStoreContext _context;

            public CategoriesController(ArtStoreContext context)
            {
                _context = context;
            }

            // GET: Categories
            public async Task<IActionResult> Index()
            {
                return _context.Categories != null ?
                            View(await _context.Categories.ToListAsync()) :
                            Problem("Entity set 'GadgetStoreContext.Categories'  is null.");
            }        
           

            //AjaxDelete()
            [AcceptVerbs("POST")]
            public JsonResult AjaxDelete(int id)
            {
                var cat = _context.Categories.Find(id);
                _context.Categories.Remove(cat);
                _context.SaveChanges();

                string message = $"Deleted the category {cat.CategoryName} from the database!";
                return Json(new { id, message });
            }

            //CategoryDetails()
            public PartialViewResult CategoryDetails(int id)
            {
                var cat = _context.Categories.Find(id);
                return PartialView(cat);
            
            }
            //AjaxCreate
            [HttpPost]
            [ValidateAntiForgeryToken]
            public JsonResult AjaxCreate(Category category)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return Json(category);
             
            }
           

            [HttpGet]
            public PartialViewResult CategoryEdit(int id)
            {
                var cat = _context.Categories.Find(id);
                return PartialView(cat);                
            }

            
            [HttpPost]
            [ValidateAntiForgeryToken]
            public JsonResult AjaxEdit(Category category)
            {
                _context.Update(category);
                _context.SaveChanges();
                return Json(category);
            }

            //#region Default
            //private readonly ArtStoreContext _context;

            //public CategoriesController(ArtStoreContext context)
            //{
            //    _context = context;
            //}

            //// GET: Categories
            //public async Task<IActionResult> Index()
            //{
            //      return _context.Categories != null ? 
            //                  View(await _context.Categories.ToListAsync()) :
            //                  Problem("Entity set 'ArtStoreContext.Categories'  is null.");
            //}

            //// GET: Categories/Details/5
            //public async Task<IActionResult> Details(int? id)
            //{
            //    if (id == null || _context.Categories == null)
            //    {
            //        return NotFound();
            //    }

            //    var category = await _context.Categories
            //        .FirstOrDefaultAsync(m => m.CategoryId == id);
            //    if (category == null)
            //    {
            //        return NotFound();
            //    }

            //    return View(category);
            //}

            //// GET: Categories/Create
            //public IActionResult Create()
            //{
            //    return View();
            //}

            //// POST: Categories/Create
            //// To protect from overposting attacks, enable the specific properties you want to bind to.
            //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            //[HttpPost]
            //[ValidateAntiForgeryToken]
            //public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,CategoryDescription")] Category category)
            //{
            //    if (ModelState.IsValid)
            //    {
            //        _context.Add(category);
            //        await _context.SaveChangesAsync();
            //        return RedirectToAction(nameof(Index));
            //    }
            //    return View(category);
            //}

            //// GET: Categories/Edit/5
            //public async Task<IActionResult> Edit(int? id)
            //{
            //    if (id == null || _context.Categories == null)
            //    {
            //        return NotFound();
            //    }

            //    var category = await _context.Categories.FindAsync(id);
            //    if (category == null)
            //    {
            //        return NotFound();
            //    }
            //    return View(category);
            //}

            //// POST: Categories/Edit/5
            //// To protect from overposting attacks, enable the specific properties you want to bind to.
            //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            //[HttpPost]
            //[ValidateAntiForgeryToken]
            //public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,CategoryDescription")] Category category)
            //{
            //    if (id != category.CategoryId)
            //    {
            //        return NotFound();
            //    }

            //    if (ModelState.IsValid)
            //    {
            //        try
            //        {
            //            _context.Update(category);
            //            await _context.SaveChangesAsync();
            //        }
            //        catch (DbUpdateConcurrencyException)
            //        {
            //            if (!CategoryExists(category.CategoryId))
            //            {
            //                return NotFound();
            //            }
            //            else
            //            {
            //                throw;
            //            }
            //        }
            //        return RedirectToAction(nameof(Index));
            //    }
            //    return View(category);
            //}

            //// GET: Categories/Delete/5
            //public async Task<IActionResult> Delete(int? id)
            //{
            //    if (id == null || _context.Categories == null)
            //    {
            //        return NotFound();
            //    }

            //    var category = await _context.Categories
            //        .FirstOrDefaultAsync(m => m.CategoryId == id);
            //    if (category == null)
            //    {
            //        return NotFound();
            //    }

            //    return View(category);
            //}

            //// POST: Categories/Delete/5
            //[HttpPost, ActionName("Delete")]
            //[ValidateAntiForgeryToken]
            //public async Task<IActionResult> DeleteConfirmed(int id)
            //{
            //    if (_context.Categories == null)
            //    {
            //        return Problem("Entity set 'ArtStoreContext.Categories'  is null.");
            //    }
            //    var category = await _context.Categories.FindAsync(id);
            //    if (category != null)
            //    {
            //        _context.Categories.Remove(category);
            //    }

            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}

            //private bool CategoryExists(int id)
            //{
            //  return (_context.Categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
            //}
            //#endregion
        }
}
