using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Shop.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; }          // название
        public ulong Price { get; set; }          // цена
        public string Description { get; set; }   // описание
        public string Image { get; set; }         // изображение
		public Category Category { get; set; }    // категория
    }
}