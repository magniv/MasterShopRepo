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
    public class OrdersController : Controller
    {
        private readonly MasterShopContext _context;

        public OrdersController(MasterShopContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var masterShopContext = _context.Order.Include(o => o.Account);
            return View(await masterShopContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail == null)
            {
                return RedirectToAction("LoginBeforeShopping", "Account");
            }

            var userCart = await _context.Cart.Include(c => c.Product).Where(c => c.Account.Email == userEmail).ToListAsync();

            double totalPrice = 0;
            foreach (var cartItem in userCart)
            {
                totalPrice += cartItem.Product.Price * cartItem.Count;
            }

            ViewData["TotalPrice"] = totalPrice;
            ViewData["UserCart"] = userCart;
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccountId,OrderTime,Address,PhoneNumber,SumToPay")] Order order)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail == null)
            {
                return RedirectToAction("LoginBeforeShopping", "Account");
            }

            Account account = _context.Account.First(s => s.Email == userEmail);
            order.Account = account;
            var userCart = _context.Cart.Include(c => c.Product).Where(c => c.AccountId == account.Id);

            order.OrderTime = DateTime.Now;
            order.SumToPay = 0;
            order.ProductOrders = new List<ProductOrder>();

            foreach (var item in userCart)
            {
                order.ProductOrders.Add(new ProductOrder() { ProductId = item.Product.Id, OrderId = order.Id, Count = item.Count });
                order.SumToPay += item.Product.Price * item.Count;
            }

            _context.Cart.RemoveRange(_context.Cart.Where(c => c.AccountId == account.Id));
            _context.Add(order);

            await _context.SaveChangesAsync();
            string orderNumber = order.Id.ToString();
            ViewData["OrderNumber"] = orderNumber;
            return View("Ordered");
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Account, "Id", "ConfirmPassword", order.AccountId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountId,OrderTime,Address,PhoneNumber,SumToPay")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Account, "Id", "ConfirmPassword", order.AccountId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}
