using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web_KhachSanResort.Models;
using Web_KhachSanResort.ViewModels;

namespace Web_KhachSanResort.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Users> userManager;
        private readonly SignInManager<Users> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;
        //Đăng nhập

        public AccountController(UserManager<Users> userManager, SignInManager<Users> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (result.IsNotAllowed)
                    {
                        // Log thêm thông tin hoặc xử lý tùy chỉnh tại đây
                        ModelState.AddModelError("", "Tài khoản không được phép đăng nhập.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email or Password không chính xác");
                    }
                    return View(model);
                }
            }
            return View(model);
        }
        

        //Đăng kí
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Users users = new Users
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    UserName = model.Email,
                    NormalizedUserName = model.Email.ToUpper(),
                    NormalizedEmail = model.Email.ToUpper(),
                    // SecurityStamp = Guid.NewGuid().ToString(),
                    // ConcurrencyStamp = Guid.NewGuid().ToString(),S
                };
                var result = await userManager.CreateAsync(users, model.Password);
                if (result.Succeeded)
                {
                    var roleResult = await roleManager.RoleExistsAsync("User");
                    if (!roleResult)
                    {
                        var role = new IdentityRole("User");
                        await roleManager.CreateAsync(role);
                    }
                    await userManager.AddToRoleAsync(users, "User");
                    await signInManager.SignInAsync(users, isPersistent: false);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }
            return View(model);
        }
        //Xác minh Email
        [HttpGet]
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email không tồn tại");
            }
            else{
                return RedirectToAction("ChangePassword", "Account", new {username = user.UserName});
            }
            return View(model);
        }
        //Thay đổi mật khẩu
        [HttpGet]
        public IActionResult ChangePassword(string username)
        {
            if(string.IsNullOrEmpty(username))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            return View(new ChangePasswordViewModel{Email = username});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
           
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var removePasswordResult = await userManager.RemovePasswordAsync(user);
                    if (removePasswordResult.Succeeded)
                    {
                        var addPasswordResult = await userManager.AddPasswordAsync(user, model.MatKhauMoi);
                        if (addPasswordResult.Succeeded)
                        {
                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {
                            foreach (var error in addPasswordResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            return View(model);
                        }
                    }
                    else
                    {
                        foreach (var error in removePasswordResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy tài khoản với email đã cung cấp.");
                    return View(model);
                }
            }
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }

}
