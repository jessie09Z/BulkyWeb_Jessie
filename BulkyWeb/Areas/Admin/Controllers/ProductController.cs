using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "category").ToList();

            return View(objProductList);
        }

        public IActionResult UpSert(int? id)
        {

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category
              .GetAll().Select(u => new SelectListItem
              {
                  Text = u.Name,
                  Value = u.Id.ToString()

              }),
                Product = new Product()
            };

            if (id == null || id == 0) { return View(productVM); }
            else
            {
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }

        }

        [HttpPost]

        public IActionResult UpSert(ProductVM productVM, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImgURL))
                    {
                        //delete the prev img
                        var preImgPath = Path.Combine(wwwRootPath, productVM.Product.ImgURL.TrimStart('\\'));
                        if (System.IO.File.Exists(preImgPath))
                        {
                            System.IO.File.Delete(preImgPath);
                        }

                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {

                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImgURL = @"\images\product\" + fileName;
                }
                if (productVM.Product.Id == 0) { _unitOfWork.Product.Add(productVM.Product); }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();
                TempData["success"] = "Product created successful";
                return RedirectToAction("Index");

            }
            else
            {

                productVM.CategoryList = _unitOfWork.Category
          .GetAll().Select(u => new SelectListItem
          {
              Text = u.Name,
              Value = u.Id.ToString()

          });
                return View(productVM);
            }


        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
            if (productFromDb == null)
            {
                return NotFound();

            }
            return View(productFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Product obj)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product updated successful";
                return RedirectToAction("Index");

            }
            return View(obj);

        }



        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "category").ToList();
            return Json(objProductList);

        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u=>u.Id == id);
            if (productToBeDeleted == null) {
                return Json(new { success = false, message="Deleting Error" }); 
            }
            var preImgPath= Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImgURL.TrimStart('\\'));
            if (System.IO.File.Exists(preImgPath))
            {
                System.IO.File.Delete(preImgPath);
            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleting Successfully" });
        }
          

        
    }
}
