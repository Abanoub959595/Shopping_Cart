using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Entities.ViewModels;
using myshop.Web.Helper;
using X.PagedList;

namespace myshop.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : Controller
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly ILogger<ProductController> logger;
        private readonly IMapper mapper;

        public ProductController(IUnitOfWork unitOfWork,
			ILogger<ProductController> logger,
			IMapper mapper)
        {
			this.unitOfWork = unitOfWork;
			this.logger = logger;
            this.mapper = mapper;
        }
        public IActionResult Index(int? page)
		{
			int pageNumber = page ?? 1;
			int pageSize = 8;

			var products = unitOfWork.Repository<Product>().GetAll(null, "Category");

			var productsViewModel = mapper.Map<IEnumerable<ProductViewModel>>(products);			

			return View(productsViewModel.ToPagedList(pageNumber, pageSize));
		}

		public IActionResult Create ()
		{
			ViewBag.Categories = unitOfWork.Repository<Category>().GetAll();
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create (ProductViewModel productViewModel)
		{
			if (!ModelState.IsValid)
			{
				foreach(var state in ModelState)
				{
					var key = state.Key;
					var errors = state.Value.Errors;
					foreach (var error in errors)
						logger.LogError($"Validation Error On Key {key} : {error.ErrorMessage}");
				}
				return View(productViewModel);
			}

			var product = mapper.Map<Product>(productViewModel);

			if (productViewModel.Url is not null)
				product.Img = DocumentSettings.UploadFile(productViewModel.Url, "Product");

			unitOfWork.Repository<Product>().Add(product);
			unitOfWork.Complete();
            TempData["Create"] = "Product Has Created Successfully";
            return RedirectToAction("Index");
		}

		//public IActionResult Create ()
		//{
		//	// ViewBag.Categories = unitOfWork.Repository<Category>().GetAll();
		//	ProductViewModel productVM = new ProductViewModel()
		//	{
		//		Product = new Product(),
		//		CategoryList = unitOfWork.Repository<Category>().GetAll().Select(x => new SelectListItem()
		//		{
		//			Text = x.Name,
		//			Value = x.Id.ToString()
		//		})
		//	};
		//	return View(productVM);
		//}
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public IActionResult Create (Product product, IFormFile? file)
		//{
		//	if (!ModelState.IsValid)
		//	{
		//		foreach(var state in ModelState)
		//		{
		//			var stateKey= state.Key;
		//			var errors = state.Value.Errors;
		//			foreach (var error in errors)
		//				logger.LogError($"validation error on {stateKey} : {error.ErrorMessage}");
		//		}	
		//		return View(product);
		//	}
		//	if (file is not null)
		//		product.Img = DocumentSettings.UploadFile(file, "Product");

		//	unitOfWork.Repository<Product>().Add(product);
		//	unitOfWork.Complete();
		//	TempData["Create"] = "Product Has Created Successfully";
		//	return RedirectToAction(nameof(Index));
		//}

		public IActionResult Edit (int? id)
		{
			if (id is null)
				return NotFound();
			var product = unitOfWork.Repository<Product>().GetFirstOrDefault(x => x.Id == id);
			var productVM = mapper.Map<ProductViewModel>(product);
            ViewBag.Categories = unitOfWork.Repository<Category>().GetAll();
            return View(productVM);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit (ProductViewModel productViewModel)
		{
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    var key = state.Key;
                    var errors = state.Value.Errors;
                    foreach (var error in errors)
                        logger.LogError($"Validation Error On Key {key} : {error.ErrorMessage}");
                }
                return View(productViewModel);
            }

			if (productViewModel.Url is not null)
				productViewModel.Img = DocumentSettings.EditFile(productViewModel.Url, productViewModel.Img, "Product");

			var product = mapper.Map<Product>(productViewModel);


			unitOfWork.Repository<Product>().Update(product);
			unitOfWork.Complete();
			TempData["Edit"] = "product Has been updated successfully!";
			return RedirectToAction(nameof(Index));

        }




		//public IActionResult Edit(int? id)
		//{
		//	if (id == null)
		//		return NotFound();
		//	var product = unitOfWork.Repository<Product>().GetFirstOrDefault(x => x.Id == id);
		//	if (product == null)	
		//		return NotFound();

		//	var categories = unitOfWork.Repository<Category>().GetAll();	

		//	return View(new ProductViewModel ()
		//	{
		//		Product = product,
		//		CategoryList = new SelectList(categories, "Id", "Name")
		//	});
		//}

		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public IActionResult Edit (ProductViewModel productVM, IFormFile file)
		//{
		//	if (!ModelState.IsValid)
		//	{
		//		foreach(var state in ModelState)
		//		{
		//			var key = state.Key;	
		//			var errors = state.Value.Errors;
		//			foreach (var error in errors)
		//				logger.LogError($"Validation Error On {key} : {error.ErrorMessage}");
		//		}
		//		return View(productVM);
		//	}
		//	var model = unitOfWork.Repository<Product>().GetFirstOrDefault(x => x.Id == productVM.Product.Id);
		//	var imgPath = DocumentSettings.EditFile(file, model.Img, "Product");

		//	if (imgPath != null)
		//		model.Img = imgPath;

		//	model.Name = productVM.Product.Name;
		//	model.Description = productVM.Product.Description;
		//	model.Price = productVM.Product.Price;
		//	model.CategoryId = productVM.Product.CategoryId;
		//	unitOfWork.Repository<Product>().Update(model);
		//	unitOfWork.Complete();
		//	TempData["Edit"] = "Product Has Updated Successfully!";
		//	return RedirectToAction(nameof(Index));
		//}

		//public IActionResult Delete (int? id)
		//{
  //          if (id == null)
  //              return NotFound();
  //          var product = unitOfWork.Repository<Product>().GetFirstOrDefault(x => x.Id == id);
  //          if (product == null)
  //              return NotFound();
		//	return View(product);
  //      }

		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public IActionResult Delete (Product model)
		//{

		//	DocumentSettings.DeleteFile(model.Img);
		//	unitOfWork.Repository<Product>().Delete(model);
		//	unitOfWork.Complete();
		//	TempData["Delete"] = "Product Has Deleted Successfully!";
		//	return RedirectToAction(nameof(Index));
		//}


		public IActionResult Delete (int? id)
		{
			if (id is null) 
				return NotFound();
			var product = unitOfWork.Repository<Product>().GetFirstOrDefault(x => x.Id == id, "Category");
			if (product is null)
				return BadRequest();

			var productVM = mapper.Map<ProductViewModel>(product);
			return View(productVM);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Delete(ProductViewModel model)
		{
			if (!ModelState.IsValid)
			{
				foreach(var state in ModelState)
				{
					var key = state.Key;
					var errors = state.Value.Errors;
					foreach (var error in errors)
						logger.LogError($"Validation Key on {key} : {error.ErrorMessage}");
				}
				return View(model);
			}
			var product = mapper.Map<Product>(model);
			DocumentSettings.DeleteFile(product.Img);
			unitOfWork.Repository<Product>().Delete(product);
			unitOfWork.Complete();
			TempData["Delete"] = "Product Has been deleted successfully!";
			return RedirectToAction(nameof(Index));
		}



    }
}
