using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecuritySystemsStore.Models
{
    public class Page
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Body { get; set; }

        [Required]
        public int Sorting { get; set; }

        [Required]
        public bool HasSidebar { get; set; }
    }
}
