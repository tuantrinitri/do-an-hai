using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Areas.PostManager.Models;
using CMS.Data;
using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using CMS.Areas.Identity.Models;
using Slugify;
using Newtonsoft.Json;
using CMS.Models;
using CMS.Areas.PostManager.DTOs;
using CMS.Helpers;
using System.Security.Claims;

namespace CMS.Areas.PostManager.Controllers
{
    [Area("PostManager")]
    [Route("AdminCP/Posts/{action=index}/{id?}")]
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public PostsController(ApplicationDbContext context, UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: PostManager/Posts
        [Breadcrumb("Bài viết")]
        public IActionResult Index(string keyword, int page = 1, string status = null, string category = "0")
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var unit = claims.Where(cl => cl.Type == "Unit").FirstOrDefault().Value;
            IQueryable<Post> posts = null;
            var otherDraft = _context.Posts.Where( pc => pc.IsDeleted != true).Select(p => !(p.ApprovalStatus == ApprovalStatuses.DRAFT && p.CreatedBy.UserName != User.Identity.Name));
            // Thu trưởng đơn vị được xem các bài viết của đơn vị
            if (User.IsInRole(RoleTypes.TT))
            {
                posts = _context.Posts
                .Include(d => d.JoinPostCategories).ThenInclude(jpc => jpc.PostCategory)
                .Include(u => u.CreatedBy).ThenInclude(u => u.Unit)
                .Where(p => p.IsDeleted != true)
                .Where(p => p.CreatedBy.Unit.ShortName == unit)
                .Where(p => !(p.ApprovalStatus == ApprovalStatuses.DRAFT && p.CreatedBy.UserName != User.Identity.Name))
                .OrderByDescending(u => u.Id).AsNoTracking();
            }
            // Cộng tác viên chỉ xem bài viết của mình
            else if (User.IsInRole(RoleTypes.CTV))
            {
                posts = _context.Posts
               .Include(d => d.JoinPostCategories).ThenInclude(jpc => jpc.PostCategory)
               .Include(u => u.CreatedBy).ThenInclude(u => u.Unit)
               .Where(p => p.IsDeleted != true)
               .Where(p => p.CreatedBy.UserName == User.Identity.Name)
               .OrderByDescending(u => u.Id).AsNoTracking();
            }
            // BTV được xem các bài viết của các đơn vị và các bài viết đã trình duyệt của mình
            else if(User.IsInRole(RoleTypes.BTV))
            {
                posts = _context.Posts
               .Include(d => d.JoinPostCategories).ThenInclude(jpc => jpc.PostCategory)
               .Include(u => u.CreatedBy).ThenInclude(u => u.Unit)
               .Where(p => p.IsDeleted != true)
               .Where( p => !(p.ApprovalStatus == ApprovalStatuses.PENDING_CTV))
               .Where( p => !(p.ApprovalStatus == ApprovalStatuses.PENDING_BTV && p.CreatedBy.UserName != User.Identity.Name))
               .Where(p => !(p.ApprovalStatus == ApprovalStatuses.DRAFT && p.CreatedBy.UserName != User.Identity.Name))
               .Where(p => !(p.ApprovalStatus == ApprovalStatuses.REFUSED && p.CreatedBy.UserName != User.Identity.Name))
               .OrderByDescending(u => u.Id).AsNoTracking();
            }
            // TBT được xem các bài viết của các đơn vị và các bài viết của các btv
            else if(User.IsInRole(RoleTypes.TBT) || User.IsInRole(RoleTypes.ADMIN))
            {
                posts = _context.Posts
               .Include(d => d.JoinPostCategories).ThenInclude(jpc => jpc.PostCategory)
               .Include(u => u.CreatedBy).ThenInclude(u => u.Unit)
               .Where(p => p.IsDeleted != true)
               .Where( p => !(p.ApprovalStatus == ApprovalStatuses.PENDING_CTV))
               .Where(p => !(p.ApprovalStatus == ApprovalStatuses.DRAFT && p.CreatedBy.UserName != User.Identity.Name))
               .Where(p => !(p.ApprovalStatus == ApprovalStatuses.REFUSED && p.CreatedBy.UserName != User.Identity.Name))
               .OrderByDescending(u => u.Id).AsNoTracking();
            }
            else 
            {
                posts = _context.Posts
               .Include(d => d.JoinPostCategories).ThenInclude(jpc => jpc.PostCategory)
               .Include(u => u.CreatedBy).ThenInclude(u => u.Unit)
               .Where(p => !(p.ApprovalStatus == ApprovalStatuses.REFUSED && p.CreatedBy.UserName != User.Identity.Name))
               .OrderByDescending(u => u.Id).AsNoTracking();
            }
            // Nếu có từ khóa tìm kiếm
            if (!String.IsNullOrEmpty(keyword))
            {
                ViewData["filter_keyword"] = keyword;
                posts = (IQueryable<Post>)posts.Where(d => d.Title.Contains(keyword));
            }

            // Nếu có tìm theo trạng thái duyệt
            if (!String.IsNullOrEmpty(status))
            {
                if (status == ApprovalStatuses.PENDING)
                {
                    // Chờ duyệt của CTV và Admin là tất cả các trạng thái pending
                    if (User.IsInRole(RoleTypes.CTV) || User.IsInRole(RoleTypes.ADMIN))
                    {
                        posts = (IQueryable<Post>)posts.Where(d => d.ApprovalStatus.Contains(ApprovalStatuses.PENDING));
                    }
                    // Chờ duyệt của thủ trưởng là pending_CTV
                    else if (User.IsInRole(RoleTypes.TT))
                    {
                        posts = (IQueryable<Post>)posts.Where(d => d.ApprovalStatus == ApprovalStatuses.PENDING_CTV);
                    }
                    // Chờ duyệt của BTV là Pending_TT
                    else if (User.IsInRole(RoleTypes.BTV))
                    {
                        posts = (IQueryable<Post>)posts.Where(d => d.ApprovalStatus == ApprovalStatuses.PENDING_TT);
                    }
                    // Chờ duyệt của TBT là Pending_BTV và Pending_TT
                    else if (User.IsInRole(RoleTypes.TBT))
                    {
                        posts = (IQueryable<Post>)posts.Where(d => (d.ApprovalStatus == ApprovalStatuses.PENDING_TT || d.ApprovalStatus == ApprovalStatuses.PENDING_BTV));
                    }
                }
                else if (status == ApprovalStatuses.REFUSED)
                {
                    posts = (IQueryable<Post>)posts.Where(p => p.CreatedBy.UserName == User.Identity.Name && p.ApprovalStatus == ApprovalStatuses.REFUSED);

                }
                else
                {
                    posts = (IQueryable<Post>)posts.Where(p => p.ApprovalStatus == status);
                }
                ViewData["filter_status"] = status;
            }

            // Nếu có tìm kiếm theo danh mục
            if (!String.IsNullOrEmpty(category) && int.Parse(category) > 0)
            {
                ViewData["filter_category"] = category;
                posts = (IQueryable<Post>)posts
                .Where(p => p.JoinPostCategories.Any(j => j.PostCategory.Id == int.Parse(category)));
            }

            var totalCount = posts.Count();

            // status dropdown menu của CTV 
            if (User.IsInRole(RoleTypes.CTV))
            {
                ViewData["status_list"] = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem{ Value = ApprovalStatuses.DRAFT, Text = "Lưu nháp"},
                    new SelectListItem{ Value = ApprovalStatuses.PUBLISHED, Text = "Đã xuất bản"},
                    new SelectListItem{ Value = ApprovalStatuses.REFUSED, Text = "Bị từ chối"},
                    new SelectListItem{ Value = ApprovalStatuses.PENDING, Text = "Đang chờ duyệt" }
                }, "Value", "Text", new { Value = status });
            }
            // status dropdown menu của TT 
            else if (User.IsInRole(RoleTypes.TT))
            {
                ViewData["status_list"] = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem{ Value = ApprovalStatuses.DRAFT, Text = "Lưu nháp"},
                    new SelectListItem{ Value = ApprovalStatuses.PUBLISHED, Text = "Đã xuất bản"},
                    new SelectListItem{ Value = ApprovalStatuses.REFUSED, Text = "Bị từ chối"},
                    new SelectListItem{ Value = ApprovalStatuses.PENDING_CTV, Text = "Chưa duyệt" },
                    new SelectListItem{ Value = ApprovalStatuses.PENDING_TT, Text = "Đang chờ duyệt" }
                }, "Value", "Text", new { Value = status });
            }
            // status dropdown menu của BTV
            else if (User.IsInRole(RoleTypes.BTV))
            {
                ViewData["status_list"] = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem{ Value = ApprovalStatuses.DRAFT, Text = "Lưu nháp"},
                    new SelectListItem{ Value = ApprovalStatuses.PUBLISHED, Text = "Đã xuất bản"},
                    new SelectListItem{ Value = ApprovalStatuses.REFUSED, Text = "Bị từ chối"},
                    new SelectListItem{ Value = ApprovalStatuses.PENDING_BTV, Text = "Chưa duyệt" },
                    new SelectListItem{ Value = ApprovalStatuses.PENDING, Text = "Đang chờ duyệt" }
                }, "Value", "Text", new { Value = status });
            }
            // status dropdown menu của TBT
            else if (User.IsInRole(RoleTypes.TBT))
            {
                ViewData["status_list"] = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem{ Value = ApprovalStatuses.PUBLISHED, Text = "Đã xuất bản"},
                    new SelectListItem{ Value = ApprovalStatuses.REFUSED, Text = "Bị từ chối"},
                    new SelectListItem{ Value = ApprovalStatuses.PENDING, Text = "Đang chờ duyệt" }
                }, "Value", "Text", new { Value = status });
            }
            // Menu của Admin
            else
            {
                ViewData["status_list"] = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem{ Value = ApprovalStatuses.DRAFT, Text = "Lưu nháp"},
                    new SelectListItem{ Value = ApprovalStatuses.PUBLISHED, Text = "Đã xuất bản"},
                    new SelectListItem{ Value = ApprovalStatuses.REFUSED, Text = "Bị từ chối"},
                    new SelectListItem{ Value = ApprovalStatuses.PENDING, Text = "Đang chờ duyệt" }
                }, "Value", "Text", new { Value = status });
            }
            List<SelectListItem> categories = _context.PostCategories.AsNoTracking()
                                .OrderBy(c => c.Name)
                                    .Select(n =>
                                    new SelectListItem
                                    {
                                        Value = n.Id.ToString(),
                                        Text = n.Name
                                    }).ToList();
            ViewData["category_list"] = new SelectList(categories, "Value", "Text", new SelectListItem { Value = category });
            //return View(await _context.Posts.ToListAsync());
            int pageSize = 10;
            var pagination = PaginatedList<Post>.Create(posts, page, pageSize);
            ViewData["totalitem"] = posts.Count();
            ViewData["Title"] = status == ApprovalStatuses.DRAFT ? "Danh sách bài viết lưu nháp"
                                         : status == ApprovalStatuses.PENDING ? "Danh sách bài viết chờ duyệt"
                                         : status == ApprovalStatuses.REFUSED ? "Danh sách bài viết bị từ chối"
                                         : status == ApprovalStatuses.PUBLISHED ? "Danh sách bài viết đã xuất bản"
                                         : "Danh sách bài viết";
            return View(pagination);
        }

        // GET: PostManager/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
           .Include(p => p.JoinPostCategories)
           .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            HashSet<int> selectedIds = post.JoinPostCategories.Select(c => c.CategoryId).ToHashSet();
            UpdatePostDTO postDTO = _mapper.Map<UpdatePostDTO>(post);

            List<SelectPostCategoryDTO> categoryDTOs = _context.PostCategories.Select(pc =>
                new SelectPostCategoryDTO()
                {
                    Id = pc.Id,
                    Name = pc.Name,
                    IsSelected = selectedIds.Contains(pc.Id)
                }).ToList();
            postDTO.PostCategories = categoryDTOs;

            return View(postDTO);
        }

        // GET: PostManager/Posts/Create
        [Breadcrumb("Tạo mới")]
        public IActionResult Create()
        {
            if (!_context.PostCategories.Any())
            {
                TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                {
                    Type = "warning",
                    Message = "Vui lòng tạo chuyên mục bài viết trước"
                });
                return RedirectToAction("Create", "PostCategories");
            }

            List<SelectPostCategoryDTO> categoryDTOs = _context.PostCategories.Select(pc =>
                new SelectPostCategoryDTO()
                {
                    Id = pc.Id,
                    Name = pc.Name
                }).ToList();

            return View(new CreatePostDTO
            {
                IsFeatured = true,
                PostCategories = categoryDTOs
            });
        }

        // POST: PostManager/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostDTO postDTO, string submitBtn)
        {

            if (ModelState.IsValid)
            {

                Post post = _mapper.Map<Post>(postDTO);

                post.JoinPostCategories = new List<JoinPostCategory>();
                var selectedCats = postDTO.PostCategories.Where(c => c.IsSelected);
                if (selectedCats.Count() == 0)
                {
                    ModelState.AddModelError("PostCategories", "Chưa chọn danh mục");
                    return View(postDTO);
                }
                foreach (SelectPostCategoryDTO selectedCat in selectedCats)
                {
                    PostCategory pc = await _context.PostCategories.FindAsync(selectedCat.Id);
                    post.JoinPostCategories.Add(new JoinPostCategory
                    {
                        PostCategory = pc,
                        Post = post
                    });
                }

                post.Title = post.Title.FullTrim();
                post.CreatedAt = DateTime.Now;
                post.UpdatedAt = DateTime.Now;
                post.CreatedById = User.GetId();
                post.UpdatedById = User.GetId();
                // generate slug
                SlugHelper.Config slugConfig = new SlugHelper.Config();
                slugConfig.StringReplacements.Add("đ", "d");
                SlugHelper _slugHelper = new SlugHelper(slugConfig);
                string slug = _slugHelper.GenerateSlug(post.Title.FullTrim());
                var slugCount = await _context.Posts.Where(p => p.Slug == slug).CountAsync();
                post.Slug = slugCount > 0 ? slug + "-" + Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds) : slug;
                // update approval status
                if (submitBtn == ApprovalStatuses.PENDING)
                {
                    if (User.IsInRole(RoleTypes.CTV))
                    {
                        post.ApprovalStatus = ApprovalStatuses.PENDING_CTV;
                    }
                    else if (User.IsInRole(RoleTypes.TT))
                    {
                        post.ApprovalStatus = ApprovalStatuses.PENDING_TT;
                    }
                    else if (User.IsInRole(RoleTypes.BTV))
                    {
                        post.ApprovalStatus = ApprovalStatuses.PENDING_BTV;
                    }
                    else if (User.IsInRole(RoleTypes.ADMIN))
                    {
                        post.ApprovalStatus = ApprovalStatuses.PUBLISHED;
                    }
                    else
                    {
                        post.ApprovalStatus = ApprovalStatuses.DRAFT;
                    }
                }
                else if (submitBtn == ApprovalStatuses.PUBLISHED)
                {
                    post.ApprovalStatus = ApprovalStatuses.PUBLISHED;
                    post.ApprovalAt = DateTime.Now;
                }
                else
                {
                    post.ApprovalStatus = ApprovalStatuses.DRAFT;
                }

                _context.Add(post);
                await _context.SaveChangesAsync();
                TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                {
                    Type = "success",
                    Message = "Tạo dữ liệu thành công"
                });
                return RedirectToAction(nameof(Index));
            }
            // ViewData["PostCategories"] = new SelectList(_context.PostCategories, "Id", "Name");
            ViewData["PostCategories"] = await _context.PostCategories.ToListAsync();
            postDTO.PostCategories = _context.PostCategories.Select(pc =>
                new SelectPostCategoryDTO()
                {
                    Id = pc.Id,
                    Name = pc.Name
                }).ToList();
            return View(postDTO);
        }

        // GET: PostManager/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
            .Include(p => p.JoinPostCategories)
            .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            HashSet<int> selectedIds = post.JoinPostCategories.Select(c => c.CategoryId).ToHashSet();
            UpdatePostDTO postDTO = _mapper.Map<UpdatePostDTO>(post);

            List<SelectPostCategoryDTO> categoryDTOs = _context.PostCategories.Select(pc =>
                new SelectPostCategoryDTO()
                {
                    Id = pc.Id,
                    Name = pc.Name,
                    IsSelected = selectedIds.Contains(pc.Id)
                }).ToList();
            postDTO.PostCategories = categoryDTOs;

            return View(postDTO);
        }

        // POST: PostManager/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePostDTO postDTO, string submitBtn)
        {
            if (id != postDTO.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Post post = await _context.Posts
                    .Include(p => p.JoinPostCategories)
                    .FirstOrDefaultAsync(p => p.Id == id);
                    Post postToUpdate = _mapper.Map<UpdatePostDTO, Post>(postDTO, post);
                    postToUpdate.UpdatedAt = DateTime.Now;
                    postToUpdate.UpdatedById = User.GetId();

                    SlugHelper.Config slugConfig = new SlugHelper.Config();
                    slugConfig.StringReplacements.Add("đ", "d");
                    SlugHelper helper = new SlugHelper(slugConfig);
                    string slug = helper.GenerateSlug(postToUpdate.Title);
                    var slugCount = await _context.StaticPages.Where(dc => dc.Slug == slug && dc.Id != post.Id).CountAsync();
                    postToUpdate.Slug = slugCount > 0 ? slug + "-" + post.Id.ToString() : slug;

                    postToUpdate.Title = postToUpdate.Title.FullTrim();
                    // update category
                    postToUpdate.JoinPostCategories = new List<JoinPostCategory>();
                    var selectedCats = postDTO.PostCategories.Where(c => c.IsSelected);
                    if (selectedCats.Count() == 0)
                    {
                        ModelState.AddModelError("PostCategories", "Chưa chọn danh mục");
                        return View(postDTO);
                    }
                    foreach (SelectPostCategoryDTO selectedCat in selectedCats)
                    {
                        PostCategory pc = await _context.PostCategories.FindAsync(selectedCat.Id);
                        postToUpdate.JoinPostCategories.Add(new JoinPostCategory
                        {
                            PostCategory = pc,
                            Post = postToUpdate
                        });
                    }

                    // update approval status
                    if (submitBtn == ApprovalStatuses.PENDING)
                    {
                        if (User.IsInRole(RoleTypes.CTV))
                        {
                            postToUpdate.ApprovalStatus = ApprovalStatuses.PENDING_CTV;
                        }
                        else if (User.IsInRole(RoleTypes.TT))
                        {
                            postToUpdate.ApprovalStatus = ApprovalStatuses.PENDING_TT;
                        }
                        else if (User.IsInRole(RoleTypes.BTV))
                        {
                            postToUpdate.ApprovalStatus = ApprovalStatuses.PENDING_BTV;
                        }
                        else if (User.IsInRole(RoleTypes.TBT) || User.IsInRole(RoleTypes.ADMIN))
                        {
                            postToUpdate.ApprovalStatus = ApprovalStatuses.PUBLISHED;
                        }
                        else
                        {
                            postToUpdate.ApprovalStatus = ApprovalStatuses.DRAFT;
                        }
                    }
                    else if (submitBtn == ApprovalStatuses.PUBLISHED)
                    {
                        postToUpdate.ApprovalStatus = ApprovalStatuses.PUBLISHED;
                        postToUpdate.ApprovalAt = DateTime.Now;
                    }
                    else if (submitBtn == ApprovalStatuses.DRAFT)
                    {
                        postToUpdate.ApprovalStatus = ApprovalStatuses.DRAFT;
                    }
                    else if (submitBtn == ApprovalStatuses.REFUSED)
                    {
                        postToUpdate.ApprovalStatus = ApprovalStatuses.REFUSED;
                    }
                    _context.Update(postToUpdate);
                    await _context.SaveChangesAsync();
                    TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                    {
                        Type = "success",
                        Message = submitBtn == ApprovalStatuses.DRAFT ? "Lưu nháp thành công"
                                               : submitBtn == ApprovalStatuses.PUBLISHED ? "Xuất bản thành công"
                                               : submitBtn == ApprovalStatuses.REFUSED ? "Đã từ chối duyệt bài viết"
                                               : (User.IsInRole(RoleTypes.TT) || User.IsInRole(RoleTypes.BTV) || User.IsInRole(RoleTypes.TBT)) ? "Duyệt bài viết thành công"
                                               : "Cập nhật dữ liệu thành công"
                    });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(postDTO.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            }

            return View(postDTO);
        }

        // POST: PostManager/Posts/Delete/5
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

            var postToDelete = await _context.Posts.FindAsync(id);
            postToDelete.IsDeleted = true;
            _context.Update(postToDelete);
            await _context.SaveChangesAsync();
            return Json(new
            {
                Status = true,
                Message = "Đã xóa dữ liệu thành công"
            });
        }

        // POST: PostManager/Posts/Refuse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Refuse(RefusePostDTO refusePost)
        {
            if (refusePost.Id == null)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Không tìm thấy bài viết"
                });
            }

            var postToRefuse = await _context.Posts.FindAsync(refusePost.Id);
            postToRefuse.ApprovalReason = refusePost.Reason;
            postToRefuse.ApprovalAt = DateTime.Now;
            postToRefuse.ApprovalStatus = ApprovalStatuses.REFUSED;
            _context.Update(postToRefuse);
            await _context.SaveChangesAsync();
            return Json(new
            {
                Status = true,
                Message = "Từ chối duyệt bài viết thành công"
            });
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
