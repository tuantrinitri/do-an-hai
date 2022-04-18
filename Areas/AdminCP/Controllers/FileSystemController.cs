using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using CMS.Helpers;
using elFinder.NetCore;
using elFinder.NetCore.Drivers.FileSystem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Areas.AdminCP.Controllers
{
    [Area("AdminCP")]
    [Route("AdminCp/el-finder/file-system")]
    [Authorize]
    public class FileSystemController : Controller
    {
        [Route("connector")]
        public async Task<IActionResult> Connector()
        {
            var connector = await GetConnector();

            return await connector.ProcessAsync(Request);
        }

        [Route("thumb/{hash}")]
        public async Task<IActionResult> Thumbs(string hash)
        {
            var connector = await GetConnector();
            return await connector.GetThumbnailAsync(HttpContext.Request, HttpContext.Response, hash);
        }

        private async Task<Connector> GetConnector()
        {
            var driver = new FileSystemDriver();
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var username = User.Identity.Name;
            string absoluteUrl = UriHelper.BuildAbsolute(Request.Scheme, Request.Host);
            var uri = new Uri(absoluteUrl);
            var webrootpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var mypath = Path.Combine(webrootpath, "public", "upload");
            var dynamicPath = Path.Combine(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());               
            var userPath = String.Empty;
            if (!User.IsInRole("admin"))
            {
                mypath = Path.Combine(mypath, username);
                userPath = username + "/";              
                if (!Directory.Exists(Path.Combine(mypath, dynamicPath))) {
                    await Task.Run(() => Directory.CreateDirectory(Path.Combine(mypath, dynamicPath)));
                    // Directory.CreateDirectory(Path.Combine(mypath, dynamicPath));
                }
            }
            if (!Directory.Exists(mypath))
            {
                await Task.Run(() => Directory.CreateDirectory(mypath));
                // Directory.CreateDirectory(Path.Combine(mypath, dynamicPath));
            }
            var root = new RootVolume(
                Startup.MapPath(mypath),
                $"{uri.Scheme}://{uri.Authority}/public/upload/{userPath}",
                $"{uri.Scheme}://{uri.Authority}/AdminCp/el-finder/file-system/thumb/")
            {
                //IsReadOnly = !User.IsInRole("Administrators")
                IsReadOnly = false, // Can be readonly according to user's membership permission
                IsLocked = false, // If locked, files and directories cannot be deleted, renamed or moved
                Alias = "Thư mục gốc", // Beautiful name given to the root/home folder
                MaxUploadSizeInKb = 1048576, // Limit imposed to user uploaded file <= 200 MB
                //LockedFolders = new List<string>(new string[] { "Folder1" })
                ThumbnailSize = 80
            };
            driver.AddRoot(root);
            return new Connector(driver)
            {
                // This allows support for the "onlyMimes" option on the client.
                MimeDetect = MimeDetectOption.Internal
            };
        }
    }
}