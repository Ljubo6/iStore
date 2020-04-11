using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using SecuritySystemsStore.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecuritySystemsStore.SeedData;
using SecuritySystemsStore.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SecuritySystemsStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddAutoMapper(typeof(Startup));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSession();
            services.AddDistributedMemoryCache();



            services.AddTransient<IPagesService, PagesService>();
            services.AddTransient<ICategoriesService, CategoriesService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scopedService = app.ApplicationServices.CreateScope())
            {
                var dbContext = scopedService.ServiceProvider.GetService<ApplicationDbContext>();

                if (env.IsDevelopment())
                {
                    dbContext.Database.Migrate();
                }

                new ApplicationDbContextSeeder(scopedService.ServiceProvider, dbContext)
                    .SeedDataAsync()
                    .GetAwaiter()
                    .GetResult();

                new SidebarSeeder(scopedService.ServiceProvider, dbContext)
                    .SeedDataAsync()
                    .GetAwaiter()
                    .GetResult();
                new CategorySeeder(scopedService.ServiceProvider, dbContext)
                   .SeedDataAsync()
                   .GetAwaiter()
                   .GetResult();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "areaRoute",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
