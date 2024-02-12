using EFCore.UtilExtensions.Annotations;
using EFCore.UtilExtensions.AuditInfo;
using EFCore.UtilExtensions.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCore.UtilExtensions.Tests;

public class TestContext : DbContext
{
    //DB Structure
    // ItemType --------|
    // ItemCategory ----|
    //                  Item ---|
    //                          ItemDetail

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
        DbSeed.SyncEnumEntities(this);
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

        //modelBuilder.Entity<Audit>().Property(a => a.ChangedBy).Metadata.IsNullable = false;

        // FluentAPI equivalent of Native and Extended Annotations:
        /*
        modelBuilder.Entity<ItemType>().HasKey(a => a.Id); // is set by default with EFCore naming convention
        modelBuilder.Entity<ItemType>().Property(a => a.Id).ValueGeneratedNever(); // is set by default with EFCore naming convention so here we disable it
        
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
        
        modelBuilder.Entity<ItemDetail>().HasKey(a => a.Id);
        modelBuilder.Entity<ItemDetail>().Property(a => a.Id).ValueGeneratedNever();
        //modelBuilder.Entity<ItemDetail>().HasOne(a => a.Item).WithMany().HasForeignKey(a => a.ItemId).OnDelete(DeleteBehavior.NoAction); // creates another fk ItemId1
        modelBuilder.Entity<ItemDetail>().Property(a => a.TimeUpdated).HasColumnType(nameof(DateTime)).HasDefaultValueSql("getdate()");
        
        modelBuilder.Entity<Message>().HasKey(a => a.Id);
        */
    }
}
