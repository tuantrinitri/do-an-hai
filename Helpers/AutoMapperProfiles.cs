using AutoMapper;
    
 
using CMS.Areas.GalleryManager.DTOs;
using CMS.Areas.GalleryManager.Models;
using CMS.Areas.HomepageManager.DTOs;
using CMS.Areas.HomepageManager.Models;
using CMS.Areas.Identity.DTOs;
using CMS.Areas.Identity.Models;
     
   
using CMS.Areas.MenuManager.DTOs;
using CMS.Areas.MenuManager.Models;
using CMS.Areas.PostManager.DTOs;
using CMS.Areas.PostManager.Models;
      
  
using CMS.Areas.SettingManager.Models;
using CMS.Areas.StaticPages.DTOs;
using CMS.Areas.StaticPages.Models;
using CMS.Areas.WidgetManager.DTOs;
using CMS.Areas.WidgetManager.Models;
using CMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
         
            CreateMap<UserForCreateDTO, User>();
            CreateMap<User, UserForEditDTO>();
            CreateMap<UserForEditDTO, User>();
            CreateMap<User, ProfileForEditDTO>();
            CreateMap<ProfileForEditDTO, User>();

            CreateMap<UnitForCreateDTO, Unit>();
            CreateMap<Unit, UnitForEditDTO>();
            CreateMap<UnitForEditDTO, Unit>();

            CreateMap<JobTitleForCreateDTO, JobTitle>();
            CreateMap<JobTitle, JobTitleForEditDTO>();
            CreateMap<JobTitleForEditDTO, JobTitle>();
            CreateMap<MenuItemForCreateDTO, MenuItem>();
            CreateMap<MenuItem, MenuItemForEditDTO>();
            CreateMap<MenuItemForEditDTO, MenuItem>();

 

            CreateMap<GalleryItemForCreateDTO, GalleryItem>();
            CreateMap<GalleryItem, GalleryItemForEditDTO>();
            CreateMap<GalleryItemForEditDTO, GalleryItem>();

            CreateMap<CreatePostCategoryDTO, PostCategory>();
            CreateMap<PostCategory, UpdatePostCategoryDTO>();
            CreateMap<UpdatePostCategoryDTO, PostCategory>();
            CreateMap<CreatePostDTO, Post>();
            CreateMap<Post, UpdatePostDTO>();
            CreateMap<UpdatePostDTO, Post>();

            CreateMap<StaticPage, UpdatePageDTO>();
            CreateMap<UpdatePageDTO, StaticPage>();

            CreateMap<GlobalSetting, HeaderBarViewModel>();
            CreateMap<HeaderBarViewModel, GlobalSetting>();

            CreateMap<CreateRightWidgetDTO, RightWidget>();
            CreateMap<RightWidget, CreateRightWidgetDTO>();
            CreateMap<UpdateRightWidgetDTO, RightWidget>();
            CreateMap<RightWidget, UpdateRightWidgetDTO>();

            CreateMap<CreateHomepageItemDTO, HomepageItem>();
            CreateMap<HomepageItem, UpdateHomepageItemDTO>();
            CreateMap<UpdateHomepageItemDTO, HomepageItem>();
        }
    }
}
