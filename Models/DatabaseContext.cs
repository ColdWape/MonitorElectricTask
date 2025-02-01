using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TestTask.Models
{
    public class DatabaseContext : DbContext
    {
        //инициализация бд
        public DbSet<Contact> Contacts { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
