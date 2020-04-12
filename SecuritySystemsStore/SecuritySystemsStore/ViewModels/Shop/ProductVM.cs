using Microsoft.AspNetCore.Mvc.Rendering;
using SecuritySystemsStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecuritySystemsStore.ViewModels.Shop
{
    public class ProductVM
    {
        public ProductVM()
        {

        }
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Slug { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public string CategoryName { get; set; }

        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [DisplayName("Image")]
        public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }
    }
}
