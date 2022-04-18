using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Areas.Identity.Models;
using CMS.Data;
using SmartBreadcrumbs.Attributes;
using CMS.Areas.Identity.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using CMS.Helpers;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using CMS.Models;


namespace CMS.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("AdminCP/Users/{action=index}/{id?}")]
    [Authorize(Roles = "admin")] // Can use multi roles by example: "admin,btv,.etc...", view more role at \Data\SeedData.cs
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UsersController(ApplicationDbContext context, IMapper mapper, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Identity/Users
        [Breadcrumb("Tài khoản")]
        public async Task<IActionResult> Index(string q, int? unitId, int? jobTitleId, int? page)
        {
            var users = _context.Users
                .Include(u => u.JobTitle)
                .Include(u => u.Unit)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .OrderByDescending(u => u.Id);

            if (!String.IsNullOrEmpty(q))
            {
                ViewData["filter_q"] = q;
                users = (IOrderedQueryable<User>)users.Where(u => u.UserName.Contains(q) || u.Fullname.Contains(q));
            }
            if(unitId != null)
            {
                ViewData["filter_unitId"] = unitId;
                users = (IOrderedQueryable<User>)users.Where(u => u.UnitId == unitId);
            }
            if(jobTitleId != null)
            {
                ViewData["filter_jobTitleId"] = jobTitleId;
                users = (IOrderedQueryable<User>)users.Where(u => u.JobTitleId == jobTitleId);
            }

            int pageSize = 10;

            ViewData["JobTitleId"] = new SelectList(_context.JobTitles, "Id", "Title", new { Id = jobTitleId });
            ViewData["UnitId"] = new SelectList(_context.Units, "Id", "Title", new { Id = unitId });
            ViewBag.totalitem = users.Count();
            return View(PaginatedList<User>.Create(users.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: Identity/Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.JobTitle)
                .Include(u => u.Unit)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Identity/Users/Create
        [Breadcrumb("Tạo mới")]
        public IActionResult Create()
        {
            ViewData["JobTitleId"] = new SelectList(_context.JobTitles, "Id", "Title");
            ViewData["UnitId"] = new SelectList(_context.Units, "Id", "Title");
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Title");
            return View(new UserForCreateDTO { Activated = true });
        }

        // POST: Identity/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Breadcrumb("Tạo mới")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserForCreateDTO userForCreate)
        {
            if (ModelState.IsValid)
            {
                User user = _mapper.Map<User>(userForCreate);

                var role = _roleManager.Roles.FirstOrDefault(r => r.Id == userForCreate.RoleId);
                user.Role = role.Name;

                var result = await _userManager.CreateAsync(user, "123456");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role.Name);
                    TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                    {
                        Type = "success",
                        Message = "Tạo dữ liệu thành công"
                    });
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    if (error.Code == "DuplicateUserName")
                    {
                        ModelState.AddModelError("UserName", error.Description);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            ViewData["JobTitleId"] = new SelectList(_context.JobTitles, "Id", "Title", userForCreate.JobTitleId);
            ViewData["UnitId"] = new SelectList(_context.Units, "Id", "Title", userForCreate.UnitId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Title", userForCreate.RoleId);
            return View(userForCreate);
        }

        // GET: Identity/Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            UserForEditDTO userForEdit = _mapper.Map<UserForEditDTO>(user);
            userForEdit.RoleId = user.UserRoles.First().Role.Id;
            ViewData["JobTitleId"] = new SelectList(_context.JobTitles, "Id", "Title", user.JobTitleId);
            ViewData["UnitId"] = new SelectList(_context.Units, "Id", "Title", user.UnitId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Title", userForEdit.RoleId);
            return View(userForEdit);
        }

        // POST: Identity/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserForEditDTO userForEdit)
        {
            if (id != userForEdit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    User user = _userManager.Users
                        .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                        .First(u => u.Id == id);

                    User userToUpdate = _mapper.Map<UserForEditDTO, User>(userForEdit, user);

                    var role = _roleManager.Roles.FirstOrDefault(r => r.Id == userForEdit.RoleId);
                    userToUpdate.Role = role.Name;

                    var result = await _userManager.UpdateAsync(userToUpdate);
                    if (result.Succeeded)
                    {
                        await _userManager.RemoveFromRoleAsync(userToUpdate, user.UserRoles.First().Role.Name);
                        await _userManager.AddToRoleAsync(userToUpdate, role.Name);
                        TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                        {
                            Type = "success",
                            Message = "Cập nhật dữ liệu thành công"
                        });
                        return RedirectToAction(nameof(Index));
                    }
                    foreach (var error in result.Errors)
                    {
                        if (error.Code == "DuplicateUserName")
                        {
                            ModelState.AddModelError("UserName", error.Description);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(userForEdit.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["JobTitleId"] = new SelectList(_context.JobTitles, "Id", "Title", userForEdit.JobTitleId);
            ViewData["UnitId"] = new SelectList(_context.Units, "Id", "Title", userForEdit.UnitId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Title", userForEdit.RoleId);
            return View(userForEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Không tìm thấy bản ghi cần xóa, vui lòng thử lại"
                });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Không tìm thấy bản ghi cần xóa, vui lòng thử lại"
                });
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Json(new
            {
                Status = true,
                Message = "Đã xóa dữ liệu thành công"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ResetPasswordAsync(int? id)
        {
            var user = _userManager.Users.First(u => u.Id == id);
            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, "123456");

            return Json(new {
                Status = true,
                Message = "Khôi phục mật khẩu mặc định thành công"
            });
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
