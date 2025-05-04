using Microsoft.AspNetCore.Mvc;
using NotinLite.Data;             
using NotinLite.Models;           
using NotinLite.ViewModels;       
using System.Threading.Tasks;     
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace NotinLite.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

                if (user != null)
                {
                    string enteredPasswordHash;
                    using (var sha256 = System.Security.Cryptography.SHA256.Create())
                    {
                        var passwordBytes = System.Text.Encoding.UTF8.GetBytes(model.Password);
                        var hashBytes = sha256.ComputeHash(passwordBytes);
                        enteredPasswordHash = Convert.ToBase64String(hashBytes);
                    }

                    if (user.PasswordHash == enteredPasswordHash)
                    {

                        var claims = new List<Claim>
                        {
                             new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), 
                             new Claim(ClaimTypes.Name, user.Username),                   
                             new Claim(ClaimTypes.Email, user.Email),                     
                         };

                        var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe, 

                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);



                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                bool usernameExists = await _context.Users.AnyAsync(u => u.Username == model.Username);
                if (usernameExists)
                {
                    ModelState.AddModelError(nameof(model.Username), "Username is already taken.");
                }

                bool emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email);
                if (emailExists)
                {
                    ModelState.AddModelError(nameof(model.Email), "Email address is already registered.");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

              
                string passwordHash;
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    var passwordBytes = System.Text.Encoding.UTF8.GetBytes(model.Password);
                    var hashBytes = sha256.ComputeHash(passwordBytes);
                    passwordHash = Convert.ToBase64String(hashBytes);
                }


                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = passwordHash 
                                                
                };

                _context.Users.Add(user);

                await _context.SaveChangesAsync();

               
                TempData["SuccessMessage"] = "Registration successful! Please log in."; 
                return RedirectToAction("Login", "Account"); 

            }

            return View(model);
        }
        [HttpPost] 
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}