using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Entities.ViewModels;
using myshop.Utilities;
using Stripe.Checkout;
using System.Security.Claims;

namespace myshop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
		private readonly IMapper mapper;

		public CartController(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
			this.mapper = mapper;
		}
        public IActionResult Index()
        {
            var userIdentity = (ClaimsIdentity)User.Identity;
            var claim = userIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userid = claim.Value;

            var items = unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().GetAll(x => x.ApplicationUserId == userid, "product");

            //ViewBag["CartNumber"] = items.Count();

            decimal total = 0; 

            foreach(var cart in items)
            {
                total += cart.Count * cart.product.Price;
            }

            
            return View(new ShoppingCartViewModel
            {
                Total = total,
                cartList = items
            });
        }
        public IActionResult Summary()
        {
            var userIdentity = (ClaimsIdentity)User.Identity;
            var claim = userIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            var carts = unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().GetAll(x => x.ApplicationUserId == userId, "product");

            var appUser = unitOfWork.Repository<ApplicationUser>().GetFirstOrDefault(x => x.Id == userId);  

            OrderHeader orderHeader = new OrderHeader()
            {
                Address = appUser.Address,
                City = appUser.City,    
                Phone = appUser.PhoneNumber,
                Name = appUser.Name,
                ApplicationUser = appUser

            };

            decimal totalprice = 0;
            foreach (var item in carts)
                totalprice += item.product.Price * item.Count;

            return View(new ShoppingCartViewModel()
            {
                OrderHeader = orderHeader,
                cartList = carts,
                Total = totalprice
                
			});

        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult PostSummary(ShoppingCartViewModel shoppingCartViewModel)
        {
            // fill the tables => OrderHeader and OrderDetails 
            var userClaim = (ClaimsIdentity)User.Identity;
            var claim = userClaim.FindFirst(ClaimTypes.NameIdentifier);

            var user = unitOfWork.Repository<ApplicationUser>().GetFirstOrDefault(x => x.Id == claim.Value);


            shoppingCartViewModel.cartList = unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().GetAll(x => x.ApplicationUserId == claim.Value, includeWord: "product");

            shoppingCartViewModel.OrderHeader.OrderStatus = SD.Pending;
            shoppingCartViewModel.OrderHeader.PaymentStatus = SD.Pending;
            shoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;
            shoppingCartViewModel.OrderHeader.PaymentDate = DateTime.Now;
            shoppingCartViewModel.OrderHeader.ApplicationUserId = claim.Value;

            shoppingCartViewModel.OrderHeader.ApplicationUser = user;

            foreach (var item in shoppingCartViewModel.cartList)
            {
                shoppingCartViewModel.OrderHeader.TotalPrice += (item.product.Price * item.Count);
            }

            unitOfWork.Repository<OrderHeader>().Add(shoppingCartViewModel.OrderHeader);
            unitOfWork.Complete();

            foreach(var item in shoppingCartViewModel.cartList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    Product = item.product,
                    OrderHeaderId = shoppingCartViewModel.OrderHeader.Id,
                    Price = item.product.Price,
                    Count = item.Count
                };
                unitOfWork.Repository<OrderDetail>().Add(orderDetail);
                unitOfWork.Complete();
            }

            var domain = "https://localhost:44343/";

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain+$"customer/cart/orderconfermation?id={shoppingCartViewModel.OrderHeader.Id}",
                CancelUrl = domain+"customer/cart/index",
            };

            foreach(var item in shoppingCartViewModel.cartList)
            {
                var sessionLineOptions =  new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.product.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.product.Name,
                        },
                    },
                    Quantity = 1,
                };
                options.LineItems.Add(sessionLineOptions);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            shoppingCartViewModel.OrderHeader.SessionId = session.Id;
            unitOfWork.Complete();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);


        }

        public IActionResult orderconfermation (int id)
        {
            OrderHeader orderHeader = unitOfWork.Repository<OrderHeader>().GetFirstOrDefault(x => x.Id == id);
            var servise = new SessionService();
            Session session = servise.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                orderHeader.OrderStatus = SD.Approve;
                orderHeader.PaymentStatus = SD.Approve;
                orderHeader.PaymentIntentId = session.PaymentIntentId;
                unitOfWork.Repository<OrderHeader>().Update(orderHeader);
                unitOfWork.Complete();
            }

            List<myshop.Entities.Models.ShoppingCart> shoppingCarts = unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().DeleteRange(shoppingCarts);
            unitOfWork.Complete();

            HttpContext.Session.Clear();

            return View(id);
        }


        public IActionResult Plus (int? cartId)
        {
            if (cartId == null)
                return NotFound();

			var cart = unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().GetFirstOrDefault(x => x.Id == cartId, "product");
            cart.Count += 1;
            unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().Update(cart);
            unitOfWork.Complete();
            return RedirectToAction("Index");
		}
        public IActionResult Minus (int? cartId)
        {
			if (cartId == null)
				return NotFound();

			var cart = unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().GetFirstOrDefault(x => x.Id == cartId, "product");
			cart.Count -= 1;
            if (cart.Count <= 0)
            {
                unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().Delete(cart);


				var userIdentity = (ClaimsIdentity)User.Identity;
				var claim = userIdentity.FindFirst(ClaimTypes.NameIdentifier);
				var userId = claim.Value;

                unitOfWork.Complete();

                var count = unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().GetAll(x => x.ApplicationUserId == userId).Count();
                HttpContext.Session.SetInt32(SD.SessionKey, count);

                var carts = unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().GetAll();
                foreach (var c in carts)
                {
                    if (c.ApplicationUserId == userId)
                    {
                        unitOfWork.Complete();
                        return RedirectToAction("Index", controllerName: "Home");
                    }
                }
            }
            else 
			    unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().Update(cart);




			unitOfWork.Complete();
			return RedirectToAction("Index");
		}

        public IActionResult Remove (int? cartId)
        {
            var userIdentity = (ClaimsIdentity)User.Identity;
            var claim = userIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            if (cartId is null)
                return NotFound();
            var cart = unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().GetFirstOrDefault(x => x.Id == cartId);
            unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().Delete(cart);
            unitOfWork.Complete();

            var count = unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().GetAll(x => x.ApplicationUserId == userId).Count() ;
            HttpContext.Session.SetInt32(SD.SessionKey, count);




            var carts = unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().GetAll();
            foreach (var c in carts)
            {
                if (c.ApplicationUserId == userId)
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Home");


        }

	}
}
