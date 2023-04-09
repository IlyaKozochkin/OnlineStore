using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Areas.Identity.Data;
using Shop.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<UsersDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

//using (AppDbContext db = new AppDbContext())
//{
//    var cat = db.Categories.First(p => p.Id == 1);

//    Product product = new Product()
//    {
//        Name = "нуууууу",
//        Price = 17999,
//        Description = "Модель такая-та, цвет такой-та и тд",
//        Quantity = 2,
//        Category = cat,
//        Image = "https://www.sneakerstyle.fr/wp-content/uploads/2022/06/nike-sb-dunk-low-phillies-DQ4040-400-01.webp"
//    };
//    db.Products.Add(product);
//    db.SaveChanges();
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

