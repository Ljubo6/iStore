using SecuritySystemsStore.Models;
using SecuritySystemsStore.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuritySystemsStore.Services
{
    public interface IPagesService
    {
        Task<IEnumerable<AllPageVM>> GetAll();
        Task AddPageAsync(string title,string slug,string body,bool hasSidebar);
        //Task<Page> GetById(int? id);
        Task EditPageAsync(EditPageVM newInput);
        string ReturnSlug(EditPageInputVM input);
        bool IsTitleEditUnique(EditPageInputVM input);
        bool IsSlugEditUnique(EditPageInputVM input, string slug);
        bool IsTitleAddUnique(AddPageVM input);
        bool IsSlugAddUnique(AddPageVM input);
    }
}
