using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{

    [Area("customer")]
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
        }


        public IActionResult Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),

            };
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderTotal += (cart.Price * cart.Count);
            }


            return View(ShoppingCartVM);
        }

        public IActionResult Plus(int cartId) {
            var cartFromDB = _unitOfWork.ShoppingCart.Get(u=>u.Id == cartId);
            cartFromDB.Count++;
            _unitOfWork.ShoppingCart.Update(cartFromDB);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        
        }
        public IActionResult Remove(int cartId)
        {
            var cartFromDB = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cartFromDB);
            _unitOfWork.Save();
            return RedirectToAction("Index");

        }
        public IActionResult Minus(int cartId)
        {
            var cartFromDB = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cartFromDB.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDB);
            }
            else {
                cartFromDB.Count--;
            }
           
            _unitOfWork.ShoppingCart.Update(cartFromDB);
            _unitOfWork.Save();
            return RedirectToAction("Index");

        }
        public IActionResult Summary() { 
            return View();
        
        }

        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }

   
        }
    }

}
