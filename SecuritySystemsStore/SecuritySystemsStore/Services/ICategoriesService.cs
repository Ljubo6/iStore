using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using SecuritySystemsStore.Models;
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
        IEnumerable<ProductVM> GetListOfProductsViews(List<Product> listOfProducts);
        ProductVM GetAllProducts<T>(int id,Product product);
        IEnumerable<Product> GetListOfProducts(int? catId);
        ProductVM FillModel<T>(int id, ProductVM model);
        Task UploadProduct(ProductVM model,Product product);
        Task Delete(int id);
    }
}
