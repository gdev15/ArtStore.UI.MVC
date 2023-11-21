using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArtStore.DATA.EF.Models;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;//added for file upload
using ArtStore.UI.MVC.Utilities;//added for image resize utility
using X.PagedList;


namespace ArtStore.UI.MVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ArtStoreContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ArtStoreContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
           
            var products = _context.Products
                .Where(p => !p.IsDiscontinued)// SELECT * FROM Products p WHERE p.IsDiscontinued <> 0
                .Include(p => p.Category)// JOIN Categories ON Category
                .Include(p => p.FileType)// JOIN Suppliers ON Supplier
                .Include(p => p.OrderProducts);//JOIN OrderProducts 
            

            return View(await products.ToListAsync());
        }

        #region Filter/Paging       


        public async Task<IActionResult> TiledProducts(string searchTerm, int categoryId = 0, int page = 1)
        {
            var products = await _context.Products
                .Where(p => !p.IsDiscontinued)// SELECT * FROM Products p WHERE p.IsDiscontinued <> 0
                .Include(p => p.Category)// JOIN Categories ON Category
                .Include(p => p.FileType)// JOIN Suppliers ON Supplier
                .Include(p => p.OrderProducts).ToListAsync();//JOIN OrderProducts 

            #region Optional Search Filter
            if (searchTerm != null)
            {
                searchTerm = searchTerm.ToLower();
                products = products.Where(p =>
                            p.ProductName.ToLower().Contains(searchTerm) ||
                            p.FileType.FileTypeName.ToLower().Contains(searchTerm) ||
                            p.Category.CategoryName.ToLower().Contains(searchTerm) ||
                            p.ProductDescription.ToLower().Contains(searchTerm)).ToList();
                ViewBag.NbrResults = products.Count;
                ViewBag.SearchTerm = searchTerm;
            }
            #endregion

            #region Optional Category Filter
            //Create a ViewBag/Data to send a list of categories to the view
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryId);
            if (categoryId != 0)
            {
                products = products.Where(p => p.CategoryId == categoryId).ToList();
                ViewBag.NbrResults = products.Count;
                ViewBag.SearchTerm = searchTerm;
            }

            //paged list:
            int pageSize = 8;

            #endregion

            //return View(products);
            return View(products.ToPagedList(page, pageSize));
        }
        #endregion


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.FileType)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["FileTypeId"] = new SelectList(_context.FileTypes, "FileTypeId", "FileTypeName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductPrice,ProductDescription,UnitsInStock,UnitsOnOrder,IsDiscontinued,CategoryId,SupplierId,ProductImage,Image")] Product product)
        {
            if (ModelState.IsValid)
            {
                #region File Upload - CREATE
                if (product.Image == null)
                {
                    product.ProductImage = "noimage.png";
                }
                else
                {
                    string ext = Path.GetExtension(product.Image.FileName);
                    List<string> validExt = new() { ".jpeg", ".jpg", ".gif", ".png" };
                    if (validExt.Contains(ext.ToLower()) && product.Image.Length < 4_194_303)
                    {
                        product.ProductImage = Guid.NewGuid() + ext;
                        string webrootPath = _webHostEnvironment.WebRootPath;
                        string fullImagePath = webrootPath + "/images/";
                        using var memoryStream = new MemoryStream();
                        await product.Image.CopyToAsync(memoryStream);
                        using var img = Image.FromStream(memoryStream);

                        int maxImage = 500;
                        int maxThumbSize = 100;
                        ImageUtilities.ResizeImage(fullImagePath, product.ProductImage, img, maxImage, maxThumbSize);
                    }
                }
                #endregion
                //end:
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["FileTypeId"] = new SelectList(_context.FileTypes, "FileTypeId", "FileType", product.FileTypeId);
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["FileTypeId"] = new SelectList(_context.FileTypes, "FileTypeId", "FileTypeName", product.FileTypeId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductPrice,ProductDescription,UnitsInStock,UnitsOnOrder,IsDiscontinued,CategoryId,SupplierId,ProductImage,Image")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                #region File Upload - Edit
                string? oldImageName = product.ProductImage;
                if (product.Image != null)
                {
                    string ext = Path.GetExtension(product.Image.FileName);
                    List<string> validExts = new() { ".jpeg", ".jpg", ".png", ".gif" };
                    if (validExts.Contains(ext.ToLower()) && product.Image.Length < 4_194_303)
                    {
                        product.ProductImage = Guid.NewGuid() + ext;
                        string fullPath = _webHostEnvironment.WebRootPath + "/images/";
                        if (oldImageName != null && !oldImageName.ToLower().StartsWith("noimage"))
                        {
                            ImageUtilities.Delete(fullPath, oldImageName);
                        }
                        using var memoryStream = new MemoryStream();
                        await product.Image.CopyToAsync(memoryStream);
                        using var img = Image.FromStream(memoryStream);
                        int maxImgSize = 500;
                        int maxThumbSize = 100;
                        ImageUtilities.ResizeImage(fullPath, product.ProductImage, img, maxImgSize, maxThumbSize);
                    }
                }
                #endregion

                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.FileTypes, "FileTypeId", "FileTypeName", product.FileTypeId);
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.FileType)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'GadgetStoreContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                string? oldImageName = product.ProductImage;
                string fullPath = _webHostEnvironment.WebRootPath + "/images/";
                if (oldImageName != null && !oldImageName.ToLower().Contains("noimage"))
                {
                    ImageUtilities.Delete(fullPath, oldImageName);
                }
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
