using EFCore.UtilExtensions.Annotations;
using EFCore.UtilExtensions.AuditInfo;
using EFCore.UtilExtensions.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCore.UtilExtensions.Tests;

public class TestContext : DbContext
{
    // Default schema
    public DbSet<ItemType> ItemTypes { get; set; } = null!; // ENUM

    public DbSet<ItemCategory> ItemCategories { get; set; } = null!;

    public DbSet<Item> Items { get; set; } = null!;

    public DbSet<ItemDetail> ItemDetails { get; set; } = null!;

    // Custom schema
    public DbSet<Message> Messages { get; set; } = null!;

    public TestContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseSnakeCaseNamingConvention(); // for testing all lower cases, required nuget: EFCore.NamingConventions
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AuditUtil.SetAuditInfo(this);

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.RemovePluralizingTableNameConvention();

        modelBuilder.ConfigureExtendedAnnotations();

        // FluentAPI equivalent of Native and Extended Annotations:
        /*
        modelBuilder.Entity<ItemCategory>().HasKey(a => a.Id);
        modelBuilder.Entity<ItemCategory>().Property(a => a.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<ItemCategory>().Property(a => a.Name).HasMaxLength(50);
        modelBuilder.Entity<ItemCategory>().Property(a => a.Number).HasMaxLength(10);
        
        modelBuilder.Entity<ItemCategory>().HasIndex(a => new { a.Name, a.Number }).IsUnique(true);

        modelBuilder.Entity<Item>().HasKey(a => a.Id);
        modelBuilder.Entity<Item>().HasKey(a => a.Id);
        modelBuilder.Entity<Item>().Property(a => a.Id).ValueGeneratedNever();
        modelBuilder.Entity<Item>().Property(a => a.Name).HasMaxLength(255);
        modelBuilder.Entity<Item>().HasIndex(a => a.Name);
        modelBuilder.Entity<Item>().Property(a => a.Code).HasMaxLength(10);
        modelBuilder.Entity<Item>().HasIndex(a => a.Code).IsUnique(true);
        modelBuilder.Entity<Item>().Property(a => a.CustomDescription).HasColumnName("Description");
        modelBuilder.Entity<Item>().Property(a => a.Price).HasPrecision(20, 4);
        modelBuilder.Entity<Item>().Property(a => a.Active).HasDefaultValue(true);
        modelBuilder.Entity<Item>().HasMany(a => a.ItemDetails).WithOne().HasForeignKey(cr => cr.ItemId).OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ItemDetail>().HasKey(a => a.Id);
        modelBuilder.Entity<ItemDetail>().Property(a => a.Id).ValueGeneratedNever();
        modelBuilder.Entity<ItemDetail>().Property(a => a.TimeUpdated).HasColumnType(nameof(DateTime)).HasDefaultValueSql("getdate()");

        modelBuilder.Entity<Log>().HasKey(a => a.Id);
        */
    }
}