using Microsoft.EntityFrameworkCore;
using BusuMatchProject.Models;

namespace BusuMatchProject.Data
{
    // This is the application's database context, used by Entity Framework Core
    public class AppDbContext : DbContext
    {
        // Constructor that passes DbContextOptions to the base class
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet representing the Users table
        public DbSet<User> Users => Set<User>();

        // DbSet representing the Expenses table
        public DbSet<Expense> Expenses { get; set; }
    }
}
