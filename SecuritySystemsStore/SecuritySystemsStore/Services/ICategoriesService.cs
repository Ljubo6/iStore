using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using SecuritySystemsStore.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuritySystemsStore.Services
{
    public interface ICategoriesService
    {
        Task<IEnumerable<CategoryVM>> GetAllCategories();
        string GetId(string catName);
        void ReorderCategories(int[] id);
        string RenameCategories(string newCatName, int id);
        ProductVM GetCategoriesList<T>();
        ProductVM CheckCategoriesList<T>(ProductVM model);
        Task<int> AddProductAsync(ProductVM input);
        IEnumerable<ProductVM> GetListOfProductsViews(int? catId);
    }
}
