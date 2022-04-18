using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.PostManager.DTOs
{
    public class RefusePostDTO
    {
        public string Reason { get; set; }
        public int? Id { get; set; }

    }
}
