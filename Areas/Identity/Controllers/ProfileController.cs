using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Areas.Identity.Models;
using CMS.Data;
using Microsoft.AspNetCore.Authorization;
using CMS.Helpers;
using CMS.Areas.Identity.DTOs;
using AutoMapper;
using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using CMS.Models;
using System.Text.RegularExpressions;

namespace CMS.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    [Route("AdminCP/Profile/{action=index}")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public ProfileController(ApplicationDbContext context, IMapper mapper, UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: Identity/Profile
        [Breadcrumb("ViewData.Title")]
        public async Task<IActionResult> Index()
        {

            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == User.GetId());

            ProfileForEditDTO userForEdit = _mapper.Map<ProfileForEditDTO>(user);
            ViewBag.Profile = user;
            return View(userForEdit);
        }


        // POST: Identity/Profile/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Breadcrumb("ViewData.Title")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileForEditDTO profileForEdit)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == User.GetId());

            //foreach (var error in result.Errors)
            //{
            //    if (error.Code == "DuplicateUserName")
            //    {
            //        ModelState.AddModelError("UserName", error.Description);
            //    }
            //    else
            //    {
            //        ModelState.AddModelError(string.Empty, error.Description);
            //    }
            //}
            if (!String.IsNullOrEmpty(profileForEdit.PhoneNumber) && Regex.IsMatch(profileForEdit.PhoneNumber, "[^0-9]"))
            {
                ModelState.AddModelError("PhoneNumber", "Số điện thoại chưa đúng định dạng");
            }

            if (!String.IsNullOrEmpty(profileForEdit.OldPassword) || !String.IsNullOrEmpty(profileForEdit.NewPassword) || !String.IsNullOrEmpty(profileForEdit.RePassword))
            {
                if (String.IsNullOrEmpty(profileForEdit.OldPassword))
                {
                    ModelState.AddModelError("OldPassword", "Chưa nhập mật khẩu cũ");
                }
                if (String.IsNullOrEmpty(profileForEdit.NewPassword))
                {
                    ModelState.AddModelError("NewPassword", "Chưa nhập mật khẩu mới");
                }
                if (String.IsNullOrEmpty(profileForEdit.RePassword))
                {
                    ModelState.AddModelError("RePassword", "Chưa nhập lại mật khẩu mới");
                }

                if (!String.IsNullOrEmpty(profileForEdit.RePassword) && !String.IsNullOrEmpty(profileForEdit.NewPassword) && profileForEdit.NewPassword != profileForEdit.RePassword)
                {
                    ModelState.AddModelError("NewPassword", "Hai mật khẩu không khớp");
                    ModelState.AddModelError("RePassword", "Hai mật khẩu không khớp");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    User userToUpdate = _mapper.Map<ProfileForEditDTO, User>(profileForEdit, user);

                    var result = await _userManager.UpdateAsync(userToUpdate);
                    if (result.Succeeded)
                    {
                        if(!String.IsNullOrEmpty(profileForEdit.NewPassword))
                        {
                            await _userManager.RemovePasswordAsync(userToUpdate);
                            await _userManager.AddPasswordAsync(userToUpdate, profileForEdit.NewPassword);
                        }
                        TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                        {
                            Type = "success",
                            Message = "Cập nhật thông tin tài khoản thành công"
                        });
                        return RedirectPermanent("/AdminCP");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(profileForEdit.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewBag.Profile = user;
            return View(profileForEdit);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
