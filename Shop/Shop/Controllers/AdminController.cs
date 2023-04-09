using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging.Signing;
using Shop.Models;

namespace Shop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        //заказы
        public IActionResult Orders()
        {
            return View();
        }

        // список новых заказов
        public IActionResult ListNewOrders()
        {
            List<Order> orders;
            using (AppDbContext db = new AppDbContext())
            {
                List<Product> products = db.Products.ToList();
                List<OrderItem> orderItems = db.OrderItems.ToList();
                orders = db.Orders.Where(p => p.Status == "NewOrder").ToList();
            }

            return View(orders);
        }

        // список всех подтвержденных заказов
        public IActionResult ListConfirmOrders()
        {
            List<Order> orders;
            using (AppDbContext db = new AppDbContext())
            {
                List<Product> products = db.Products.ToList();
                List<OrderItem> orderItems = db.OrderItems.ToList();
                orders = db.Orders.Where(p => p.Status == "Confirm").ToList();
            }

            return View(orders);
        }

        // информация о заказе
        public IActionResult InfoOrder(int? id)
        {
            List<OrderItem> orderItems;
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    var prods = db.Products.ToList();
                    var allOrderItems = db.OrderItems.ToList();
                    orderItems = db.Orders.First(p => p.Id == id).OrderItems.ToList();
                }
            }
            catch (Exception)
            {
                using (AppDbContext db = new AppDbContext())
                {
                    Order order = db.Orders.First(p => p.Id == id);
                    db.Orders.Remove(order);
                    db.SaveChanges();
                }

                return RedirectToAction("ListNewOrders");
            }
            return View(orderItems);
        }

        // подтверждение заказа админом
        public IActionResult ConfirmOrder(int? id)
        {
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    List<OrderItem> orderItems = db.OrderItems.ToList();
                    Order order = db.Orders.First(p => p.Id == id);
                    order.Status = "Confirm";
                    db.Orders.Update(order);
                    db.SaveChanges();
                }
            }
            catch (Exception) { }

            return RedirectToAction("ListNewOrders");
        }

        // заказ отдан
        public IActionResult CompletedOrder(int? id)
        {
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    //List<Product> products = db.Products.ToList();
                    List<OrderItem> allOrderItems = db.OrderItems.ToList();
                    Order order = db.Orders.First(p => p.Id == id);
                    List<OrderItem> orderItems = db.Orders.First(p => p.Id == id).OrderItems.ToList();
                    order.Status = "Completed";
                    foreach (var item in orderItems)
                    {
                        item.Status = "Completed";
                        db.OrderItems.Update(item);
                    }
                    db.Orders.Update(order);
                    db.SaveChanges();
                }
                return RedirectToAction("ListConfirmOrders");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // отмена заказа
        public IActionResult DeleteOrder(int? id)
        {
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    Order order = db.Orders.First(p => p.Id == id);
                    List<OrderItem> allOrderItems = db.OrderItems.ToList();
                    List<OrderItem> orderItems = db.Orders.First(p => p.Id == id).OrderItems.ToList();

                    db.Orders.Remove(order);
                    foreach (var item in orderItems)
                    {
                        db.OrderItems.Remove(item);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception) { }

            return RedirectToAction("ListNewOrders");
        }

        // удаление определенного товара из заказа
        public IActionResult DeleteOrderItems(int? id)
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
            catch (Exception) { }

            return RedirectToAction("ListNewOrders");
        }

        ////////////////////////////////////////////////////////////////////////


        // список все товаров
        public IActionResult Products()
        {
            List<Product> products;
            using (AppDbContext db = new AppDbContext())
            {
                var categories = db.Categories.ToList();
                products = db.Products.ToList();
            }
            return View(products);
        }

        // Добавление товара в каталог
        [HttpGet]
        public IActionResult AddProduct()      
        {
            using (AppDbContext db = new AppDbContext())
            {
                var categories = db.Categories.ToList();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            try
            {   
                using (AppDbContext db = new AppDbContext())
                {
                    var category = db.Categories.First(p => p.Id == int.Parse(Request.Form["categoryId"]));
                    product.Category = category;
                    db.Products.Add(product);
                    await db.SaveChangesAsync();
                }
                return View("Index");
            }
            catch (Exception)
            {
                return View("Index");
            }
        }

        // редактирование товара из каталога
        [HttpGet]
        public IActionResult EditProduct(int? Id)
        {
            Product product;
            using (AppDbContext db = new AppDbContext())
            {
                var categories = db.Categories.ToList();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                product = db.Products.First(p => p.Id == Id);
            }
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var category = db.Categories.First(p => p.Id == int.Parse(Request.Form["categoryId"]));
                product.Category = category;
                db.Products.Update(product);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Products");
        }

        // удаление товара из каталога
        public IActionResult DeleteProduct(int? id)
        {
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    Product product = db.Products.First(p => p.Id == id);
                    db.Products.Remove(product);
                    db.SaveChanges();
                }
            }
            catch (Exception){ }
            
            return RedirectToAction("Products");
        }


        ////////////////////////////////////////////////////////////////////////


        // список всех категорий
        public IActionResult Categories()
        {
            List<Category> categories;
            using (AppDbContext db = new AppDbContext())
            {
                categories = db.Categories.ToList();
            }
            return View(categories);
        }

        // добавить категорию
        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    db.Categories.Add(category);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception){ }

            return RedirectToAction("Categories");
        }

        // редактировать название категории
        [HttpGet]
        public IActionResult EditCategory(int? id)
        {
            Category category;
            using (AppDbContext db = new AppDbContext())
            {
                category = db.Categories.First(p => p.Id == id);
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category)
        {
            using (AppDbContext db = new AppDbContext())
            {
                db.Categories.Update(category);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Categories");
        }

        public IActionResult DeleteCategory(int? id)
        {
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    Category category = db.Categories.First(p => p.Id == id);
                    db.Categories.Remove(category);
                    db.SaveChanges();
                }
            }
            catch (Exception) { }
            return RedirectToAction("Categories");
        }

        ////////////////////////////////////////////////////////////////////////


        // Все созданные роли
        public IActionResult Roles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        // Создание новой роли
        [HttpGet]
        public IActionResult AddRole() => View();
        [HttpPost]
        public IActionResult AddRole(IdentityRole model)
        {
            if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }
    }
}