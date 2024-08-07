

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
using Stripe;

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

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {

            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderVM.OrderHeader.Id);
            orderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = orderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order Shipped Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {

            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderVM.OrderHeader.Id);

            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["Success"] = "Order Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });

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
