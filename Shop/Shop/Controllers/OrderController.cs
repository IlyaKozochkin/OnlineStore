using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using Shop.Data;

namespace Shop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        // корзина
        [HttpGet]
        public IActionResult Index()
        {
            List<OrderItem> orderItems;
            using (AppDbContext db = new AppDbContext())
            {
                var prods = db.Products.ToList();
                orderItems = db.OrderItems.Where(p => p.UserName == User.Identity.Name && p.Status == "Cart").ToList();
            }
            if (!orderItems.Any())
            {
                return View("CartAny");
            }
            else
            {
                return View(orderItems);
            }
        }

        // оформление заказа
        [HttpGet]
        public IActionResult CreateOrder()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            string firstName;
            string lastName;
            using (UsersDbContext db = new UsersDbContext())
            {
                ApplicationUser user = db.ApplicationUsers.First(p => p.UserName == User.Identity.Name);
                firstName = user.FirstName;
                lastName = user.LastName;
            }

            using (AppDbContext db = new AppDbContext())
            {
                ulong sum = 0;
                List<Product> products = db.Products.ToList();
                List<OrderItem> orderItems = db.OrderItems.Where(p => p.UserName == User.Identity.Name && p.Status == "Cart").ToList();
                foreach (var item in orderItems)
                {
                    sum += item.Product.Price;
                }
                order.dateTime = DateTime.Now;
                order.FirstName = firstName;
                order.LastName = lastName;
                order.TotalSum = sum;
                order.OrderItems = orderItems;
                order.Status = "NewOrder";
                foreach (var item in orderItems)
                {
                    item.Status = "Order";
                }
                db.Orders.Add(order);
                await db.SaveChangesAsync();
            }

            return View("OrderCreated");
        }

        // удаление объекта из корзины
        public IActionResult DeleteItemCarts(int id)
        {
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    OrderItem orderItem = db.OrderItems.First(p => p.Id == id);
                    db.OrderItems.Remove(orderItem);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {

            }

            return RedirectToAction("Index");
        }
    }
}