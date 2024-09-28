using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Data;
using myshop.DataAccess.Implementation;
using myshop.Entities.Repositories;
using myshop.Web.Mapper;
using Microsoft.AspNetCore.Identity;
using myshop.Entities.Models;
using Microsoft.Win32;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity.UI.Services;
using myshop.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Stripe;
using myshop.DataAccess.DbInializer;

namespace myshop.Web
{
	public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IDbInializer, DbInializer>();

            builder.Services.AddSingleton<IEmailSender, EmailSender>();

            builder.Services.AddDbContext<AppDBContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.Configure<StripeDetails>(builder.Configuration.GetSection("Stripe"));


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(30);
                option.Lockout.MaxFailedAccessAttempts = 3;
            })
            .AddEntityFrameworkStores<AppDBContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();
            builder.Services.AddAutoMapper(map => map.AddProfile(new ProductProfile()));


            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();


            var app = builder.Build();
            

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecertKey").Get<string>();

            SeedDB();

            app.UseAuthorization();

            app.UseSession();


            app.MapRazorPages();


            //app.MapControllerRoute(
            //    name: "default",
            //    pattern: "{area=Customer}/{controller=Category}/{action=Index}/{id?}");



            //app.MapControllerRoute(
            //    name: "default",
            //    pattern: "{area=Admin}/{controller=Category}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");


            app.Run();

            void SeedDB()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbInializer = scope.ServiceProvider.GetRequiredService<IDbInializer>();
                    dbInializer.Inialize();
                }
            }

        }
    }
}