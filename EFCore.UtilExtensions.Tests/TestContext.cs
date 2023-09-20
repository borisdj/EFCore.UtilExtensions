using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.UtilExtensions.Test;

public class TestContext : DbContext
{
    public DbSet<ItemCategory> ItemCategories { get; set; } = null!;

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

        modelBuilder.ConfigureExtendedAnnotations();
        // FluentAPI equivalent of Native and Extended Annotations:
        //modelBuilder.Entity<Item>().Property(x => x.Active).HasDefaultValue();
    }
}

//[Index(new string[] { nameof(Name), nameof(Number) })] // Alternatively for multiple Index
public class ItemCategory
{
    [Key]
    [Column(nameof(ItemCategory) + nameof(Id))]
    public int Id { get; set; }

    //[Required] // not need, is based on property nullability
    [MaxLength(50)]
    [UniqueIndex("U1")]
    public string Name { get; set; } = null!;

    [MaxLength(10)]
    [UniqueIndex("U1")]
    public string Number { get; set; } = null!;

}

public class Item
{
    public int ItemId { get; set; }

    [MaxLength(255)]
    [IndexExtension]
    public string Name { get; set; } = null!;

    [MaxLength(10)]
    [UniqueIndex]
    public string Code { get; set; } = null!;

    [Column("Description")]
    public string? CustomDescription { get; set; }

    [DecimalType(20, 4)] // default is (18, 2)
    public decimal? Price { get; set; }

    [DefaultValue(true)]
    public bool Active { get; set; }

    [ForeignKeyExtension(DeleteBehavior.Cascade)]
    public int? ItemCategoryId { get; set; }
    public virtual ItemCategory? Category { get; set; }

    public ICollection<ItemHistory> ItemHistories { get; set; } = null!;
}

[Table(nameof(ItemHistory), Schema = "dba")]
public class ItemHistory
{
    //[Key] // not required since property named 'TableNameId' or just 'Id' / 'ID' is PK-PrimaryKey by convention (default)
    [DatabaseGenerated(DatabaseGeneratedOption.None)] // when NotNone, Key is generated by EF before going into Db, otherwise we explicitly generate it in memory
    public Guid ItemHistoryId { get; set; }

    public int ItemId { get; set; }
    public virtual Item Item { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Remark { get; set; }

    [Column(TypeName = (nameof(DateTime)))] // Column to be of DbType 'datetime' instead of default 'datetime2'
    [DefaultValueSql("getdate()")]
    public DateTime? TimeUpdated { get; set; }
}