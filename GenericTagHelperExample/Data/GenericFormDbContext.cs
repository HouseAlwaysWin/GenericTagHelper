using GenericTagHelperExample.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericTagHelperExample.Data
{
    public class GenericFormDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public GenericFormDbContext(DbContextOptions<GenericFormDbContext> options)
            : base(options)
        {
        }
    }
}

