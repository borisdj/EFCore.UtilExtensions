using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace EFCore.UtilExtensions.Test;

public static class ContextOptions
{
    static SqlType _databaseType;

    // TODO: Pass DbService through all the GetOptions methods as a parameter and eliminate this property so the automated tests
    // are thread safe
    public static SqlType DatabaseType
    {
        get => _databaseType;
        set
        {
            _databaseType = value;
        }
    }

    public static DbContextOptions GetOptions(IInterceptor dbInterceptor) => GetOptions(new[] { dbInterceptor });
    public static DbContextOptions GetOptions(IEnumerable<IInterceptor>? dbInterceptors = null) => GetOptions<TestContext>(dbInterceptors);

    public static DbContextOptions GetOptions<TDbContext>(IEnumerable<IInterceptor>? dbInterceptors = null, string databaseName = nameof(EFCoreUtilTest))
        where TDbContext : DbContext
        => GetOptions<TDbContext>(ContextOptions.DatabaseType, dbInterceptors, databaseName);

    public static DbContextOptions GetOptions<TDbContext>(SqlType dbServerType, IEnumerable<IInterceptor>? dbInterceptors = null, string databaseName = nameof(EFCoreUtilTest))
        where TDbContext : DbContext
    {
        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();

        //if (dbServerType == SqlType.SqlServer)
        //{
        var connectionString = GetSqlServerConnectionString(databaseName);

        // ALTERNATIVELY (Using MSSQLLocalDB):
        //var connectionString = $@"Data Source=(localdb)\MSSQLLocalDB;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=True";

        //optionsBuilder.UseSqlServer(connectionString); // Can NOT Test with UseInMemoryDb (Exception: Relational-specific methods can only be used when the context is using a relational)
        //optionsBuilder.UseSqlServer(connectionString, opt => opt.UseNetTopologySuite()); // NetTopologySuite for Geometry / Geometry types
        optionsBuilder.UseSqlServer(connectionString, opt =>
        {
        });
        //}
        //else
        //{
        //    throw new NotSupportedException($"Database {dbServerType} is not supported. Only SQL Server and SQLite are Currently supported.");
        //}

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