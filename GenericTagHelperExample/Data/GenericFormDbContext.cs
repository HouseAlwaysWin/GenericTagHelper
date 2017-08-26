using GenericTagHelperExample.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericTagHelperExample.Data
{
    public class GenericDbContext : DbContext
    {
        public DbSet<FormModel> FormModels { get; set; }

        public GenericDbContext(DbContextOptions<GenericDbContext> options)
            : base(options)
        {
        }
    }
}

