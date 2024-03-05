using System;
using EFCore.UtilExtensions.Entity;

namespace EFCore.UtilExtensions.AuditInfo;

public interface IHasParentEntity : IEntityId
{
    Guid ParentId { get; set; }
}

public interface IHistoryEntityFlatten : IHasParentEntity, IHistoryEntity
{
}

public interface IHistoryEntity<T> : IHistoryEntityFlatten
{
    T Parent { get; set; }
}

public interface IHistoryEntity
{
    DateTime EffectiveDate { get; set; }

    DateTime? DefectiveDate { get; set; }
}

public interface IHasHistoryTable { }