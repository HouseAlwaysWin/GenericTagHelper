using GenericTagHelperExample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenericTagHelperExample.Data.Migrations
{
    public partial class AddSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            using (var context = new GenericDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    var customers = new List<Customer>
                {
                    new Customer { Id=1,Name="Jon Snow" ,Gender="male"},
                    new Customer { Id=2,Name="Sansa Stark" ,Gender="Female"},
                    new Customer { Id=3,Name="Ned Stark",Gender="male" },
                    new Customer { Id=4,Name="Arya Stark",Gender="male" },
                    new Customer { Id=5,Name="Sansa Stark",Gender="Female" },
                    new Customer { Id=6,Name="Bran Stark",Gender="male" },
                    new Customer { Id=7,Name="Benjen Stark",Gender="male" },
                    new Customer { Id=8,Name="Tormund",Gender="male" },
                    new Customer { Id=9,Name="Edmure Tully",Gender="male" },
                    new Customer { Id=10,Name="Hodor",Gender="male" },
                    new Customer { Id=11,Name="Cersei Lannister",Gender="Female" },
                    new Customer { Id=12,Name="Tyrion Lannister" ,Gender="male"},
                    new Customer { Id=13,Name="Jaime Lannister",Gender="male" },
                    new Customer { Id=14,Name="Tywin Lannister",Gender="male" },
                    new Customer { Id=15,Name="Daenerys Targaryen",Gender="Female" },
                    new Customer { Id=16,Name="Rhaegar Targaryen",Gender="male" },
                    new Customer { Id=17,Name="Joffrey Baratheon",Gender="male" },
                    new Customer { Id=18,Name="Myrcella Baratheon",Gender="Female" },
                    new Customer { Id=19,Name="Tommen Baratheon",Gender="male" },
                    new Customer { Id=20,Name="Gregor Clegane",Gender="male" },
                    new Customer { Id=21,Name="Qyburn",Gender="male" },
                    new Customer { Id=22,Name="Catelyn Tully",Gender="male" },
                    new Customer { Id=23,Name="Bronn",Gender="male" },
                    new Customer { Id=24,Name="Vary",Gender="male" },
                    new Customer { Id=25,Name="Yara Greyjoy",Gender="Female" },
                    new Customer { Id=26,Name="Theon Greyjoy",Gender="male" },
                    new Customer { Id=27,Name="Grey Worm",Gender="male" },
                    new Customer { Id=28,Name="Olenna Tyrell",Gender="Female" },
                    new Customer { Id=29,Name="Ramsay Bolton",Gender="male" },
                    new Customer { Id=30,Name="Petyr Baelish",Gender="male" },
                    new Customer { Id=31,Name="Lysa Tully",Gender="Female" },
                    new Customer { Id=32,Name="Robin Arryn",Gender="male" },
                    new Customer { Id=33,Name="Rickon Stark" ,Gender="male"},
                    new Customer { Id=34,Name="Brynden Tully" ,Gender="male"},
                    new Customer { Id=35,Name="Lyanna Mormont" ,Gender="male"},
                    new Customer { Id=36,Name="Jorah Mormont",Gender="male" },
                    new Customer { Id=37,Name="Jeor Mormont",Gender="male" },
                    new Customer { Id=38,Name="Maege Mormont",Gender="male" },
                    new Customer { Id=39,Name="Gilly",Gender="Female" },
                    new Customer { Id=40,Name="Melisandre",Gender="Female" },
                    new Customer { Id=41,Name="Gendry",Gender="male" },
                    new Customer { Id=42,Name="Davos Seaworth",Gender="male" },
                    new Customer { Id=43,Name="Robert Baratheon" ,Gender="male"},
                    new Customer { Id=44,Name="Stannis Baratheon",Gender="male" },
                    new Customer { Id=45,Name="Selyse Baratheon",Gender="male" },
                    new Customer { Id=46,Name="Loras Tyrell",Gender="male" },
                    new Customer { Id=47,Name="Renly Baratheon",Gender="male" },
                    new Customer { Id=48,Name="Margaery Tyrell ",Gender="male" },
                    new Customer { Id=49,Name="Shireen Baratheon",Gender="male" },
                    new Customer { Id=50,Name="Samwell Tarly",Gender="male" },
                };

                    context.Customers.AddRange(customers);
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Customers] ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Customers] OFF");
                    transaction.Commit();
                }

            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

