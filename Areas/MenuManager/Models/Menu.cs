using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace CMS.Areas.MenuManager.Models
{
    public class Menu
    {
        public int Id { get; set; }

        public string Alias { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool Published { get; set; }
    }
}
