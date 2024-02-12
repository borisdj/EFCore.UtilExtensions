using EFCore.UtilExtensions.Annotations;
using EFCore.UtilExtensions.AuditInfo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading; // Async

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

            var enumType = enumEntityType.enumType;
            if (enumType == null)
                continue;
            var enumsValues = EnumExtension.ToList(enumType).Select(a =>
            {
                Object? enumObject = Activator.CreateInstance(type);
                if (enumObject is IEnum enumObjectEnum)
                {
                    enumObjectEnum.Id = a.Id;
                    enumObjectEnum.Name = a.Name;
                    enumObjectEnum.Description = a.Description;
                }
                // previously:
                //accessor[enumObject, "Id"] = a.Id; //accessor[e, $"{type.Name}Id"] = (int)a[0]; // (using FastMember;) TypeAccessor accessor = TypeAccessor.Create(type); 
                //accessor[enumObject, "Name"] = a.Description; //  enumObject =  accessor.CreateNew();
                return enumObject as IEnum;
            }).ToList();

            MethodInfo? contextSetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null);
            if (contextSetMethod == null)
                continue; //added to omit null warning in the code below

            MethodInfo genericSetMethodInfo = contextSetMethod.MakeGenericMethod(type);

            IQueryable<IEnum> queryable = ((IQueryable)genericSetMethodInfo.Invoke(context, null)).Cast<IEnum>();
            var entitesInDb = queryable.ToList();

            List<IEnum>? enumsToAdd = new List<IEnum>();
            List<IEnum>? enumsToUpdate = new List<IEnum>();
            if (entitesInDb != null && entitesInDb is List<IEnum>) // List<IEnumBase>
            {
                List<IEnum> enumsInDb = entitesInDb;
                foreach (var enumValue in enumsValues)
                {
                    var enumInDb = enumsInDb.SingleOrDefault(a => a.Id == enumValue.Id);
                    if (enumInDb != null)
                    {
                        if (enumInDb.Name != enumValue.Name || enumInDb.Description != enumValue.Description)
                        {
                            enumInDb.Name = enumValue.Name;
                            enumInDb.Description = enumValue.Description;
                            enumsToUpdate.Add(enumInDb);
                        }
                    }
                    else
                    {
                        enumsToAdd.Add(enumValue);
                    }
                }
            }

            context.AddRange(enumsToAdd);
            //context.RemoveRange(enumsToRemove); // Warning if Enum FK has CascadeDelete then all entities are deleted for removed enums
                                                  // Better to use NoAction or Restrict
            context.SaveChanges();
        }
    }
}