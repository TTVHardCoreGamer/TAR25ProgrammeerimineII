using Microsoft.EntityFrameworkCore;
using ShopTARpe25.Core.Domain;

namespace ShopTARpe25.Data
{
    public class ShopTARpe25Context : DbContext
    {
        public ShopTARpe25Context(DbContextOptions<ShopTARpe25Context> options)
            : base(options)
        {
        }
        //teha DbSet, et saaks andmebaasi kasutada
        public DbSet<Spaceship> Spaceships { get; set; }
     }
}