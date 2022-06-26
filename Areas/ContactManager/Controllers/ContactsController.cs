using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Areas.ContactManager.Models;
using CMS.Data;
using SmartBreadcrumbs.Attributes;
using Newtonsoft.Json;
using CMS.Models;
using CMS.Helpers;

namespace CMS.Areas.ContactManager.Controllers
{
    [Area("ContactManager")]
    [Route("AdminCP/Contacts/{action=index}/{id?}")]
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GoogleReCAPTCHAService _reCAPTCHAService;

        public ContactsController(ApplicationDbContext context, GoogleReCAPTCHAService reCAPTCHAService)
        {
            _context = context;
            _reCAPTCHAService = reCAPTCHAService;
        }

        // GET: ContactManager/Contacts
        [Breadcrumb("Liên hệ")]
        public async Task<IActionResult> Index(string q, int? page)
        {
            var contacts = _context.Contacts.OrderByDescending(u => u.Id);

            ViewBag.totalitem = contacts.Count();

            return View(PaginatedList<Contact>.Create(contacts.AsNoTracking(), page ?? 1, 10));
        }

        // GET: ContactManager/Contacts/Details/5
        [Breadcrumb("Chi tiết liên hệ")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            contact.Status = true;
            await _context.SaveChangesAsync();

            return View(contact);
        }

        // GET: ContactManager/Contacts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ContactManager/Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Fullname,Email,PhoneNumber,Content,Status,CreatedAt,LastModifiedAt")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                contact.CreatedAt = DateTime.Now;
                var isFromPublic = Request.Form["public"];
                if (isFromPublic == "true")
                {
                    var token = Request.Form["g-recaptcha-response"];
                    var controller = Request.Form["controller"];
                    var area = Request.Form["area"];
                    var ggRecapt = _reCAPTCHAService.VerifyCaptchaAsync(token, 2);
                    if (ggRecapt.Result.success)
                    {
                        _context.Add(contact);
                        await _context.SaveChangesAsync();
                        TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                        {
                            Type = "success",
                            Message = "Tạo câu hỏi thành công"
                        });
                        return RedirectToAction("Index", controller, new { @area = area });
                    }
                    else
                    {
                        TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                        {
                            Type = "error",
                            Message = "Dữ liệu ko hợp lệ"
                        });
                        return RedirectToAction("Index", controller, new { @area = area });
                    }
                }
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }

        // GET: ContactManager/Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
        }

        // POST: ContactManager/Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Status")] Contact contact)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldContact = _context.Contacts.FirstOrDefault(c => c.Id == id);
                    oldContact.Status = contact.Status;
                    oldContact.LastModifiedAt = DateTime.Now;
                    _context.Update(oldContact);
                    await _context.SaveChangesAsync();
                    TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                    {
                        Type = "success",
                        Message = "Đã cập nhật dữ liệu thành công"
                    });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(contact);
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

            var contact = await _context.Contacts.FindAsync(id);
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return Json(new
            {
                Status = true,
                Message = "Đã xóa dữ liệu thành công"
            });
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }
    }
}
