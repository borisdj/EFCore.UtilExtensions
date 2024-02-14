using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace EFCore.UtilExtensions;

public class EnumObject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public static class EnumExtension
{
    public static string GetDescription<T>(this T value)
    {
        Type enumType = value.GetType();
        FieldInfo field = enumType.GetField(value.ToString()); // Reflection
        DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        //DescriptionAttribute[] attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attribute == null ? value.ToString() : attribute.Description;
    }

    public static List<EnumObject> ToList(Type enumType)
    {
        ThrowExceptionIfNotEnumType(enumType);

        List<EnumObject> enumList = new List<EnumObject>();
        Array enumValues = enumType.GetEnumValues();

        foreach (Enum enumValue in enumValues)
        {
            EnumObject enumObject = new EnumObject()
            {
                Id = Convert.ToInt32(enumValue),
                Name = enumValue.ToString(),
                Description = enumValue.GetDescription()

            };

            enumList.Add(enumObject);
        }
        return enumList;
    }

    public static List<EnumObject> ToList<T>() where T : IConvertible
    {
        Type enumType = typeof(T);
        ThrowExceptionIfNotEnumType(enumType);
        return ToList(enumType);
    }

    public static Dictionary<int, string> ToDictionary<T>() where T : IConvertible
    {
        return ToList<T>().ToDictionary(a => a.Id, a => a.Description);
    }

    public static Dictionary<string, int> ToDictionaryByDescription<T>() where T : IConvertible
    {
        var dict = new Dictionary<string, int>();
        ToList<T>().ForEach(a =>
        {
            if (!dict.TryAdd(a.Description, a.Id))
                throw new InvalidProgramException($"Enum description: {a.Description} is not unique");
        });
        return dict;
    }

    public static Dictionary<string, int> ToDictionaryByName<T>() where T : IConvertible
    {
        var dict = new Dictionary<string, int>();
        ToList<T>().ForEach(a =>
        {
            if (!dict.TryAdd(a.Name, a.Id))
                throw new InvalidProgramException($"Enum name: {a.Name} is not unique");
        });
        return dict;
    }
    private static void ThrowExceptionIfNotEnumType(Type enumType)
    {
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("<T> must be an enumerated type");
        }
    }
}