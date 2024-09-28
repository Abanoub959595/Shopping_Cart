using AutoMapper;
using myshop.Entities.Models;
using myshop.Entities.ViewModels;

namespace myshop.Web.Mapper
{
	public class ShoppingCartProfile : Profile
	{
        public ShoppingCartProfile()
        {
            CreateMap<myshop.Entities.Models.ShoppingCart, ShoppingCartViewModel>().ReverseMap();  
        }
    }
}
