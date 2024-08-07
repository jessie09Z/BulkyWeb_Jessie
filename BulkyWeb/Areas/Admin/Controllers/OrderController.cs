﻿

using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
           
     public OrderController(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
            OrderVM vm = new OrderVM();
            return View();
        }
        public IActionResult Details(int orderId)
        {
            OrderVM orderVM = new()
            {

            OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties:"ApplicationUser"),
            OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId== orderId, includeProperties:"Product")
            };
            return View(orderVM);
        }





        [HttpGet]
        public IActionResult GetAll(string status) { 
        {
            IEnumerable<OrderHeader> objOderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
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