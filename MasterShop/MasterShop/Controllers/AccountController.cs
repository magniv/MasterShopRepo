using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MasterShop.Data;
using MasterShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace MasterShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly MasterShopContext _context;

        public AccountController(MasterShopContext context)
        {
            _context = context;
        }

        public IActionResult Profile()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
            Account account = _context.Account.FirstOrDefault(c => c.Email == userEmail);
          
            if (account == null)
            {
                return NotFound();
            }
            return View(account);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(int id, [Bind("Id,Email,Password,ConfirmPassword,FullName,Gender,Type")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(account);
        }

        private bool AccountExists(int id)
        {
            return _context.Account.Any(e => e.Id == id);
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult LoginBeforeShopping()
        {
            ViewData["Message"] = "Please login before shopping.";
            return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Account.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (ModelState.IsValid)
            {

                if (user != null)
                {
                    await SignIn(user);
                    return RedirectToAction("Index", "Prodcuts");
                }
                else
                {
                    ModelState.AddModelError("Password", "The user name or password provided is incorrect.");
                }
            }             

            return View(); //login failed
        }

        private async Task SignIn(Account user)
        {
            //HttpContext.Session.SetString("Type", user.Type.ToString());
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FullName", user.FullName),
                new Claim("Email", user.Email),
                new Claim("Password", user.Password),
                new Claim(ClaimTypes.Role, user.Type.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        public IActionResult Register()
        {
            ViewData["Message"] = "";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string fname, string lname, string ConfirmPassword, string gender)
        {
            if (_context.Account.FirstOrDefault(x => x.Email == email) != null)
            {
                ModelState.AddModelError("email", "Email address already exists. Please enter a different email address.");
                ViewData["Message"] = "User with this Email Already Exist";
                return View();
            }
            Account account = new Account()
            {
                Email = email,
                Password = password,
                ConfirmPassword = ConfirmPassword,
                FullName = fname + " " + lname,
                Type = userType.Customer,
                Gender = gender
            };

            var account_check = await _context.Account
        .FirstOrDefaultAsync(m => m.Email == account.Email);
            if (account_check != null)
            {
                return NotFound();
            }

            _context.Add(account);
            await _context.SaveChangesAsync();

            await SignIn(account);
            return RedirectToAction("Index", "Home");
   
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
