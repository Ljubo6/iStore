using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecuritySystemsStore.ViewModels.Pages
{
    public class EditPageVM
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        public string Slug { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        public string Body { get; set; }


        [Display(Name = "Sidebar")]
        public bool HasSidebar { get; set; }
    }
}
