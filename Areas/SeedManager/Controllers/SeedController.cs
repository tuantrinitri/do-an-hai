using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CMS.Areas.PostManager.Models;
using CMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Areas.SeedManager.Controllers
{
    [Area("SeedManager")]
    [Route("AdminCP/SeedManager/{action}")]
    [Authorize(Roles = "admin")]
    public class SeedController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SeedController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        //public IActionResult PostHandle()
        //{
        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        public async Task<IActionResult> PostHandle(string btnSubmit)
        {
            if(btnSubmit == "clear")
            {
                _context.Posts.RemoveRange(_context.Posts);
                await _context.SaveChangesAsync();
            }
            if (Request.Form.Files.Count > 0)
            {
                var jsonFile = Request.Form.Files[0];
                using(var reader = new StreamReader(jsonFile.OpenReadStream()))
                {
                    var fileContent = reader.ReadToEnd();
                }    
                    
            }

            return RedirectToAction("Index");
        }
    }
}
