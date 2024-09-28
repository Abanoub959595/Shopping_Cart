using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.DataAccess.Data;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Utilities;
using System.Security.Claims;
using X.PagedList;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly AppDBContext context;
        private readonly IUnitOfWork unitOfWork;

        public UsersController(AppDBContext context,
            IUnitOfWork unitOfWork)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 8;


            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            var users = context.Users.Where(user => user.Id != userId).ToList();    
            return View(users.ToPagedList(pageNumber, pageSize));
        }
        public IActionResult LockUnlock (string? id)
        {
            if (id is null)
                return NotFound();
            var user = unitOfWork.Repository<ApplicationUser>().GetFirstOrDefault(user => user.Id == id);
            if (user is null)
                return NotFound();

            if (user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
                user.LockoutEnd = DateTime.Now.AddDays(4);
            else 
                user.LockoutEnd = DateTime.Now;


            context.SaveChanges();
            return RedirectToAction("Index", "Users", new {area = "Admin"});
        }

        public IActionResult Delete(string id)
        {
            var user = unitOfWork.Repository<ApplicationUser>().GetFirstOrDefault(x => x.Id == id);
            if (user is null)
                return NotFound();


            unitOfWork.Repository<ApplicationUser>().Delete(user);
            unitOfWork.Complete();
            return RedirectToAction("Index");


        }


    }
}
