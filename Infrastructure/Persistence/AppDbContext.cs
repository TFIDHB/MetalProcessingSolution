using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<MetalService> Services => Set<MetalService>();
    public DbSet<UnliquidProduct> UnliquidProducts => Set<UnliquidProduct>();
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

        modelBuilder.Entity<UnliquidProduct>()
            .HasMany(p => p.Images)
            .WithOne(i => i.UnliquidProduct)
            .HasForeignKey(i => i.UnliquidProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MetalService>().HasData(
            new MetalService { Id = 1, Title = "Лазерная резка ЧПУ", Description = "Высокоточный раскрой листового проката до 20мм.", PriceFrom = 12.50 },
            new MetalService { Id = 2, Title = "Гибка листового металла", Description = "Радиусная и профильная гибка на гидравлических прессах.", PriceFrom = 6.80 }
        );

        modelBuilder.Entity<UnliquidProduct>().HasData(
            new UnliquidProduct { Id = 1, Name = "Стальной лист ГК 10мм", Description = "Лист горячекатаный, марка Ст3сп, размер 1500х6000.", Price = 450.00, Quantity = "4 шт" },
            new UnliquidProduct { Id = 2, Name = "Труба профильная 40х40х3", Description = "Остатки после производства металлоконструкций. Длина хлыстов 3м.", Price = 12.80, Quantity = "25 шт" }
        );

        base.OnModelCreating(modelBuilder);
    }
}
