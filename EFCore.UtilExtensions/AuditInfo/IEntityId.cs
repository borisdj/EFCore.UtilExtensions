using System;

namespace EFCore.UtilExtensions.AuditInfo;

public interface IId<T>
{
    T Id { get; set; }
}

public interface IEntityId : IId<Guid> { }

public interface IEntityIdN : IId<int> { }      // IEntityId + 'N' (iNt; Number - Numberic)

public interface IEntityIdN64 : IId<long> { }   // IEntityId + 'N64' (Int64 - long)

public interface IEntityIdDec : IId<decimal> { }// IEntityId + 'D' (Decimal)

public interface IEntityIdS : IId<string> { }   // IEntityId + 'S' (String)