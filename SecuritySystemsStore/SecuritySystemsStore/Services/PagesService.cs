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
    public class PagesService : IPagesService
    {
        private readonly ApplicationDbContext db;

        public PagesService(ApplicationDbContext db)
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

        public async Task EditPageAsync(EditPageVM newInput)
        {
            var page = await this.db.Pages.FindAsync(newInput.Id);

            page.Title = newInput.Title;
            page.Slug = newInput.Slug;
            page.Body = newInput.Body;
            page.HasSidebar = newInput.HasSidebar;

            await this.db.SaveChangesAsync();
        }

        public async Task EditSidebar(int id, string body)
        {
            var sidebar = this.db.Sidbar.Find(id);

            sidebar.Body = body;

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

        public DetailsPageVM GetDetailsView<T>(Page page)
        {
            var viewModel = new DetailsPageVM
            {
                Id = page.Id,
                Title = page.Title,
                Slug = page.Slug,
                Body = page.Body,
            };
            return viewModel;
        }

        public EditPageInputVM GetEditView<T>(Page page)
        {
            var viewModel = new EditPageInputVM
            {
                Title = page.Title,
                Slug = page.Slug,
                Body = page.Body,
                HasSidebar = page.HasSidebar,
            };
            return viewModel;
        }

        public async Task<IEnumerable<SidebarVM>> GetSidebar()
        {
            var sidebar = this.db.Sidbar.Select(x => new SidebarVM
            {
                Id = x.Id,
                Body = x.Body,
            }).ToListAsync();

            return await sidebar;
        }

        public bool IsSlugAddUnique(AddPageVM input)
        {
            if (db.Pages.Any(x => x.Slug == input.Slug))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsSlugEditUnique(EditPageInputVM input, string slug)
        {
            if (this.db.Pages.Where(x => x.Id != input.Id).Any(x => x.Slug == slug))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsTitleAddUnique(AddPageVM input)
        {
            if (db.Pages.Any(x => x.Title == input.Title))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsTitleEditUnique(EditPageInputVM input)
        {
            if (this.db.Pages.Where(x => x.Id != input.Id).Any(x => x.Title == input.Title))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void ReorderPages(int[] id)
        {
            int count = 1;

            foreach (var pageId in id)
            {
                var page = this.db.Pages.Find(pageId);

                page.Sorting = count;

                this.db.SaveChanges();

                count++;
            }           
        }

        public string ReturnSlug(EditPageInputVM input)
        {
            string slug = "home";
            if (input.Slug != "home")
            {
                if (string.IsNullOrWhiteSpace(input.Slug))
                {
                    slug = input.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = input.Slug.Replace(" ", "-").ToLower();
                }
            }
            return slug;
        }

        public SidebarVM ViewEditSidebar<T>()
        {
            var sidebar = this.db.Sidbar.FirstOrDefault();
            var model = new SidebarVM
            {
                Id = sidebar.Id,
                Body = sidebar.Body,
            };

            return model;
        }
    }
}
