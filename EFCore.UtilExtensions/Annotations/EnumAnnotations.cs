using System;

namespace EFCore.UtilExtensions.Annotations;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EnumTypeAttribute : Attribute
{
    public Type Type { get; set; }

    public EnumTypeAttribute(Type type)
    {
        Type = type;
    }
}