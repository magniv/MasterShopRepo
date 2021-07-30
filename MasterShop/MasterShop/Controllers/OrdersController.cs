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
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using System.Net;

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var masterShopContext = _context.Order.Include(o => o.Account);
            return View(await masterShopContext.ToListAsync());
        }

        // GET: Orders/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Account)
                .Include(o => o.ProductOrders).ThenInclude(p => p.Product)
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

            if (userCart.Count == 0)
            {
                return View("../Carts/Index", userCart);
            }

            double totalPrice = 0;
            foreach (var cartItem in userCart)
            {
                totalPrice += cartItem.Product.Price * cartItem.Count;
            }

            ViewData["TotalPrice"] = totalPrice;
            ViewData["UserCart"] = userCart;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccountId,OrderTime,Address,PhoneNumber,SumToPay")] Order order)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail == null)
            {
                return RedirectToAction("LoginBeforeShopping", "Account");
            }

            if (!ValidatePhoneNumber(order.PhoneNumber))
            {
                ModelState.AddModelError(nameof(Order.PhoneNumber), "The phone number format is not ok");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            if (!ModelState.IsValid)
            {
                return View(order);
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

        private bool ValidatePhoneNumber(string phoneNumber)
        {
            var regex = new Regex(@"^0(5[^7]|[2-4]|[8-9]|7[0-9])[0-9]{7}");
            return regex.IsMatch(phoneNumber);
        }

        // GET: Orders/Edit/5
       [Authorize(Roles = "Admin")]
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
            ViewData["AccountId"] = new SelectList(_context.Account, nameof(Account.Id), nameof(Account.FullName), order.AccountId);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountId,OrderTime,Address,PhoneNumber,SumToPay")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (!ValidatePhoneNumber(order.PhoneNumber))
            {
                ModelState.AddModelError(nameof(Order.PhoneNumber), "The phone number format is not ok");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
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
                return RedirectToAction("AdminPage", "Home");
            }
            ViewData["AccountId"] = new SelectList(_context.Account, nameof(Account.Id), nameof(Account.FullName), order.AccountId);
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("AdminPage", "Home");
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}
