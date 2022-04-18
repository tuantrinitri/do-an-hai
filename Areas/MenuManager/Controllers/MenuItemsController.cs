using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Areas.MenuManager.Models;
using CMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using CMS.Areas.MenuManager.DTOs;
using SmartBreadcrumbs.Attributes;
using AutoMapper;
using Newtonsoft.Json;
using CMS.Models;
using CMS.Areas.MenuManager.Helpers;
using CMS.Helpers;

namespace CMS.Areas.MenuManager.Controllers
{
    [Area("MenuManager")]
    [Route("AdminCP/Menus/{menuId?}/{action=index}/{id?}")]
    [Authorize(Roles = "admin")]
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MenuItemsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: MenuManager/MenuItems
        [Breadcrumb("ViewData.MenuTitle", FromController = typeof(MenusController), FromAction = "Index")]
        public async Task<IActionResult> Index(int? menuId, int? page)
        {
            //SeedMenuItems();
            if (menuId == null)
            {
                return RedirectToAction("Index", "Menus");
            }

            var menu = _context.Menus.FirstOrDefault(m => m.Id == menuId);
            if (menu == null)
            {
                return RedirectToAction("Index", "Menus");
            }
            var menus = _context.MenuItems.Where(l => l.MenuId == menuId).OrderBy(l => l.Order).ToList();
            int i = 1;
            foreach (var iMenu in menus)
            {
                iMenu.Order = i;
                _context.SaveChanges();
                i++;
            }
            ViewData["MenuTitle"] = menu.Title;
            ViewBag.totalitem = menus.Count();
            IQueryable<MenuItemTree> menuitems = TreeMenu(menuId).AsQueryable();
            return View(PaginatedList<MenuItemTree>.Create(menuitems, page ?? 1, 10));
        }

        // GET: MenuManager/MenuItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItems
                .Include(m => m.Menu)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        // GET: MenuManager/MenuItems/Create
        [Breadcrumb("Tạo mới item", FromAction = "Index")]
        public IActionResult Create(int? menuId)
        {
            var menu = _context.Menus.FirstOrDefault(m => m.Id == menuId);
            if (menu == null)
            {
                return RedirectToAction("Index", "Menus");
            }

            ViewData["MenuTitle"] = menu.Title;
            ViewData["ParentId"] = new SelectList(TreeMenu(menuId), "Id", "TitleWithPrefix");
            return View(new MenuItemForCreateDTO { Published = true });
        }

        // POST: MenuManager/MenuItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Breadcrumb("Tạo mới item", FromAction = "Index")]
        public async Task<IActionResult> Create(int? menuId, MenuItemForCreateDTO menuItemForCreate)
        {
            var menu = _context.Menus.FirstOrDefault(m => m.Id == menuId);
            if (menu == null)
            {
                return RedirectToAction("Index", "Menus");
            }

            if (ModelState.IsValid)
            {
                MenuItem menuItem = _mapper.Map<MenuItem>(menuItemForCreate);
                var maxItem = _context.MenuItems.Where(i => i.MenuId == menuId && i.ParentId == menuItem.ParentId).OrderByDescending(i => i.Order).LastOrDefault();
                menuItem.Order = maxItem != null ? maxItem.Order + 1 : 1;
                menuItem.CreatedAt = DateTime.Now;
                menuItem.LastModifiedAt = DateTime.Now;
                menuItem.MenuId = menu.Id;

                _context.Add(menuItem);
                await _context.SaveChangesAsync();
                TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                {
                    Type = "success",
                    Message = "Tạo dữ liệu thành công"
                });
                return RedirectToAction(nameof(Index));
            }

