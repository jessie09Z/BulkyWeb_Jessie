﻿using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
          
            return View(objProductList);
        }

        public IActionResult Create()
        {
            
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category
              .GetAll().Select(u => new SelectListItem
              {
                  Text = u.Name,
                  Value = u.Id.ToString()

              }),
                Product =new Product()
            };
            return View(productVM);
        }

        [HttpPost]

        public IActionResult Create(ProductVM productVM)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product created successful";
                return RedirectToAction("Index");

            }
            else {

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


        public IActionResult Delete(int? id)
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

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
            if (productFromDb == null)
            {
                return NotFound();

            }
            _unitOfWork.Product.Remove(productFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successful";
            return RedirectToAction("Index");

        }


    }
}