using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using FastMember;
using EFCore.UtilExtensions.Entity;

namespace EFCore.UtilExtensions.AuditInfo;

public class IgnoreAuditInfoAttribute : Attribute { }

public static class AuditUtil
{
    // AuditInfo is tracked in ChangeHistory column using deltas - storing only differences(updated columns) starting from last version and descending [jsonFormat]
    public static void SetAuditInfo(DbContext dbContext)
    {
        var data = dbContext.ChangeTracker.Entries()
            .GroupBy(a => new { Type = a.Entity.GetType(), a.State })
            .Select(a => new
            {
                a.Key.Type,
                a.Key.State,
                Entities = a.Select(x => x.Entity),
                PreviousEntities = a.Select(x => x.OriginalValues.ToObject())
            })
            .Where(a => a.State == EntityState.Added || a.State == EntityState.Modified)
            .Where(a => HasAudit(a.Type))
            .ToList();

        foreach (var d in data)
        {
            var entities = d.Entities.ToList();
            if (d.State == EntityState.Added)
            {
                SetAuditInfo(entities, d.Type);
            }
            else if (d.State == EntityState.Modified)
            {
                SetAuditInfo(entities, d.Type, d.PreviousEntities.ToList());
            }
        }
    }

    public static void SetAuditInfo<T>(IList<T> entities, IList<T> previousEntities = null)
    {
        SetAuditInfo(entities, typeof(T), previousEntities);
    }

    /// <summary>
    /// Used with Bulk operations (Insert / Update) when SaveChanges is not being called.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entities"></param>
    /// <param name="previousEntities">required for BulkUpdate</param>
    public static void SetAuditInfo<T>(IList<T> entities, Type entityType, IList<T> previousEntities = null)
    {
        if (!HasAudit(entityType))
            return;

        List<PropertyInfo>? propertyInfos = null;

        if (entities == null || entities.Count == 0)
        {
            throw new InvalidOperationException("List of entities can not be null nor empty.");
        }

        var auditInfoPropertiesSet = new HashSet<string> {
                nameof(IAudit.ChangedBy),
                nameof(IAudit.ChangedTime),
                nameof(IAudit.RowVersion),
                nameof(IAudit.ChangeHistory),
                nameof(IAuditCreate.CreatedBy),
                nameof(IAuditCreate.CreatedTime)
            };

        if (previousEntities != null) // if it's Update operation previousEntities are send for determining which properties were changed
        {
            if (entities.Count != previousEntities.Count)
                throw new InvalidOperationException("List of entities and previousEntities are not the same size.");

            propertyInfos = entityType.GetProperties()
                .Where(a => !a.GetGetMethod().IsVirtual
                    && !auditInfoPropertiesSet.Contains(a.Name)
                    && a.GetCustomAttribute<IgnoreAuditInfoAttribute>() != null
                ).ToList();
        }

        var userName = "UserName"; //UserName; TODO
        var changedTime = DateTime.Now;

        bool isIAudit = typeof(IAudit).IsAssignableFrom(entityType);
        bool isIAuditCreate = typeof(IAuditCreate).IsAssignableFrom(entityType);

        string primaryKeyText = entities[0] is IEntityId ? nameof(IEntityId.Id)
            : entities[0] is IEnum ? nameof(IEnum.Id) : Unproxy(entityType).Name + "Id";

        TypeAccessor accessor = TypeAccessor.Create(entityType);
        bool isUpdate = previousEntities != null;

        for (int i = 0; i < entities.Count; i++)
        {
            object entity = entities[i];

            if (isIAudit)
            {
                IAudit audit = (IAudit)entity;
                if (isUpdate)
                {
                    bool hasChange = false;

                    if (!(entity is IHasHistoryTable)) // adding to HistoryTable could be done here automatically with else { ...
                    {
                        if (accessor[entity, primaryKeyText].ToString() != accessor[previousEntities[i], primaryKeyText].ToString())
                            throw new InvalidOperationException("EntityId not equal for same position in previousEntities list.");

                        IDictionary<string, object> updatedProperties = GetAuditUpdatedProperties(audit, userName);
                        foreach (var propertyInfo in propertyInfos)
                        {
                            var originalValue = accessor[previousEntities[i], propertyInfo.Name];

                            var newValue = accessor[entity, propertyInfo.Name];
                            if (newValue?.ToString() != originalValue?.ToString())
                            {
                                updatedProperties[propertyInfo.Name] = originalValue;
                                hasChange = true;
                            }
                        }

                        if (hasChange)
                        {
                            SetChangeHistory(audit, updatedProperties);
                            SetAuditProperties(audit, userName, changedTime);
                        }
                    }
                }
                else
                {
                    SetAuditProperties(audit, userName, changedTime);
                }
            }

            if (isIAuditCreate && !isUpdate)
            {
                IAuditCreate audit = (IAuditCreate)entity;
                SetAuditCreateProperties(audit, userName, changedTime);
            }
        }
    }

    public static IDictionary<string, object> GetAuditUpdatedProperties(IAudit audit, string userName)
    {
        var updatedProperties = (IDictionary<string, object>)new ExpandoObject();
        updatedProperties[nameof(IAudit.ChangedTime)] = audit.ChangedTime?.ToString("s");
        if (audit.ChangedBy != userName)
        {
            updatedProperties[nameof(IAudit.ChangedBy)] = audit.ChangedBy;
        }
        return updatedProperties;
    }

    public static void SetChangeHistory(IAudit audit, IDictionary<string, object> updatedProperties)
    {
        //var updatedPropertiesJson = JsonConvert.SerializeObject(updatedProperties); // TODO

        audit.ChangeHistory = !string.IsNullOrEmpty(audit.ChangeHistory) ? audit.ChangeHistory : "[]";
        audit.ChangeHistory = audit.ChangeHistory.Remove(audit.ChangeHistory.Length - 1); // removes closing brackets ']'
        audit.ChangeHistory += audit.ChangeHistory.Length > 1 ? "," : "";
        //audit.ChangeHistory += updatedPropertiesJson; // TODO
        audit.ChangeHistory += "]";
    }

    public static void SetAuditProperties(IAudit audit, string userName, DateTime changedTime)
    {
        audit.ChangedBy = userName;
        audit.ChangedTime = changedTime;
        audit.RowVersion++; // RowVersion not needed in ChangeHistory, can be calculed by counting
    }

    public static void SetAuditCreateProperties(IAuditCreate audit, string userName, DateTime createdTime)
    {
        audit.CreatedBy = userName;
        audit.CreatedTime = createdTime;
    }

    public static bool HasAudit(Type type)
    {
        return type.GetInterfaces().Any(a => GetAuditInterfaceTypes().Contains(a));
    }

    public static Type[] GetAuditInterfaceTypes() => new Type[] { typeof(IAudit), typeof(IAuditCreate) };

    public static Type Unproxy(Type type)
    {
        if (type.Namespace == "Castle.Proxies")
        {
            return type.BaseType;
        }

        return type;
    }
}