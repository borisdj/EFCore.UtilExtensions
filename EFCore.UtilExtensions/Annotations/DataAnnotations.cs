using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace EFCore.UtilExtensions.Annotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class IndexExtensionAttribute : Attribute // 'IndexAttribute' already exist on class so to avoid ambiguous reference it is named 'IndexExtension', extension being sufix
{
    public string Group { get; set; }

    /// <param name="group"> for setting grouped index (same 'group') on multiple columns</param>
    public IndexExtensionAttribute(string? group = null)
    {
        Group = group;
    }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class UniqueIndexAttribute : Attribute
{
    public string? Group { get; set; }
    public string? Filter { get; set; }

    protected bool HasFilter => Filter != null;

    /// <param name="group"> for setting grouped index (same 'group') on multiple columns</param>
    /// <param name="filter"> for setting filtered index, value to be like: "[Deleted] = 0"</param>
    public UniqueIndexAttribute(string? group = null, string? filter = null)
    {
        Group = group;
        Filter = filter;
    }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class DefaultValueAttribute : Attribute
{
    public object Value { get; }

    public DefaultValueAttribute(object value)
    {
        Value = value;
    }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class DefaultValueSqlAttribute : Attribute
{
    public string Sql { get; }

    public DefaultValueSqlAttribute(string sql)
    {
        Sql = sql;
    }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class ForeignKeyExtensionAttribute : ForeignKeyAttribute
{
    public DeleteBehavior DeleteBehavior { get; set; }

    /// <param name="deleteBehavior"> for setting DeleteBehavior on ForeignKey</param>
    public ForeignKeyExtensionAttribute(DeleteBehavior deleteBehavior, [CallerMemberName] string name = null) : base(name)
    {
        DeleteBehavior = deleteBehavior;
    }
}