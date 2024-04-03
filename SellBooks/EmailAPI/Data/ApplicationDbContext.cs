using EmailAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EmailAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Email> emails { get; set; }
    }
}
