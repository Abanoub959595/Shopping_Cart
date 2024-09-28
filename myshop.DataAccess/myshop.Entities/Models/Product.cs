using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace myshop.Entities.Models
{
	public class Product
	{
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
		[Display(Name="Product Image")]
        public string? Img { get; set; }
		[Required(ErrorMessage ="This Record is Required")]
        public decimal Price { get; set; }
		[Display(Name ="Product Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
