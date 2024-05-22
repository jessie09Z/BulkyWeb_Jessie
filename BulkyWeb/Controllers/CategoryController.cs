using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepo;
            public CategoryController(ICategoryRepository db)
            {
            _categoryRepo = db;
            }
            public IActionResult Index()
            {


                List<Category> objCategoryList = _categoryRepo.GetAll().ToList();
                return View(objCategoryList);
            }

            public IActionResult Create()
            {
                return View();
            }

            [HttpPost]

            public IActionResult Create(Category obj)
            {
                if (obj.Name.ToLower() == obj.DisplayOrder.ToString())
                {
                    ModelState.AddModelError("name", "The Display Order cannot 100% match the Name");
                }
                if (obj.Name != null && obj.Name.ToLower() == "test")
                {
                    ModelState.AddModelError("", "Test is an invalid value");
                }
                if (ModelState.IsValid)
                {
                _categoryRepo.Add(obj);
                _categoryRepo.Save();
                    TempData["success"] = "Category created successful";
                    return RedirectToAction("Index");

                }
                return View();

            }

            public IActionResult Edit(int? id)
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }
                Category catergoryFromDb = _categoryRepo.Get(u=>u.Id==id);
                if (catergoryFromDb == null)
                {
                    return NotFound();

                }
                return View(catergoryFromDb);
            }

            [HttpPost]
            public IActionResult Edit(Category obj)
            {

                if (ModelState.IsValid)
                {
                _categoryRepo.Update(obj);
                _categoryRepo.Save();
                TempData["success"] = "Category updated successful";
                    return RedirectToAction("Index");

                }
                return View(obj);

            }


            public IActionResult Delete(int? id)
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }
                Category catergoryFromDb = _categoryRepo.Get(u => u.Id == id);
            if (catergoryFromDb == null)
                {
                    return NotFound();

                }
                return View(catergoryFromDb);
            }

            [HttpPost, ActionName("Delete")]
            public IActionResult DeletePost(int? id)
            {
                Category catergoryFromDb = _categoryRepo.Get(u => u.Id == id);
            if (catergoryFromDb == null)
                {
                    return NotFound();

                }
            _categoryRepo.Remove(catergoryFromDb);
            _categoryRepo.Save();
                TempData["success"] = "Category deleted successful";
                return RedirectToAction("Index");

            }

        
    }
}
