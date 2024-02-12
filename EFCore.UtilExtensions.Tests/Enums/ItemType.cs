using System.ComponentModel;

namespace EFCore.UtilExtensions.Tests.Enums;

public enum ItemType
{
    [Description("Physical2")] // can have Desc different then enum item
    Physical = 1,

    [Description("Digital")]
    Digital = 2
}