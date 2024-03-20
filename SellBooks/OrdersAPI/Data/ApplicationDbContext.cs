using OrdersAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;

namespace OrdersAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<OrderHeader> orderHeaders { get; set; }
        public DbSet<OrderDetails> orderDetails { get; set; }
        public DbSet<OrderHead> orderHeads { get; set; }
        public DbSet<OrderDetail> orderDetail { get; set; }
    }
}
