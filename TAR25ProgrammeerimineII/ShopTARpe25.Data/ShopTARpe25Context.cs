using Microsoft.EntityFrameworkCore;
using ShopTARpe25.Core.Domain;

namespace ShopTARpe25.Data
{
    // DbContext on sild C# objektide ja andmebaasi vahel.
    public class ShopTARpe25Context : DbContext
    {
        // Ühenduse seadistus:
        // 1) appsettings.json -> "ConnectionStrings:DefaultConnection" sisaldab ühendusstringi
        // 2) Program.cs -> AddDbContext<ShopTARpe25Context>(o => o.UseSqlServer(...)) loeb selle
        //    konfiguratsioonist ja registreerib konteksti DI-konteinerisse
        // 3) DI annab selle options-objekti siia konstruktorisse
        public ShopTARpe25Context(DbContextOptions<ShopTARpe25Context> options)
            : base(options)
        {
        }

        // Iga DbSet = üks tabel andmebaasis
        public DbSet<Book> Books { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Book 1 : N Loan
            // Restrict: andmebaas ei luba kustutada raamatut, millel on laenutusi
            // (laenutused tuleb enne ise ära kustutada, vt LibaryService.Delete)
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // Reader 1 : N Loan
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Reader)
                .WithMany(r => r.Loans)
                .HasForeignKey(l => l.ReaderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
