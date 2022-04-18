using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Helpers
{
    public class ContextHelper
    {
        public ApplicationDbContext _context { get; private set; }

        public ContextHelper(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
