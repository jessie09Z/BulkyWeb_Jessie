using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
            public CategoryController(ApplicationDbContext db)
            {
                _db = db;
            }
            public IActionResult Index()
            {


                List<Category> objCategoryList = _db.Categories.ToList();
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
                    _db.Categories.Add(obj);
                    _db.SaveChanges();
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
                Category catergoryFromDb = _db.Categories.Find(id);
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
                    _db.Categories.Update(obj);
                    _db.SaveChanges();
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
                Category catergoryFromDb = _db.Categories.Find(id);
                if (catergoryFromDb == null)
                {
                    return NotFound();

                }
                return View(catergoryFromDb);
            }

            [HttpPost, ActionName("Delete")]
            public IActionResult DeletePost(int? id)
            {
                Category catergoryFromDb = _db.Categories.Find(id);
                if (catergoryFromDb == null)
                {
                    return NotFound();

                }
                _db.Categories.Remove(catergoryFromDb);
                _db.SaveChanges();
                TempData["success"] = "Category deleted successful";
                return RedirectToAction("Index");

            }

        
    }
}
