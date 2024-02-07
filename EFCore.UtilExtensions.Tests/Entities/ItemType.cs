using EFCore.UtilExtensions.Annotations;
using EFCore.UtilExtensions.AuditInfo;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.UtilExtensions.Tests.Entities;

[EnumType(typeof(Enums.ItemType))]
public class ItemType : IEnum
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column(nameof(ItemType) + nameof(Id))]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [MaxLength(255)]
    public string? Description { get; set; }
}