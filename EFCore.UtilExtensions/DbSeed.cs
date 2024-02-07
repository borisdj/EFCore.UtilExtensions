using FastMember; // check with FastProperty
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using System;
using EFCore.UtilExtensions.Annotations;
using EFCore.UtilExtensions.AuditInfo;
using System.Data;

namespace EFCore.UtilExtensions;

public static class DbSeed
{
    public static void SyncEnumEntities(DbContext context)
    {
        var enumEntityTypes = context.Model.GetEntityTypes()
            .Select(e => new { 
                entityType = e, 
                enumType = e.ClrType.GetCustomAttribute<EnumTypeAttribute>()?.Type })
            .Where(a => a.enumType != null)
            .ToList();

        foreach (var enumEntityType in enumEntityTypes)
        {
            Type type = enumEntityType.entityType.ClrType;
            TypeAccessor accessor = TypeAccessor.Create(type);

            var enumType = enumEntityType.enumType;
            if (enumType == null)
                continue;
            var enumValues = EnumExtension.ToList(enumType).Select(a =>
            {
                Object enumObject = accessor.CreateNew();
                if (enumObject is IEnum enumObjectEnum)
                {
                    enumObjectEnum.Id = a.Id;
                    enumObjectEnum.Name = a.Name;
                    enumObjectEnum.Description = a.Description;
                }
                // previuos code:
                //accessor[enumObject, "Id"] = a.Id; //accessor[e, $"{type.Name}Id"] = (int)a[0];
                //accessor[enumObject, "Name"] = a.Description;
                return enumObject;
            }).ToList();

            //var codeValuesEnum = codeValues as IEnum;

            var dbSetMethodInfo = typeof(DbContext).GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null);
            if (dbSetMethodInfo == null)
                continue;

            dynamic method = dbSetMethodInfo.MakeGenericMethod(type);
            dynamic dbSet = method.Invoke(context, null);
            var record = Activator.CreateInstance(type);

            dbSet?.AddRange(enumValues.ToArray());
            context.SaveChanges();

            var list = dbSet?.GetMethod("ToList").MakeGenericMethod(dbSet).Invoke(dbSet, null);
            foreach (var row in list)
            {
                    
            }

            //newCodeValues = codeValuesEnum.Where();

            /*
            MethodInfo contextSetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(type);
            var set = contextSetMethod.Invoke(context, null);// as IQueryable;
            var dbValues = set.ToDynamicList();

            context.RemoveRange(dbValues);
            context.AddRange(codeValues);///Warning if cascade delete on enum fk when deleting all entities are deleted for missing enums
            */
        }
    }
}