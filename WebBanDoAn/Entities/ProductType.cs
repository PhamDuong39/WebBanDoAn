﻿using System.ComponentModel.DataAnnotations;

namespace WebBanDoAn.Entities
{
    public class ProductType
    {
        [Key]
        public int Id { get; set; }

        public string? NameProductType { get; set; }

        public string? ImageTypeProduct { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdateAt { get; set; }
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}
