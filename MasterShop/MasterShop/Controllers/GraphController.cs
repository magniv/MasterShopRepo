using MasterShop.Data;
using MasterShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MasterShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GraphController : Controller
    {
        private readonly MasterShopContext _context;

        public GraphController(MasterShopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetTopSoldProducts()
        {
            var query = (from pc in _context.ProductOrder
                         join p in _context.Product on pc.ProductId equals p.Id
                         group p by p.Name into g
                         orderby g.Count() descending
                         select new
                         {
                             ProductName = g.Key,
                             OrdersCount = g.Count()
                         });
            var res = await query.ToListAsync();

            return Json(res);
        }

        public async Task<IActionResult> GetOrdersByAddress()
        {
            var res = await _context.Order.GroupBy(o => o.Address)
                .Select(g => new { Address = g.Key, OrdersCount = g.Count() }).ToListAsync();
            return Json(res);
        }

        public async Task<IActionResult> GetAverageOrderPayementPerMonth()
        {
            var res = await _context.Order
                .GroupBy(o => o.OrderTime.Month)
                .Select(g => new {
                    Month = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(g.Key),
                    Average = g.Sum(o => o.SumToPay) / g.Count() 
                }).ToListAsync();
            return Json(res);
        }
    }
}
