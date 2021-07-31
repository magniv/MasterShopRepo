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
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Text.RegularExpressions;

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
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
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

            if (!ValidatePassword(account.Password))
            {
                ModelState.AddModelError(nameof(Account.Password), "The minumum requierments are: 8 characters long containing 1 uppercase letter, 1 lowercase letter, a number and a special character");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            if (account.Password != account.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(Account.ConfirmPassword), "Passwords does not match");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                    await SignIn(account);
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
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(nameof(Account.Password), "The user name or password provided is incorrect.");
                }
            }             

            return View(); //login failed
        }

        private async Task SignIn(Account user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FullName", user.FullName),
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,Email,Password,ConfirmPassword,FullName,Gender")] Account account)
        {
            if (_context.Account.FirstOrDefault(x => x.Email == account.Email) != null)
            {
                ModelState.AddModelError(nameof(Account.Email), "Email address already exists. Please enter a different email address.");
                return View();
            }

            if (!ValidatePassword(account.Password))
            {
                ModelState.AddModelError(nameof(Account.Password), "The minumum requierments are: 8 characters long containing 1 uppercase letter, 1 lowercase letter, a number and a special character");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            if (account.Password != account.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(Account.ConfirmPassword), "Passwords does not match");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            if (!ModelState.IsValid)
            {
                return View(account);
            }

            account.Type = userType.Customer;

            _context.Add(account);
            await _context.SaveChangesAsync();

            await SignIn(account);
            return RedirectToAction("Index", "Home");
   
        }

        private bool ValidatePassword(string password)
        {
            Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return regex.IsMatch(password);

        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account.FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Email,Password,ConfirmPassword,FullName,Gender,Type")] Account account)
        {
            if (_context.Account.FirstOrDefault(x => x.Email == account.Email) != null)
            {
                ModelState.AddModelError("email", "Email address already exists. Please enter a different email address.");
                ViewData["Message"] = "User with this Email Already Exist";
                return View();
            }

            if (!ValidatePassword(account.Password))
            {
                ModelState.AddModelError(nameof(Account.Password), "The minumum requierments are: 8 characters long containing 1 uppercase letter, 1 lowercase letter, a number and a special character");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            if (account.Password != account.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(Account.ConfirmPassword), "Passwords does not match");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }


            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction("AdminPage", "Home");
            }
            return View(account);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Username,Password,ConfirmPassword,FullName,Gender,Type")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            if(!ValidatePassword(account.Password))
            {
                ModelState.AddModelError(nameof(Account.Password), "The minumum requierments are: 8 characters long containing 1 uppercase letter, 1 lowercase letter, a number and a special character");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            if (account.Password != account.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(Account.ConfirmPassword), "Passwords does not match");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
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
                return RedirectToAction("AdminPage", "Home");
            }

            return View(account);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account.FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Account.FindAsync(id);
            _context.Account.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction("AdminPage", "Home");
        }
    }
}
