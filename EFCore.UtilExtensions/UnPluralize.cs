using FastMember;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using System.Reflection;
using System;
using EFCore.UtilExtensions.Annotations;

namespace EFCore.UtilExtensions;

public static class UnPluralize
{
    public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
    {
        foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
        {
            if (!entity.IsOwned() && entity.BaseType == null) // without this exclusion OwnedType would not be by default in Owner Table
            {
                entity.SetTableName(entity.ClrType.Name);
            }
        }
    }
}