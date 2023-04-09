using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Shop.Models
{
	public class Order
	{
		public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
		public string PhoneNumber { get; set; }
		public string Adress { get; set; }
        public string Status { get; set; }
        public DateTime dateTime { get; set; }
        public ulong TotalSum { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
    }
}

