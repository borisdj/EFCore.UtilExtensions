using FastMember;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using System;
using EFCore.UtilExtensions.Annotations;

namespace EFCore.UtilExtensions;

public static class DbSeed
{
    public static void SyncEnumEntities(DbContext context)
    {
        var enumEntityTypes = context.Model.GetEntityTypes()
            .Select(e => new { entityType = e, enumType = e.ClrType.GetCustomAttribute<EnumTypeAttribute>()?.Type })
            .Where(a => a.enumType != null)
            .ToList();

        foreach (var enumEntityType in enumEntityTypes)
        {
            Type type = enumEntityType.entityType.ClrType;
            TypeAccessor accessor = TypeAccessor.Create(type);

            var codeValues = EnumExtension.ToList(enumEntityType.enumType).Select(a =>
            {
                Object e = accessor.CreateNew();
                accessor[e, "Id"] = a.Id; //accessor[e, $"{type.Name}Id"] = (int)a[0];
                accessor[e, "Name"] = a.Description;
                return e;
            }).ToList();

            MethodInfo contextSetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(type);

            var set = contextSetMethod.Invoke(context, null) as IQueryable;
            var dbValues = set.ToDynamicList();

            context.RemoveRange(dbValues);
            context.AddRange(codeValues);///Warning if cascade delete on enum fk when deleting all entities are deleted for missing enums              
        }
    }
}