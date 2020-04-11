using AutoMapper;
using SecuritySystemsStore.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuritySystemsStore.Models
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Product,ProductVM>();
        }
    }
}
