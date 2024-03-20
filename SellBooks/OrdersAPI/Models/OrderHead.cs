﻿using System.ComponentModel.DataAnnotations;

namespace OrdersAPI.Models
{
    public class OrderHead
    {
        [Key]
        public int OrderHeadId { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public double OrderTotal { get; set; }
        public string? Address { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
        public string Status { get; set; }
    }
}