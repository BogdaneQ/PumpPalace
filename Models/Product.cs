﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PumpPalace.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? DiscountPrice { get; set; }
        [Range(0, 1)]
        public decimal VAT {  get; set; }
        [Required]
        public string Currency { get; set; }
        public string PictureUrl { get; set; }
        [Range(0, int.MaxValue)]
        public string InStock { get; set; }
        [Required]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public bool IsNew { get; set; }
        public bool IsPromotion { get; set; }
    }
}