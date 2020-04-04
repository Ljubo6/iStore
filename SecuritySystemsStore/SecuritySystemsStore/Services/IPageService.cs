using SecuritySystemsStore.Models;
using SecuritySystemsStore.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuritySystemsStore.Services
{
    public interface IPageService
    {
        Task<IEnumerable<AllPageVM>> GetAll();
        Task AddPageAsync(string title,string slug,string body,bool hasSidebar);
    }
}
