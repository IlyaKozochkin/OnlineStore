using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using Shop.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Shop.Controllers;

public class HomeController : Controller
{
    public IActionResult Index(int? category, string? name)
    {
        using (AppDbContext db = new AppDbContext())
        {
            List<Product> products = db.Products.Include(p => p.Category).ToList();
            if (category != null && category != 0)
            {
                products = products.Where(p => p.Category.Id == category).ToList();
            }
            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(p => p.Name!.ToLower().Contains(name.ToLower())).ToList();
            }

            List<Category> categories = db.Categories.ToList();
            categories.Insert(0, new Category { Name = "Все", Id = 0 });

            ProductListViewModel viewModel = new ProductListViewModel
            {
                Products = products.ToList(),
                Categories = new SelectList(categories, "Id", "Name", category),
                Name = name
            };

            return View(viewModel);
        }
    }

    public IActionResult Info(int? Id)
    {
        using (AppDbContext db = new AppDbContext())
        {
            var categories = db.Categories.ToList();
            Product product = db.Products.First(p => p.Id == Id);
            return View(product);
        }
    }

    [Authorize]
    public IActionResult AddOrderItem(int id)
    {
        try
        {
            using (AppDbContext db = new AppDbContext())
            {
                var categories = db.Categories.ToList();
                Product product = db.Products.First(p => p.Id == id);
                OrderItem orderItem = new OrderItem() { Product = product, Quantity = 1, Status = "Cart", UserName = User.Identity.Name };
                db.OrderItems.Add(orderItem);
                db.SaveChanges();
            }
        }
        catch (Exception)
        {
            return RedirectToAction("Index");
        }
        
        return RedirectToAction("Index", "Order");
    }




    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

