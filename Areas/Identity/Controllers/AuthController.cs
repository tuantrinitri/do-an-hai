using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Areas.Identity.DTOs;
using CMS.Areas.Identity.Models;
using CMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace CMS.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("Auth/{action}")]
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly SignInManager<User> _signInManager;

        public AuthController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "AdminCP" });
            }
            return View(new AuthLoginDTO { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(AuthLoginDTO authLogin)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "AdminCP" });
            }

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(authLogin.UserName, authLogin.Password, authLogin.Remember, false);
                if (result.Succeeded)
                {
                    TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                    {
                        Type = "success",
                        Message = "Đăng nhập hệ thống thành công"
                    });
                    if (authLogin.ReturnUrl != "" && authLogin.ReturnUrl != null)
                    {
                        return Redirect(authLogin.ReturnUrl);
                    }
                    return RedirectToAction("Index", "Dashboard", new { area = "AdminCP" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không chính xác");
                }
            }
            return View(authLogin);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth", new { area = "Identity" });
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
            {
                Type = "error",
                Message = "Bạn không có quyền truy cập"
            });
            return RedirectToAction("Index", "Dashboard", new { area = "AdminCP" });
        }
    }
}
