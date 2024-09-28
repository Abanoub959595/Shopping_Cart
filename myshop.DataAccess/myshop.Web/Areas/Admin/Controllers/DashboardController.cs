using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Utilities;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            ViewBag.Orders = unitOfWork.Repository<OrderHeader>().GetAll().Count();
            ViewBag.ApprovedOrders = unitOfWork.Repository<OrderHeader>().GetAll(x => x.OrderStatus == SD.Approve).Count();
            ViewBag.Users = unitOfWork.Repository<ApplicationUser>().GetAll().Count();
            ViewBag.Products = unitOfWork.Repository<Product>().GetAll().Count();   

            return View();
        }
    }
}
