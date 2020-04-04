using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecuritySystemsStore.Data;
using SecuritySystemsStore.Models;
using SecuritySystemsStore.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuritySystemsStore.Services
{
    public class PageService : IPageService
    {
        private readonly ApplicationDbContext db;

        public PageService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task AddPageAsync(string title, string slug, string body, bool hasSidebar)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                slug = slug.Replace(" ","-").ToLower();
            }
            else
            {
                slug = slug.Replace(" ", "-").ToLower();
            }

            var page = new Page
            {
                Title = title.ToUpper(),
                Slug = slug,
                Body = body,
                HasSidebar = hasSidebar,
            };

            await this.db.AddAsync(page);
            await this.db.SaveChangesAsync();
        }

        public async Task<IEnumerable<AllPageVM>> GetAll()
        {
           
            var pageList = db.Pages.OrderBy(x => x.Sorting).Select(x => new AllPageVM
            {
                Id = x.Id,
                Title = x.Title,
                Slug = x.Slug,
                HasSidebar = x.HasSidebar,
            }).ToListAsync();

            return await pageList;
        }
    }
}
