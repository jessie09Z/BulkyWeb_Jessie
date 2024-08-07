

using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
    
    [Area("admin")]
    [Authorize]
   
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM orderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
     
            return View();
        }
        public IActionResult Details(int orderId)
        {
            orderVM = new()
            {

            OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties:"ApplicationUser"),
            OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId== orderId, includeProperties:"Product")
            };
            return View(orderVM);
        }

     [HttpPost]
     [Authorize(Roles=SD.Role_Admin+","+ SD.Role_Employee)]
             public IActionResult UpdateOrderDetail(int orderId)
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == orderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = orderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = orderVM.OrderHeader.City;
            orderHeaderFromDb.State = orderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = orderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.Carrier = orderVM.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully.";


            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }


        [HttpGet]
        public IActionResult GetAll(string status) {
            {
                IEnumerable<OrderHeader> objOderHeaders;


                if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
                {
                    objOderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
                }
                else {
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                    objOderHeaders = _unitOfWork.OrderHeader
                        .GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");

                }
                switch (status)
            {
                case "pending":
                   objOderHeaders = objOderHeaders.Where(u=>u.PaymentStatus==SD.PaymentStatusPending);
                    break;
                case "inprocess":
                    objOderHeaders = objOderHeaders.Where(u => u.PaymentStatus == SD.StatusInProcess);
                    break;
                case "approved":
                    objOderHeaders = objOderHeaders.Where(u => u.PaymentStatus == SD.StatusApproved);
                    break;
                case "completed":
                    objOderHeaders = objOderHeaders.Where(u => u.PaymentStatus == SD.StatusShipped);
                    break;
     
                default:
    
                    break;

            }
                return Json(new { data = objOderHeaders });
            }
       
           

        }

    }
}
