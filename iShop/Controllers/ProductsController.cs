using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using iShop.Data;
using iShop.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace iShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly iShopContext _context;

        public ProductsController(iShopContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(int id=0)
        {
            List<ProductViewModel> list = new List<ProductViewModel>();
            ViewBag.Categories = _context.Category.ToList();
            List<Product> products = null;
            IsCartCreated();
            if (id == 0)
            {
                products = await _context.Product.ToListAsync();
            }
            else
            {
                products = await _context.Product.Where( p => p.CategoryId == id).ToListAsync();
            }
            
            foreach(Product item in products)
            {
                Picture pic = _context.Picture.Where(p => p.ProductId == item.Id).FirstOrDefault();

                list.Add(new ProductViewModel() { 
                    Product = item,
                    PictureUrl = pic == null ? "" : pic.PictureUrl
                });
            }
            return View(list);
        }

        private void IsCartCreated()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userId))
            {
                return;
            }
            var cart = _context.Cart.FirstOrDefault( c => c.UserId == userId);
            if( cart != null)
            {
                HttpContext.Session.SetString("cart", userId);
            }
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }
            var product = await _context.Product.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            ProductViewModel pvm = new ProductViewModel() { 
                Product = product,
                PictureUrl = _context.Picture.Where( p => p.ProductId == id ).FirstOrDefault().PictureUrl
            };
            ViewBag.Images = _context.Picture.Where( p => p.ProductId == id ).ToList();
            return View(pvm);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CategoryId,Description,Count,PriceIn,PriceOut")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CategoryId,Description,Count,PriceIn,PriceOut")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'iShopContext.Product'  is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> AddToCart(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);     //get logged user Id
            HttpContext.Session.SetString("cart", userId);
            Product product = _context.Product.Find(id);
            if( product == null)
            {
                return NotFound();
            }
            Cart cart = new Cart()
            {
                ProductId = id,
                PriceOut = product.PriceOut,
                Count = 1,
                AddedAt = DateTime.Now,
                UserId = userId
            };
            _context.Cart.Add(cart);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string email = User.FindFirstValue(ClaimTypes.Email);
            List<Cart> carts = _context.Cart.Where(c => c.UserId == userId && c.Count > 0).ToList();

            int total = 0;

            List <CartViewModel> list = new List<CartViewModel>();
            foreach(Cart item in carts)
            {
                total += item.Count * item.PriceOut;
                CartViewModel cvm = new CartViewModel()
                {
                    Cart = item,
                    MaxCount = _context.Product.FirstOrDefault(p => p.Id == item.ProductId).Count,
                    TotalCost = total
                };
                list.Add(cvm);
            }

            ViewBag.Email = email;
            ViewBag.Total = total;
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeCount(int id, int count)
        {
            Cart cart = _context.Cart.Find(id);
            if( cart == null)
            {
                return RedirectToAction("Cart");
            }
            cart.Count = count;
            if( count == 0)
            {
                _context.Cart.Remove(cart);
                _context.SaveChanges();
            }
            else
            {
                _context.Cart.Update(cart);
                _context.SaveChanges();
            }

            return RedirectToAction("Cart");
        }



        [Authorize]
        public async Task<IActionResult> Buy()
        {
            //add to Sale all Cart objects with UserId == logged user Id
            //remove all Cart objects with UserId == logged user Id

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<Cart> carts = _context.Cart.Where(c => c.UserId == userId).ToList();
            foreach(Cart item in carts)
            {
                var sale = new Sale()
                {
                    ProductId = item.ProductId,
                    PriceOut = item.PriceOut,
                    SaleDate = DateTime.Now,
                    Count = item.Count,
                    UserId = userId
                };
                var product = _context.Product.Find(item.ProductId);
                product.Count -= item.Count;        //TODO validate Count
                _context.Product.Update(product);
                _context.Sale.Add(sale);
            }
            _context.SaveChanges();
            _context.Cart.RemoveRange(carts);
            _context.SaveChanges();
            HttpContext.Session.Remove("cart");

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
