using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Entities.ViewModels;
using myshop.Utilities;
using System.Security.Claims;
using X.PagedList;

namespace myshop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper mapper;

        public HomeController(IUnitOfWork unitOfWork
            , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public IActionResult Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 8;


            var products = _unitOfWork.Repository<Product>().GetAll();
            var productsVM = mapper.Map<IEnumerable<ProductViewModel>>(products);
            return View(productsVM.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public IActionResult Details (int productId)
        {
            if (productId == null)
            {
                return NotFound();
            }
            var _product = _unitOfWork.Repository<Product>().GetFirstOrDefault(x => x.Id == productId, "Category");
            if (_product == null)
                return NotFound();

            return View(new myshop.Entities.Models.ShoppingCart
            {
                productId = productId,
                product = _product,
                Count = 1
            });
        }
        // we wanna the user that add item to cart must be signed in 
        // so we add this data annotation 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(myshop.Entities.Models.ShoppingCart shoppingCart)
        {
            if (shoppingCart == null)
                return NotFound();

            var userIdentity = (ClaimsIdentity)User.Identity;
            var claim = userIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            var cart = _unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().GetFirstOrDefault(
                x => x.ApplicationUserId == claim.Value && x.productId == shoppingCart.productId);
            if (cart  == null)
            {
                _unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().Add(shoppingCart);
                _unitOfWork.Complete();

                HttpContext.Session.SetInt32(SD.SessionKey,
                    _unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count);
            }
            else
            {
                cart.Count += shoppingCart.Count;
                _unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().Update(cart);
                _unitOfWork.Complete();
            }

            return RedirectToAction("Index");
        }
    }
}
