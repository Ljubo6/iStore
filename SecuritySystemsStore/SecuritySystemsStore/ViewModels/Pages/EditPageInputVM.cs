using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuritySystemsStore.ViewModels.Pages
{
    public class EditPageInputVM
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Slug { get; set; }

        public string Body { get; set; }

        public bool HasSidebar { get; set; }
    }
}
