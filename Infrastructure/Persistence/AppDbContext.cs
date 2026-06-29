using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<MetalService> Services => Set<MetalService>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<MetalService>()
                .HasMany(s => s.Images)
                .WithOne(i => i.MetalService)
                .HasForeignKey(i => i.MetalServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MetalService>().HasData(
                new MetalService { Id = 1, Title = "Лазерная резка ЧПУ", Description = "Высокоточный раскрой листового проката до 20мм.", PriceFrom = 12.50m },
                new MetalService { Id = 2, Title = "Гибка листового металла", Description = "Радиусная и профильная гибка на гидравлических прессах.", PriceFrom = 6.80m }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Стальной лист ГК 10мм", Description = "Лист горячекатаный, марка Ст3сп, размер 1500х6000.", Price = 450.00m, Quantity = "4 шт", Category = ProductCategory.Unliquid },
                new Product { Id = 2, Name = "Труба профильная 40х40х3", Description = "Остатки после производства металлоконструкций. Длина хлыстов 3м.", Price = 12.80m, Quantity = "25 шт", Category = ProductCategory.Unliquid }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}