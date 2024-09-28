using AutoMapper;
using myshop.Entities.Models;
using myshop.Entities.ViewModels;

namespace myshop.Web.Mapper
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductViewModel>().ReverseMap();    
        }
    }
}
