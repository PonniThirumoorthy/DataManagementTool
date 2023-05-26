using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using webapi.Models;

namespace webapi.EF
{
    public class CustomerContext
       : DbContext
    {
        public CustomerContext(DbContextOptions options)
        : base(options)
        {

        }

        public DbSet<Details> Details { get; set; }
        public DbSet<RegisterModel> UserLogin { get; set; }
        public DbSet<ValidationModel> Validation { get; set; }
    }
}
