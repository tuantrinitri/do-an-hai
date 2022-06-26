using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Areas.SettingManager.Models;
using CMS.Data;
using CMS.Areas.ContactManager.Models;
using Newtonsoft.Json;
using CMS.Helpers;
using CMS.Models;

namespace CMS.Areas.ContactManager.Controllers
{
    [Area("ContactManager")]
    [Route("lien-he/{action=index}")]
    public class ContactsPublicController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GoogleReCAPTCHAService _reCAPTCHAService;

        public ContactsPublicController(ApplicationDbContext context, GoogleReCAPTCHAService reCAPTCHAService)
        {
            _context = context;
            _reCAPTCHAService = reCAPTCHAService;
        }

        // GET: ContactManager/ContactsPublic
        public IActionResult Index()
        {
            return View();
        }

        // POST: ContactManager/Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Contact contact)
        {
            if (ModelState.IsValid)
            {
                contact.CreatedAt = DateTime.Now;
               
                    _context.Add(contact);
                    await _context.SaveChangesAsync();
                    TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                    {
                        Type = "success",
                        Message = "Gửi liên hệ thành công"
                    });
                    return View();
            }
            TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
            {
                Type = "error",
                Message = "Gửi liên hệ thất bại"
            });
            return View();
        }
        private bool GlobalSettingExists(int id)
        {
            return _context.GlobalSettings.Any(e => e.Id == id);
        }
    }
}
