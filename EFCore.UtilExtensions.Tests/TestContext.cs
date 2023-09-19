using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using EFCore.UtilExtensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.UtilsExtensions.Tests;

public class TestContext : DbContext
{

    public DbSet<Item> Items { get; set; } = null!;

    public DbSet<ItemHistory> ItemHistories { get; set; } = null!;

    public TestContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseSnakeCaseNamingConvention(); // for testing all lower cases, required nuget: EFCore.NamingConventions
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.RemovePluralizingTableNameConvention();

        // FluentAPI equivalent of Native and Extended Annotations:

        //modelBuilder.Entity<Item>().Property(x => x.Active).HasDefaultValue();
    }
}

public class ItemCategory
{
    [Key]
    [Column(nameof(ItemCategory) + nameof(Id))]
    public int Id { get; set; }

    [MaxLength(50)]
    [UniqueIndex("U1")]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(10)]
    [UniqueIndex("U1")]
    public string Number { get; set; } = null!;

}

public class Item
{
    public int ItemId { get; set; }

    [Required]
    [MaxLength(255)]
    [IndexExtension]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(10)]
    [UniqueIndex]
    public string Code { get; set; } = null!;


    [Column("Description")]
    public string? CustomDescription { get; set; }

    //[DecimalType(20, 4)] // default is (18, 2)
    public decimal? Price { get; set; }

    [DefaultValue(true)]
    public bool Active { get; set; }

    [ForeignKeyExtension(DeleteBehavior.Restrict)]
    public int? ItemCategoryId { get; set; }
    public virtual ItemCategory? Category { get; set; }

    public ICollection<ItemHistory> ItemHistories { get; set; } = null!;
}

[Table(nameof(ItemHistory), Schema = "dbm")]
public class ItemHistory
{
    [Key] // not required since property named 'TableNameId' or just 'Id' / 'ID' is PK by convention (default)
    //[DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid ItemHistoryId { get; set; }

    public int ItemId { get; set; }
    public virtual Item Item { get; set; } = null!;

    public decimal? Price { get; set; }

    public string Remark { get; set; } = null!;

    [Column(TypeName = (nameof(DateTime)))] // Column to be of DbType 'datetime' instead of default 'datetime2'
    [DefaultValueSql("getdate()")]
    public DateTime? TimeUpdated { get; set; }
}
