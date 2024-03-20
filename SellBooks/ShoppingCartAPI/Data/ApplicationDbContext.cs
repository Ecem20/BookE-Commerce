using ShoppingCartAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;

namespace ShoppingCartAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<CartHeader> cartHeaders { get; set; }
        public DbSet<CartDetails> cartDetails { get; set; }
        public DbSet<CartHead> cartHeads { get; set; }
        public DbSet<CartDetail> cartDetail { get; set; }
    }
}
