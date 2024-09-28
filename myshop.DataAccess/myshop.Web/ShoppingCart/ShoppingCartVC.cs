using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Repositories;
using myshop.Utilities;
using System.Security.Claims;


namespace myshop.Web.ShoppingCart
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync ()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier );

            if (claim !=  null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionKey)!= null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionKey));
                }else
                {
                    var count = unitOfWork.Repository<myshop.Entities.Models.ShoppingCart>().GetAll(x => x.ApplicationUserId == claim.Value).Count();
                    HttpContext.Session.SetInt32(SD.SessionKey, count);

                    return View(HttpContext.Session.GetInt32(SD.SessionKey));
                }
            }else
            {
                HttpContext.Session.Clear();
                return View(0);
            }

        }


    }
}
