using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Data;
using myshop.Entities.Models;

namespace myshop.DataAccess.DbInializer
{
    public class DbInializer : IDbInializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDBContext appDBContext;

        public DbInializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDBContext appDBContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.appDBContext = appDBContext;
        }


        public void Inialize()
        {
            // Migration 
            try
            {
                try
                {
                    if (appDBContext.Database.GetPendingMigrations().Count() > 0)
                    {
                        appDBContext.Database.Migrate();
                    }
                }
                catch (Exception)
                {

                    throw;
                }

                // Roles 
                if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole("Editor")).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole("Customer")).GetAwaiter().GetResult();

                    // User




                    var res = _userManager.CreateAsync(new ApplicationUser()
                    {
                        UserName = "Admin@myshop.com",
                        Email = "Admin@myshop.com",
                        Name = "Administrator",
                        PhoneNumber = "4565645",
                        Address = "Alex",
                        City = "Smoha",
                    }, "Admin1@2Admin").GetAwaiter().GetResult();


                    if (res.Succeeded)
                    {

                        appDBContext.SaveChanges();

                        ApplicationUser applicationUser = appDBContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == "Admin@myshop.com").GetAwaiter().GetResult();

                        _userManager.AddToRoleAsync(applicationUser, "Admin").GetAwaiter().GetResult();

                        appDBContext.SaveChanges();

                    }






                }


                return;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
