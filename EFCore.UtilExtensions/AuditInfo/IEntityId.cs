using System;

namespace EFCore.UtilExtensions.AuditInfo;

public interface IId<T>
{
    T Id { get; set; }
}

public interface IEntityId : IId<Guid> { }

public interface IEntityIdn : IId<int> { } // Id + 'n' (n - IdeNtity iNt)

public interface IEnumId : IEntityIdn, IName { }