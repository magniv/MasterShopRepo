using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MasterShop.Models;

namespace MasterShop.Data
{
    public class MasterShopContext : DbContext
    {
        public MasterShopContext (DbContextOptions<MasterShopContext> options)
            : base(options)
        {
        }

        public DbSet<MasterShop.Models.Category> Category { get; set; }

        public DbSet<MasterShop.Models.Product> Product { get; set; }

        public DbSet<MasterShop.Models.Account> Account { get; set; }

        public DbSet<MasterShop.Models.Order> Order { get; set; }
    }
}
