using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EFCore.UtilExtensions.Annotations;
using EFCore.UtilExtensions.AuditInfo;
using System.Windows.Input;

namespace EFCore.UtilExtensions.Test;

public class TestContext : DbContext
{
    // Default schema
    public DbSet<ItemType> ItemTypes { get; set; } = null!; // ENUM

    public DbSet<ItemCategory> ItemCategories { get; set; } = null!;

    public DbSet<Item> Items { get; set; } = null!;

    public DbSet<ItemDetail> ItemDetails { get; set; } = null!;

    // Custom schema
    public DbSet<Log> Log { get; set; } = null!;

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
    }
}

public enum ItemType
{
    [System.ComponentModel.Description("Physical")]
    Physical = 1,

    [System.ComponentModel.Description("Digital")]
    Digital = 2
}

public partial class ItemType : IEnumId
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column(nameof(ItemType) + nameof(Id))]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
}

//[Index(nameof(Name), nameof(Number), IsUnique = true, )] // Alternatively for multiple Index
//[Index(new string[] { nameof(Name), nameof(Number) }, IsUnique = true, Name = "ItemCategory_Unique_Index")] // Alternatively for multiple Index
public class ItemCategory : IEntityId, IAuditOwned, ISoftDelete
{
    //[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] // not required since property named 'TableNameId' or just 'Id' / 'ID' is PK-PrimaryKey and Identity(AutoIncrement) by convention (default)
    [Column(nameof(ItemCategory) + nameof(Id))]
    public Guid Id { get; set; }

    //[Required] // not needed, is based on property nullability
    [MaxLength(50)]
    [UniqueIndex("U1")]
    public string Name { get; set; } = null!;

    [MaxLength(10)]
    [UniqueIndex("U1")]
    public string Number { get; set; } = null!;
}

//[Table(nameof(Item), Schema = Schema.dbo)] // remains default so no need for explicit config
public class Item : IEntityId, IAuditOwned, ISoftDelete
{
    // PK in Entity class is named only 'Id' (is in its Entity Item.Id),
    // but in DB column is named TableNameId, in this case 'ItemId' (practical convention for better readability of sql joins).
    [Column(nameof(Item) + nameof(Id))]
    public Guid Id { get; set; }

    [MaxLength(255)]
    [IndexExtension]
    public string Name { get; set; } = null!;

    [MaxLength(10)]
    [UniqueIndex]
    public string Code { get; set; } = null!;

    [Column("Description")]
    public string? CustomDescription { get; set; }

    [Precision(20, 4)]
    public decimal? Price { get; set; }

    [DefaultValue(true)]
    public bool Active { get; set; }

    [ForeignKeyExtension(DeleteBehavior.NoAction)]
    public Guid? ItemCategoryId { get; set; }
    public virtual ItemCategory? Category { get; set; }

    public ICollection<ItemDetail> ItemDetails { get; set; } = null!;

    public Audit Audit { get; set; }
    public bool Deleted { get; set; }
}

[Table(nameof(ItemDetail), Schema = "dba")] // example of different schema
public class ItemDetail : IEntityId, IAuditOwned, ISoftDelete
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)] // when NotNone, Key is generated by EF before going into Db, otherwise we explicitly generate it in memory
    [Column(nameof(ItemDetail) + nameof(Id))]
    public Guid Id { get; set; }

    public int ItemId { get; set; }
    public virtual Item Item { get; set; } = null!;

    //default Precision is (18, 2)
    public decimal Price { get; set; }

    public string? Remark { get; set; }

    [Column(TypeName = (nameof(DateTime)))] // Column to be of DbType 'datetime' instead of default 'datetime2'
    [DefaultValueSql("getdate()")]
    public DateTime? TimeUpdated { get; set; }
}

[Table(nameof(Log), Schema = "util")] // example of different schema and Id with 'int' (autoincrement)
public class Log
{
    [Column(nameof(Log) + nameof(Id))]
    public int Id { get; set; }

    public string Message { get; set; }

    public DateTime? TimeCreated { get; set; }
}