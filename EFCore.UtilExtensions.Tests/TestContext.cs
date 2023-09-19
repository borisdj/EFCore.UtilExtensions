using EFCore.UtilExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

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

public static class ContextUtil
{
    static IDbServer? _dbServerMapping;
    static SqlType _databaseType;

    // TODO: Pass DbService through all the GetOptions methods as a parameter and eliminate this property so the automated tests
    // are thread safe
    public static SqlType DatabaseType
    {
        get => _databaseType;
        set
        {
            _databaseType = value;
            _dbServerMapping = value switch
            {
                SqlType.SqlServer => new SqlAdapters.SqlServer.SqlServerDbServer(),
                SqlType.Sqlite => new SqlAdapters.Sqlite.SqliteDbServer(),
                SqlType.PostgreSql => new SqlAdapters.PostgreSql.PostgreSqlDbServer(),
                SqlType.MySql => new SqlAdapters.MySql.MySqlDbServer(),
                _ => throw new NotImplementedException(),
            };
        }
    }

    public static DbContextOptions GetOptions(IInterceptor dbInterceptor) => GetOptions(new[] { dbInterceptor });
    public static DbContextOptions GetOptions(IEnumerable<IInterceptor>? dbInterceptors = null) => GetOptions<TestContext>(dbInterceptors);

    public static DbContextOptions GetOptions<TDbContext>(IEnumerable<IInterceptor>? dbInterceptors = null, string databaseName = nameof(EFCoreUtilTest))
        where TDbContext : DbContext
        => GetOptions<TDbContext>(ContextUtil.DatabaseType, dbInterceptors, databaseName);

    public static DbContextOptions GetOptions<TDbContext>(SqlType dbServerType, IEnumerable<IInterceptor>? dbInterceptors = null, string databaseName = nameof(EFCoreUtilTest))
        where TDbContext : DbContext
    {
        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();

        //if (dbServerType == SqlType.SqlServer)
        {
            var connectionString = GetSqlServerConnectionString(databaseName);

            // ALTERNATIVELY (Using MSSQLLocalDB):
            //var connectionString = $@"Data Source=(localdb)\MSSQLLocalDB;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=True";

            //optionsBuilder.UseSqlServer(connectionString); // Can NOT Test with UseInMemoryDb (Exception: Relational-specific methods can only be used when the context is using a relational)
            //optionsBuilder.UseSqlServer(connectionString, opt => opt.UseNetTopologySuite()); // NetTopologySuite for Geometry / Geometry types
            optionsBuilder.UseSqlServer(connectionString, opt =>
            {
            });
        }
        else
        {
            throw new NotSupportedException($"Database {dbServerType} is not supported. Only SQL Server and SQLite are Currently supported.");
        }

        if (dbInterceptors?.Any() == true)
        {
            optionsBuilder.AddInterceptors(dbInterceptors);
        }
        return optionsBuilder.Options;
    }

    private static IConfiguration GetConfiguration()
    {
        var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("testsettings.json", optional: false)
            .AddJsonFile("testsettings.local.json", optional: true);

        return configBuilder.Build();
    }

    public static string GetSqlServerConnectionString(string databaseName)
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        return GetConfiguration().GetConnectionString("SqlServer").Replace("{databaseName}", databaseName);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
    [Key] // not required since property named 'TableNameId' or just 'Id' / 'ID' is PK-PrimaryKey by convention (default)
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
