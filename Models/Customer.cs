﻿using System.ComponentModel.DataAnnotations;

namespace PumpPalace.Models
{
    public enum UserRole
    {
        Customer,
        Admin,
    }
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public bool IsSubscribedToNewsletter { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}