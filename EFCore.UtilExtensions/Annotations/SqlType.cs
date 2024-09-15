namespace EFCore.UtilExtensions;

public enum SqlType
{
    /// <summary>
    /// Indicates database is Microsoft's SQL Server
    /// </summary>
    SqlServer,

    /// <summary>
    /// Indicates database is SQLite
    /// </summary>
    Sqlite,

    /// <summary>
    /// Indicates database is PostgreSQL
    /// </summary>
    PostgreSql,

    /// <summary>
    ///  Indicates database is MySQL
    /// </summary>
    MySql,
}