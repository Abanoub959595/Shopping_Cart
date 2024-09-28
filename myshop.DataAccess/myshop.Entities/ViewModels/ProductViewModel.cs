using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using myshop.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.Entities.ViewModels
{
    public class ProductViewModel
    {
        //public Product Product { get; set; }
        //// to use SelectListItem => you should install ViewFeatures
        //public IEnumerable<SelectListItem>? CategoryList { get; set; }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Display(Name = "Product Image")]
        public string? Img { get; set; }
        public IFormFile? Url { get; set; }
        [Required(ErrorMessage = "This Record is Required")]
        public decimal Price { get; set; }
        [Display(Name = "Product Category")]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category? category { get; set; }

    }
}