            ViewData["MenuTitle"] = menu.Title;
            ViewData["ParentId"] = new SelectList(TreeMenu(menuId), "Id", "TitleWithPrefix", menuItemForCreate.ParentId);
            return View(menuItemForCreate);
        }

        // GET: MenuManager/MenuItems/Edit/5
        [Breadcrumb("ViewData.Title", FromAction = "Index")]
        public async Task<IActionResult> Edit(int? menuId, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = _context.Menus.FirstOrDefault(m => m.Id == menuId);
            if (menu == null)
            {
                return RedirectToAction("Index", "Menus");
            }

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            var menuItemForEdit = _mapper.Map<MenuItemForEditDTO>(menuItem);
            ViewData["ItemTitle"] = menuItem.Title;
            ViewData["MenuTitle"] = menu.Title;
            ViewData["ParentId"] = new SelectList(TreeMenu(menuId).Where(i => i.Id != id && i.ParentId != id), "Id", "TitleWithPrefix", menuItem.ParentId);
            return View(menuItemForEdit);
        }

        // POST: MenuManager/MenuItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Breadcrumb("ViewData.Title", FromAction = "Index")]
        public async Task<IActionResult> Edit(int? menuId, int id, MenuItemForEditDTO menuItemForEdit)
        {
            if (id != menuItemForEdit.Id)
            {
                return NotFound();
            }

            var menu = _context.Menus.FirstOrDefault(m => m.Id == menuId);
            if (menu == null)
            {
                return RedirectToAction("Index", "Menus");
            }

            MenuItem menuItem = _context.MenuItems.FirstOrDefault(i => i.Id == id);

            if (ModelState.IsValid)
            {
                try
                {
                    MenuItem menuItemToUpdate = _mapper.Map<MenuItemForEditDTO, MenuItem>(menuItemForEdit, menuItem);
                    menuItemToUpdate.LastModifiedAt = DateTime.Now;

                    _context.Update(menuItemToUpdate);
                    await _context.SaveChangesAsync();
                    TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                    {
                        Type = "success",
                        Message = "Cập nhật dữ liệu thành công"
                    });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuItemExists(menuItemForEdit.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["ItemTitle"] = menuItem.Title;
            ViewData["MenuTitle"] = menu.Title;
            ViewData["ParentId"] = new SelectList(TreeMenu(menuId), "Id", "TitleWithPrefix", menuItemForEdit.ParentId);
            return View(menuItemForEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteAsync(int? id)
        {
            if (id == null)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Không tìm thấy bản ghi cần xóa, vui lòng thử lại"
                });
            }
            List<int> items = new List<int>();
            items.Add((int)id);
            await DeleteMenuItemAsync(items);

            return Json(new
            {
                Status = true,
                Reload = true,
                Message = "Đã xóa dữ liệu thành công"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ChangeOrder(int? menuId, int? id, int? order)
        {
            if (id == null)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Không tìm thấy bản ghi cần update, vui lòng thử lại"
                });
            }
            if (order <= 0)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Vui lòng nhập giá trị lớn hơn 0"
                });
            }
            if (order == null)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Không tìm thấy thứ tự cần cập nhật, vui lòng thử lại"
                });
            }

            var newOrder = _context.MenuItems.Where(i => i.MenuId == menuId).FirstOrDefault(i => i.Id == id);
            var oldOrder = _context.MenuItems.Where(i => i.MenuId == menuId).FirstOrDefault(i => i.Order == order);
            if (newOrder == null)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Không tìm thấy bản ghi cần update, vui lòng thử lại"
                });
            }
            if (oldOrder != null)
            {
                oldOrder.Order = newOrder.Order;
                await _context.SaveChangesAsync();
            }

            newOrder.Order = (int)order;
            await _context.SaveChangesAsync();

            return Json(new
            {
                Status = true,
                Reload = true,
                Message = "Cập nhật vị trí thành công"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> LoadDataSource(int? type)
        {
            if (type == null)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Nguồn cung cấp liên kết không đúng, vui lòng thử lại"
                });
            }

            var data = new List<SourceItem>();
            var _sourceMenuItemProvider = new SourceMenuItemProvider(_context);
            switch (type)
            {
                
                case 2:
                    data = await _sourceMenuItemProvider.PostCategoriesAsync();
                    break;
                case 3:
                    data = await _sourceMenuItemProvider.StaticPageAsync();
                    break;
                case 4:
                    data = _sourceMenuItemProvider.OtherItem();
                    break;

                default:
                    break;
            };

            return Json(new
            {
                Status = true,
                Message = "Đã lấy dữ liệu nguồn liên kết thành công",
                Data = data
            });
        }

        private bool MenuItemExists(int id)
        {
            return _context.MenuItems.Any(e => e.Id == id);
        }

        public List<MenuItemTree> TreeMenu(int? menuId, int parentId = 0, string prefix = "")
        {
            var groups = _context.MenuItems
                .Where(i => i.MenuId == menuId && i.ParentId == parentId)
                .OrderBy(x => x.Order)
                .ToLookup(x => x.ParentId, x => new MenuItemTree
                {
                    Id = x.Id,
                    Title = x.Title,
                    Link = x.Link,
                    ParentId = x.ParentId,
                    MenuId = x.MenuId,
                    Published = x.Published,
                    OpenType = x.OpenType,
                    Order = x.Order
                });

            List<MenuItemTree> result = new List<MenuItemTree>();
            // Assign children
            foreach (var item in groups.SelectMany(x => x))
            {
                item.Children = groups[item.Id].ToList();
                item.Prefix = prefix;
                item.TitleWithPrefix = item.Prefix + item.Title;
                result.Add(item);
                var beforePrefix = prefix;
                prefix += "|--------- ";
                result.AddRange(TreeMenu(menuId, item.Id, prefix));
                prefix = beforePrefix;
            }
            return result;
        }

        public async Task DeleteMenuItemAsync(List<int> items)
        {
            if (_context.MenuItems.Any(i => items.Contains(i.ParentId)))
            {
                var childItems = _context.MenuItems.Where(i => items.Contains(i.ParentId)).Select(i => i.Id).ToList();
                DeleteMenuItemAsync(childItems).Wait();
            }
            var menuItems = _context.MenuItems.Where(i => items.Contains(i.Id)).ToList();
            _context.MenuItems.RemoveRange(menuItems);
            await _context.SaveChangesAsync();
        }

        public void SeedMenuItems()
        {
            for (var i = 0; i < 3; i++)
            {
                var maxItem = _context.MenuItems.Where(i => i.MenuId == 1 && i.ParentId == 0).OrderByDescending(i => i.Order).LastOrDefault();
                var item = new MenuItem
                {
                    Title = "Tieu de menu " + i,
                    Link = "#",
                    CreatedAt = DateTime.Now,
                    LastModifiedAt = DateTime.Now,
                    ParentId = 0,
                    MenuId = 1,
                    Order = maxItem != null ? maxItem.Order + 1 : 1
                };

                _context.MenuItems.Add(item);
                _context.SaveChanges();

                for (var j = 0; j < 3; j++)
                {
                    var maxItem2 = _context.MenuItems.Where(i => i.MenuId == 1 && i.ParentId == item.Id).OrderByDescending(i => i.Order).LastOrDefault();
                    var item2 = new MenuItem
                    {
                        Title = "Tieu de menu " + i + "." + j,
                        Link = "#",
                        CreatedAt = DateTime.Now,
                        LastModifiedAt = DateTime.Now,
                        ParentId = item.Id,
                        MenuId = 1,
                        Order = maxItem2 != null ? maxItem2.Order + 1 : 1
                    };

                    _context.MenuItems.Add(item2);
                    _context.SaveChanges();

                    for (var t = 0; t < 3; t++)
                    {
                        var maxItem3 = _context.MenuItems.Where(i => i.MenuId == 1 && i.ParentId == item2.Id).OrderByDescending(i => i.Order).LastOrDefault();
                        var item3 = new MenuItem
                        {
                            Title = "Tieu de menu " + i + "." + j + "." + t,
                            Link = "#",
                            CreatedAt = DateTime.Now,
                            LastModifiedAt = DateTime.Now,
                            ParentId = item2.Id,
                            MenuId = 1,
                            Order = maxItem3 != null ? maxItem3.Order + 1 : 1
                        };

                        _context.MenuItems.Add(item3);
                        _context.SaveChanges();

                        for (var k = 0; k < 3; k++)
                        {
                            var maxItem4 = _context.MenuItems.Where(i => i.MenuId == 1 && i.ParentId == item3.Id).OrderByDescending(i => i.Order).LastOrDefault();
                            var item4 = new MenuItem
                            {
                                Title = "Tieu de menu " + i + "." + j + "." + t + "." + k,
                                Link = "#",
                                CreatedAt = DateTime.Now,
                                LastModifiedAt = DateTime.Now,
                                ParentId = item3.Id,
                                MenuId = 1,
                                Order = maxItem4 != null ? maxItem4.Order + 1 : 1
                            };

                            _context.MenuItems.Add(item4);
                            _context.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
