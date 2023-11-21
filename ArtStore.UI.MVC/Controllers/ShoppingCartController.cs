using Microsoft.AspNetCore.Mvc;
using ArtStore.DATA.EF.Models;//context
using ArtStore.UI.MVC.Models;//CartItemViewModel
using Microsoft.AspNetCore.Identity;//UserManager
using Newtonsoft.Json;//To manage Session variables
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace ArtShop.UI.MVC.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ArtStoreContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ShoppingCartController(ArtStoreContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            //retrieve the cart contents from session
            string? sessionCart = HttpContext.Session.GetString("cart");

            //create the shell for the C# version of the cart
            Dictionary<int, CartItemViewModel>? shoppingCart;

            //check to see if the cart exists
            if (string.IsNullOrEmpty(sessionCart))
            {
                shoppingCart = new();
                ViewBag.Message = "There are no items in your cart.";
            }
            else
            {
                ViewBag.Message = null;
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            }
            //var shoppingCart = GetCart();//Dry example.
            //ViewBag.Message = shoppingCart.Any() ? null : "There are no items in your cart";
            return View(shoppingCart);
        }

        //Add to cart
        public IActionResult AddToCart(int id)
        {            

            var sessionCart = HttpContext.Session.GetString("cart");

            //Empty shell for a LOCAL shopping cart variable
            Dictionary<int, CartItemViewModel> shoppingCart;

            if (string.IsNullOrEmpty(sessionCart))
            {
                shoppingCart = new();
            }
            else
            {
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            }
            //var shoppingCart = GetCart();
            //add newly selected items to the cart
            Product? product = _context.Products.Find(id);

            //Initialize a cart item so we can add to the cart.
            CartItemViewModel item = new(1, product);

            if (shoppingCart.ContainsKey(product.ProductId))
            {
                //update the quantity
                shoppingCart[product.ProductId].Qty++;
            }
            else
            {
                shoppingCart.Add(product.ProductId, item);
            }
            //SetCart(shoppingCart);
            string jsonCart = JsonConvert.SerializeObject(shoppingCart);
            HttpContext.Session.SetString("cart", jsonCart);

            return RedirectToAction("Index");

        }

        public IActionResult RemoveFromCart(int id)
        {
            //Dry : var shoppingCart = GetCart();
            //retrieve the cart from session
            var jsonCart = HttpContext.Session.GetString("cart");
            var shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(jsonCart);

            shoppingCart.Remove(id);

            //Check if there are any other items in the cart. if not, remove the cart from session.
            if (shoppingCart.Count == 0) //if (!shoppingCart.Any())
            {
                HttpContext.Session.Remove("cart");
            }
            else
            {
                jsonCart = JsonConvert.SerializeObject(shoppingCart);
                HttpContext.Session.SetString("cart", jsonCart);
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult UpdateCart(int productId, int qty)
        {
            var jsonCart = HttpContext.Session.GetString("cart");
            var shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(jsonCart);

            //update the qty for our specific dictionary key.
            //if qty is 0, remove the item from the cart.
            if (qty <= 0)
            {
                RemoveFromCart(productId);
            }
            else
            {
                shoppingCart[productId].Qty = qty;
                jsonCart = JsonConvert.SerializeObject(shoppingCart);
                HttpContext.Session.SetString("cart", jsonCart);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        //[HttpPost]
        public async Task<IActionResult> SubmitOrder()
        {
            //retrieve the current user's ID
            var userId = (await _userManager.GetUserAsync(HttpContext.User))?.Id;

            var userName = (await _userManager.GetUserAsync(HttpContext.User))?.UserName;

            var userEmail = (await _userManager.GetUserAsync(HttpContext.User))?.Email;

            //retrieve the UserDetails for that user
            var ud = _context.UserDetails.Find(userId);
            if (ud == null)
            {
                var newUd = new UserDetail()
                {
                    UserId = userId,
                    FirstName = userName,                    
                    EmailAddress = userEmail,
                };
                _context.Add(newUd);
                ud = newUd;
            }

            //Create the order object and assign values (either from user details or from your checkout form submission.)
            Order o = new()
            {
                OrderDate = DateTime.Now,
                UserId = userId,
                CustomerEmail = ud?.EmailAddress ?? "Not Given",
                CustomerName = ud?.FullName ?? "Not Given"
               
            };

            _context.Add(o);

            //Retrieve the session cart
            var jsonCart = HttpContext.Session.GetString("cart");
            var shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(jsonCart);

            foreach (var item in shoppingCart.Values)
            {
                //create an OrderProduct object for each item in the cart
                OrderProduct op = new OrderProduct()
                {
                    OrderId = o.OrderId,
                    ProductId = item.Product.ProductId,
                    ProductPrice = item.Product.ProductPrice
                 
                };

                o.OrderProducts.Add(op);
            }
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("cart");
            return RedirectToAction("Index", "Orders");
        }

        //Checkout
        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel cvm)
        {
            if (!ModelState.IsValid)
            {
                return View(cvm);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!_context.UserDetails.Any(ud => ud.UserId == userId))
            {
                var newUd = new UserDetail()
                {
                    UserId = userId,
                    FirstName = cvm.FirstName,
                    LastName = cvm.LastName,
                    EmailAddress = cvm.Email
                };
                _context.Add(newUd);
            }

            Order o = new()
            {
                OrderDate = DateTime.Now,
                UserId = userId,
                CustomerEmail = cvm.Email,
                CustomerName = cvm.FirstName + " " + cvm.LastName                
            };

            _context.Orders.Add(o);
            //Retrieve the session cart
            var jsonCart = HttpContext.Session.GetString("cart");
            var shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(jsonCart);

            foreach (var item in shoppingCart.Values)
            {
                //create an OrderProduct object for each item in the cart
                OrderProduct op = new OrderProduct()
                {
                    OrderId = o.OrderId,
                    ProductId = item.Product.ProductId,
                    ProductPrice = item.Product.ProductPrice
                  
                };

                o.OrderProducts.Add(op);
            }
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("cart");
            return RedirectToAction("Index", "Orders");
        }

    }
}

