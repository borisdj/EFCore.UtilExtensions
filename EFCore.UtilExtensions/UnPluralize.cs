using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EFCore.UtilExtensions;

public static class UtilConvention
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