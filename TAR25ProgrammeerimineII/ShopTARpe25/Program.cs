using Microsoft.EntityFrameworkCore;
using ShopTARpe25.ApplicationServices.Services;
using ShopTARpe25.Core.ServiceInterface;
using ShopTARpe25.Data;

namespace ShopTARpe25
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ÜHENDUSE SEADISTAMINE (Ülesanne 2):
            // appsettings.json failis on "ConnectionStrings": { "DefaultConnection": "..." }.
            // GetConnectionString("DefaultConnection") loeb selle ja UseSqlServer ütleb EF Core'ile,
            // et andmebaas on SQL Server (LocalDB). Vajalikud NuGet paketid:
            // Microsoft.EntityFrameworkCore.SqlServer ja Microsoft.EntityFrameworkCore.Tools.
            builder.Services.AddDbContext<ShopTARpe25Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Kontroller küsib IBookServices, DI annab talle LibaryService'i
            builder.Services.AddScoped<IBookServices, LibaryService>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Libary}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
