using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using myshop.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.Entities.ViewModels
{
	public class ShoppingCartViewModel
	{
        public IEnumerable<ShoppingCart> cartList { get; set; }
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }
        public decimal Total { get; set; }
    }
}
