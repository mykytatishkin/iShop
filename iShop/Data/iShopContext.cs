using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using iShop.Models;

namespace iShop.Data
{
    public class iShopContext : DbContext
    {
        public iShopContext (DbContextOptions<iShopContext> options)
            : base(options)
        {
        }

        public DbSet<iShop.Models.Category> Category { get; set; }
        public DbSet<iShop.Models.Product> Product { get; set; }
        public DbSet<iShop.Models.Picture> Picture { get; set; }
        public DbSet<iShop.Models.Sale> Sale { get; set; }
        public DbSet<iShop.Models.Cart> Cart { get; set; }
    }
}
