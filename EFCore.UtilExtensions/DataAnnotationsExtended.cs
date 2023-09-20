using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EFCore.UtilExtensions;

public static class DataAnnotationsExtended
{
    public static void ConfigureExtendedAnnotations(this ModelBuilder modelBuilder)
    {
        var entityTypes = modelBuilder.Model.GetEntityTypes();
        foreach (var entityType in entityTypes)
        {
            //if (entityType.IsOwned())
            //    continue;

            var indexAttributeGroupsProperties = new Dictionary<string, List<string>>();
            var uniqueIndexAttributeGroupsProperties = new Dictionary<string, List<string>>();

            foreach (var property in entityType.GetProperties())
            {
                // [Index()]
                var indexAttribute = (IndexExtensionAttribute)property.PropertyInfo?.GetCustomAttributes(typeof(IndexExtensionAttribute), true).FirstOrDefault(); //entityType.ClrType.GetMembers().Where(a => a.IsDefined(typeof(IndexAttribute)))
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
                var uniqueIndexAttribute = (UniqueIndexAttribute)property.PropertyInfo?.GetCustomAttributes(typeof(UniqueIndexAttribute), true).FirstOrDefault(); //entityType.ClrType.GetMembers().Where(a => a.IsDefined(typeof(UniqueIndexAttribute)))
                if (uniqueIndexAttribute != null)
                {
                    if (uniqueIndexAttribute.Group == null)
                    {
                        modelBuilder.Entity(entityType.ClrType).HasIndex(property.Name).IsUnique();
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

                // default decimal precision
                if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                {
                    if (entityType.IsOwned())
                    {
                        //modelBuilder.Entity<TransportInfo>().OwnsOne("CarrierInvoice", "CarrierInvoice");
                        //modelBuilder.Entity<TransportInfo>().OwnsOne(entityType.GetTableName(), entityType.GetTableName()).Property(property.Name).HasColumnType(DecimalTypeDefaultPrecision);
                    }
                    else
                    {
                        //modelBuilder.Entity(entityType.ClrType).Property(property.Name).HasColumnType(DecimalTypeDefaultPrecision);
                    }
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
                modelBuilder.Entity(entityType.ClrType).HasIndex(uniqueIndexAttributeGroup.Value.ToArray()).IsUnique();
            }
        }

        //modelBuilder.Entity<Block>().Property(a => a.NetVolume).HasColumnType("decimal(18, 3)");
    }

    public static IEnumerable<ForeignKeyExtensionAttribute> GetForeignKeyExtensionAttributes(IMutableEntityType entityType, IMutableProperty property)
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