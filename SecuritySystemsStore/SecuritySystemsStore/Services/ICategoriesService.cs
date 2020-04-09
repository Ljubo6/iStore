using SecuritySystemsStore.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuritySystemsStore.Services
{
    public interface ICategoriesService
    {
        Task <IEnumerable<CategoryVM>> GetAllCategories();
    }
}
