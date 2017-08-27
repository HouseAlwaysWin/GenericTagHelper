using GenericTagHelperExample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GenericTagHelperExample.Data
{
    public class GenericDbContext : DbContext
    {
        public DbSet<FormModel> FormModels { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public GenericDbContext(DbContextOptions<GenericDbContext> options)
            : base(options)
        {
        }
        public GenericDbContext()
        {
        }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            //Called by parameterless ctor Usually Migrations
            var environmentName = Environment.GetEnvironmentVariable("Development") ?? "";

            optionsBuilder.UseSqlServer(
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: false)
                    .Build()
                    .GetConnectionString("Default")
            );
        }
    }
}

