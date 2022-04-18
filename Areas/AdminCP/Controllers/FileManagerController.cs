using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using System;
using System.IO;
using CMS.Helpers;
using System.Runtime.InteropServices;

namespace CMS.Areas.AdminCP.Controllers
{
    [Route("AdminCp/file-manager")]
    [Area("AdminCP")]
    [Authorize]
    [Breadcrumb("Quản lý file")]
    public class FileManagerController : Controller
    {
        public IActionResult Index()
        {
            if (!User.IsInRole("admin"))
            {
                string currentFolder = String.Empty;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    currentFolder = string.Format("\\{0}\\{1}", DateTime.Now.Year, DateTime.Now.Month);
                }
                else
                {
                    currentFolder = string.Format("/{0}/{1}", DateTime.Now.Year, DateTime.Now.Month);
                }
                ViewBag.CurrentFolder = "v1_" + currentFolder.EncodePath();
            }

            return View();
        }
        [Route("ckeditor-browse")]
        public IActionResult CkEditorBrowse()
        {
            return View();
        }
        [Route("standalone-browse")]
        public IActionResult StandAloneBrowse()
        {
            if (!User.IsInRole("admin"))
            {
                string currentFolder = String.Empty;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    currentFolder = string.Format("\\{0}\\{1}", DateTime.Now.Year, DateTime.Now.Month);
                }
                else
                {
                    currentFolder = string.Format("/{0}/{1}", DateTime.Now.Year, DateTime.Now.Month);
                }
                ViewBag.CurrentFolder = "v1_" + currentFolder.EncodePath();
            }
            return View();
        }
    }
}