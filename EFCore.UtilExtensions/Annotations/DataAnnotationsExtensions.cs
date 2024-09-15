using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EFCore.UtilExtensions.Annotations;

public static class DataAnnotationsExtensions
{
    public static void ConfigureExtendedAnnotations(this ModelBuilder modelBuilder)
    {
        var entityTypes = modelBuilder.Model.GetEntityTypes().Where(a => !a.IsOwned());
        foreach (var entityType in entityTypes)
        {
            var indexAttributeGroupsProperties = new Dictionary<string, List<string>>();
            var uniqueIndexAttributeGroupsProperties = new Dictionary<string, List<string>>();
            var uniqueIndexAttributeGroupsFilter = new Dictionary<string, string>();

            foreach (var property in entityType.GetProperties())
            {
                // [Index()]
                var indexAttribute = property.PropertyInfo?.GetCustomAttributes(typeof(IndexExtensionAttribute), true).FirstOrDefault() as IndexExtensionAttribute; //entityType.ClrType.GetMembers().Where(a => a.IsDefined(typeof(IndexAttribute)))
                if (indexAttribute != null)
                {
                    if (indexAttribute.Group == null)
                    {
                        modelBuilder.Entity(entityType.ClrType).HasIndex(property.Name);
                    }
                    else // when Group is set then multi-column index set after loop from Dict
                    {
                        if (indexAttributeGroupsProperties.ContainsKey(indexAttribute.Group))
                        {
                            indexAttributeGroupsProperties[indexAttribute.Group].Add(property.Name);
                        }
                        else
                        {
                            indexAttributeGroupsProperties.Add(indexAttribute.Group, new List<string> { property.Name });
                        }
                    }
                }

                // [UniqueIndex()]
                var uniqueIndexAttribute = property.PropertyInfo?.GetCustomAttributes(typeof(UniqueIndexAttribute), true).FirstOrDefault() as UniqueIndexAttribute; //entityType.ClrType.GetMembers().Where(a => a.IsDefined(typeof(UniqueIndexAttribute)))
                if (uniqueIndexAttribute != null)
                {
                    if (uniqueIndexAttribute.Group == null)
                    {
                        var uniqueIndex = modelBuilder.Entity(entityType.ClrType).HasIndex(property.Name).IsUnique();
                        if (uniqueIndexAttribute.Filter == null)
                        {
                            uniqueIndex.HasFilter(uniqueIndexAttribute.Filter);
                        }
                    }
                    else // when Group is set then multi-column index set after loop from Dict
                    {
                        if (uniqueIndexAttributeGroupsProperties.ContainsKey(uniqueIndexAttribute.Group))
                        {
                            uniqueIndexAttributeGroupsProperties[uniqueIndexAttribute.Group].Add(property.Name);
                        }
                        else
                        {
                            uniqueIndexAttributeGroupsProperties.Add(uniqueIndexAttribute.Group, new List<string> { property.Name });
                            if (uniqueIndexAttribute.Filter != null)
                            {
                                uniqueIndexAttributeGroupsFilter.Add(uniqueIndexAttribute.Group, uniqueIndexAttribute.Filter);
                            }
                        }
                    }
                }

                // [DefaultValue()]
                var defaultValueAttribute = property.PropertyInfo?.GetCustomAttributes(typeof(DefaultValueAttribute), true).FirstOrDefault();
                if (defaultValueAttribute != null)
                {
                    var defaultValue = ((DefaultValueAttribute)defaultValueAttribute).Value;
                    modelBuilder.Entity(entityType.ClrType).Property(property.Name).HasDefaultValue(defaultValue);
                }

                // [DefaultValueSql()]
                var defaultValueSqlAttribute = property.PropertyInfo?.GetCustomAttributes(typeof(DefaultValueSqlAttribute), true).FirstOrDefault();
                if (defaultValueSqlAttribute != null)
                {
                    string sql = ((DefaultValueSqlAttribute)defaultValueSqlAttribute).Sql;
                    modelBuilder.Entity(entityType.ClrType).Property(property.Name).HasDefaultValueSql(sql);
                }

                // [ForeignKeyExtension(DeleteBehavior.Restrict)]
                var foreignKeysExtension = GetForeignKeyExtensionAttributes(entityType, property);
                if (foreignKeysExtension != null && foreignKeysExtension.Any())
                {
                    var foreignKeyExtension = foreignKeysExtension.First();
                    var foreignKey = property.GetContainingForeignKeys().First();
                    foreignKey.DeleteBehavior = foreignKeyExtension.DeleteBehavior;
                }
            }

            foreach (var indexAttributeGroup in indexAttributeGroupsProperties)
            {
                modelBuilder.Entity(entityType.ClrType).HasIndex(indexAttributeGroup.Value.ToArray());
            }
            foreach (var uniqueIndexAttributeGroup in uniqueIndexAttributeGroupsProperties)
            {
                var uniqueIndexGrouped = modelBuilder.Entity(entityType.ClrType).HasIndex(uniqueIndexAttributeGroup.Value.ToArray()).IsUnique();
                if (uniqueIndexAttributeGroupsFilter.ContainsKey(uniqueIndexAttributeGroup.Key))
                {
                    uniqueIndexGrouped.HasFilter(uniqueIndexAttributeGroupsFilter[uniqueIndexAttributeGroup.Key]);
                }
            }
        }
    }

    public static IEnumerable<ForeignKeyExtensionAttribute>? GetForeignKeyExtensionAttributes(IMutableEntityType entityType, IMutableProperty property)
    {
        var propertyInfo = GetPropertyInfo(entityType, property);
        return propertyInfo?.GetCustomAttributes<ForeignKeyExtensionAttribute>();
    }

    public static PropertyInfo GetPropertyInfo(IMutableEntityType entityType, IMutableProperty property)
    {
        if (entityType == null)
        {
            throw new ArgumentNullException(nameof(entityType));
        }
        else if (entityType.ClrType == null)
        {
            throw new ArgumentNullException(nameof(entityType.ClrType));
        }
        else if (property == null)
        {
            throw new ArgumentNullException(nameof(property));
        }
        else if (property.Name == null)
        {
            throw new ArgumentNullException(nameof(property.Name));
        }

        //var bindingFlag = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        var propertyInfo = entityType.ClrType.GetProperty(property.Name);
        return propertyInfo;
    }
}