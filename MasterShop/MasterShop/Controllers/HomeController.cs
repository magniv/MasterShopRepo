using MasterShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MasterShop.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MasterShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly MasterShopContext _context;

        public HomeController(MasterShopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminPage()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            try
            {
                Account account = _context.Account.First(s => s.Email == userEmail);
                if (account.Type == userType.Admin)
                {
                    ViewData["Orders"] = _context.Order.ToList();
                    ViewData["Products"] = _context.Product.Include(p => p.Category).ToList();
                    ViewData["Accounts"] = _context.Account.ToList();
                    ViewData["Categories"] = _context.Category.ToList();
                    return View("../Admin/index");
                }
                else
                    return View();
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
