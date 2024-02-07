using System;

namespace EFCore.UtilExtensions.AuditInfo;

public interface IEnum : IEntityIdN, IName, IDescription { }

public interface IEnumBase : IEntityIdN, IName { }

public interface IName
{
    string Name { get; set; }
}

public interface IDescription
{
    string Description { get; set; }
}