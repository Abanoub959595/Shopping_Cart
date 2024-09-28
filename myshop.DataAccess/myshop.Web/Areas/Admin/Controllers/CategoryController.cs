using Microsoft.AspNetCore.Mvc;
using myshop.DataAccess.Data;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using X.PagedList;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> logger;
        private readonly IUnitOfWork unitOfWork;

        public CategoryController(AppDBContext context,
            ILogger<CategoryController> logger,
            IUnitOfWork unitOfWork)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index(int? page)
        {
            int pageNumber = page?? 1;
            int pageSize = 5;

            var categories = unitOfWork.Repository<Category>().GetAll();
            return View(categories.ToPagedList(pageNumber, pageSize));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    var key = state.Key;
                    var errors = state.Value.Errors;
                    foreach (var error in errors)
                        logger.LogError($"validation error on {key} : {error.ErrorMessage}");
                }
                return View(model);
            }

            if (model == null)
                return NotFound();

            unitOfWork.Repository<Category>().Add(model);
            unitOfWork.Complete();
            TempData["Create"] = "Category Has Created Successfully";
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Edit(int? id)
        {
            if (id is null || id == 0)
                return NotFound();
            var category = unitOfWork.Repository<Category>().GetFirstOrDefault(x => x.Id == id, null);
            if (category == null)
                return BadRequest();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    var key = state.Key;
                    var errors = state.Value.Errors;
                    foreach (var error in errors)
                        logger.LogError($"Validation Error On {key} : {error.ErrorMessage}");
                }
                return View(category);
            }
            unitOfWork.Repository<Category>().Update(category);
            unitOfWork.Complete();
            TempData["Edit"] = "Category Has Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id is null || id == 0)
                return NotFound();

            var category = unitOfWork.Repository<Category>().GetFirstOrDefault(x => x.Id == id, null);
            if (category is null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Category model)
        {

            unitOfWork.Repository<Category>().Delete(model);
            unitOfWork.Complete();
            TempData["Delete"] = "Category Has Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }



    }
}
