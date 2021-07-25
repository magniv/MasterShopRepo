using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MasterShop.Data;
using MasterShop.Models;
using System.Security.Claims;

namespace MasterShop.Controllers
{
    public class CartsController : Controller
    {
        private readonly MasterShopContext _context;

        public CartsController(MasterShopContext context)
        {
            _context = context;
        }

        public double GetUserSumPayment()
        {
            double sum = 0;
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            Account account = _context.Account.First(s => s.Email == userEmail);

            var sumQuery = (from c in _context.Cart
                         join p in _context.Product on c.Product.Id equals p.Id
                         where c.Account.Id == account.Id
                         select new
                         {
                             price = c.Count * p.Price
                         });

            foreach (var x in sumQuery)
            {
                sum += x.price;
            }

            return sum;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail == null)
            {
                return RedirectToAction("LoginBeforeShopping", "Account");
            }

            var userCart = await _context.Cart.Include(c => c.Product).Where(c => c.Account.Email == userEmail).ToListAsync();
            if (userCart.Count > 0)
            {
                ViewData["CartSumPayment"] = GetUserSumPayment();
            }

            return View(userCart);
        }

        public async Task<IActionResult> Increase(int id)
        {
            var cart = _context.Cart.Where(s => s.Id == id).FirstOrDefault();
            if (cart != null)
            {
                cart.Count++;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Decrease(int id)
        {
            var cart = _context.Cart.Where(s => s.Id == id).FirstOrDefault();
            if (cart != null)
            {
                if (cart.Count != 1)
                {
                    cart.Count--;
                }
                else
                {
                    var cartToDelete = await _context.Cart.FindAsync(id);
                    _context.Cart.Remove(cartToDelete);
                }

                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Account)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Account, "Id", "ConfirmPassword");
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Count,AccountId")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Account, "Id", "ConfirmPassword", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Account, "Id", "ConfirmPassword", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name", cart.ProductId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Count,AccountId")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Account, "Id", "ConfirmPassword", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Account)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Cart.FindAsync(id);
            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Cart.Any(e => e.Id == id);
        }
    }
}
