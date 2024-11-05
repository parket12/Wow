using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Yakshin.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Yakshin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private AppDbContext _appDbContext;

        public HomeController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _appDbContext.Product.ToListAsync());
        }

        public async Task<IActionResult> Catalog()
        {
            var product = await _appDbContext.Product.ToListAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SingIn(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Username == model.Username && u.Passwords == model.Passwords);
                if (user != null)
                {
                    var role = await _appDbContext.Roles.FindAsync(user.ID_Roles);
                    HttpContext.Session.SetString("AuthUser ", model.Username);
                    await Authenticate(model.Username, role?.Title);

                    if (role != null && role.Title == "Admin")
                    {
                        return RedirectToAction("AdminPage", "Home");
                    }
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Некорректные логин или пароль");
            }
            return View(model);
        }






        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _appDbContext.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(Users user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _appDbContext.Users.FindAsync(user.ID);
                if (existingUser != null)
                {
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Username = user.Username;

                    if (!string.IsNullOrEmpty(user.Passwords))
                    {
                        existingUser.Passwords = user.Passwords;
                    }

                    _appDbContext.Update(existingUser);
                    await _appDbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(AdminPage));
                }
                return NotFound();
            }
            return View(user);
        }




        public async Task<IActionResult> AdminPage()
        {
            var users = await _appDbContext.Users.ToListAsync();
            return View(users);
        }


        public IActionResult SingIn()
        {
            if (HttpContext.Session.Keys.Contains("AuthUser "))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        private async Task Authenticate(string userName, string role)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
        new Claim(ClaimsIdentity.DefaultRoleClaimType, role) // Добавляем роль
    };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("AuthUser ");
            return RedirectToAction("SingIn");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SingUp(Users person)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Username == person.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Пользователь с таким именем уже существует.");
                    return View(person);
                }

                person.ID_Roles = 1;

                await _appDbContext.Users.AddAsync(person);
                await _appDbContext.SaveChangesAsync();
                return RedirectToAction("SingIn");
            }
            return View(person);
        }


        public IActionResult SingUp()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
