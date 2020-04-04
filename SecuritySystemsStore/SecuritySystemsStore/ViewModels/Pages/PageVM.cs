using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecuritySystemsStore.ViewModels.Pages
{
    public class PageVM
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Body { get; set; }

        public int Sorting { get; set; }

        public bool HasSidebar { get; set; }
    }
}
