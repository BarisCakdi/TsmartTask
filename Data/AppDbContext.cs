using Microsoft.EntityFrameworkCore;
using System;
using TsmartTask.Model;

namespace TsmartTask.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
}
