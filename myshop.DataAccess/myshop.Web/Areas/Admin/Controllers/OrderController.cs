using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.DataAccess.Implementation;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Entities.ViewModels;
using myshop.Utilities;
using Stripe;
using Stripe.Climate;
using X.PagedList;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        [BindProperty]
        public OrderViewModel orderViewModel { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 8;


            IEnumerable<OrderHeader> orderHeaders = unitOfWork.Repository<OrderHeader>().GetAll(includeWord: "ApplicationUser");

            return View(orderHeaders.ToPagedList(pageNumber, pageSize));
        }

        public IActionResult Details(int orderid)
        {
            OrderViewModel orderViewModel = new OrderViewModel()
            {
                orderHeader = unitOfWork.Repository<OrderHeader>().GetFirstOrDefault(x => x.Id == orderid, includeWord: "ApplicationUser"),
                orderDetails = unitOfWork.Repository<OrderDetail>().GetAll(x => x.OrderHeaderId == orderid, includeWord: "Product")
            };
            return View(orderViewModel);
        }

        [HttpPost]
        //[ActionName("Details")]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderHeaderDetails ()
        {
            var orderFromDb = unitOfWork.Repository<OrderHeader>().GetFirstOrDefault(x => x.Id == orderViewModel.orderHeader.Id);
            orderFromDb.Name = orderViewModel.orderHeader.Name;
            orderFromDb.Address = orderViewModel.orderHeader.Address;
			orderFromDb.Phone = orderViewModel.orderHeader.Phone;
			orderFromDb.City = orderViewModel.orderHeader.City;

            if (orderViewModel.orderHeader.Carrier != null) 
                orderFromDb.Carrier = orderViewModel.orderHeader.Carrier;
            if (orderViewModel.orderHeader.TrackingNumber != null)
                orderFromDb.TrackingNumber = orderViewModel.orderHeader.TrackingNumber;

            unitOfWork.Repository<OrderHeader>().Update(orderFromDb);
            unitOfWork.Complete();

			TempData["Edit"] = "Order Has Update Successfully";
			return RedirectToAction("Details", "Order", new { orderid = orderFromDb.Id});

		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcess (int orderid)
        {
            var order = unitOfWork.Repository<OrderHeader>().GetFirstOrDefault(x => x.Id == orderid);
            order.OrderStatus = SD.Proccessing;
            unitOfWork.Repository<OrderHeader>().Update(order);
            unitOfWork.Complete();
			TempData["Edit"] = "Order Status Has Update Successfully";
			return RedirectToAction("Details", "Order", new { orderid = order.Id });

		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartShipp(int orderid)
		{
            // in start ship process we must identify tracking number and carrier 
			var order = unitOfWork.Repository<OrderHeader>().GetFirstOrDefault(x => x.Id == orderid);
            order.TrackingNumber = orderViewModel.orderHeader.TrackingNumber;
            order.Carrier = orderViewModel.orderHeader.Carrier;
			order.OrderStatus = SD.Shipped;
            order.ShippingDate = DateTime.Now;
            unitOfWork.Repository<OrderHeader>().Update(order);

			unitOfWork.Complete();
			TempData["Edit"] = "Order Has Shipped Successfully";
            return RedirectToAction("Details", "Order", new { orderid = order.Id });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder(int orderid)
		{
			var order = unitOfWork.Repository<OrderHeader>().GetFirstOrDefault(x => x.Id == orderid);

            if (order.PaymentStatus == SD.Approve)
            {
                var options = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = order.PaymentIntentId,

                };
                var services = new RefundService();
                Refund refund = services.Create(options);

            }
            order.OrderStatus = SD.Cancelled;
            order.PaymentStatus = SD.Refund;
            unitOfWork.Repository<OrderHeader>().Update(order);
            unitOfWork.Complete();


			TempData["Edit"] = "Order Has Cancelled Successfully";
			return RedirectToAction("Details", "Order", new { orderid = order.Id });

		}






	}

    
}
