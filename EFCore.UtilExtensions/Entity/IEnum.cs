namespace EFCore.UtilExtensions.Entity;

public interface IEnum : IEnumBase, IDescription { }

public interface IEnumBase : IEntityIdN, IName { }

public interface IName
{
    string Name { get; set; }
}

public interface IDescription
{
    string Description { get; set; }
}