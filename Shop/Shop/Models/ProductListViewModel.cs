using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shop.Models
{
	public class ProductListViewModel
	{
		public List<Product> Products { get; set; }
		public SelectList Categories = new SelectList(new List<Category>(), "Id", "Name");
		public string? Name { get; set; }
	}
}

