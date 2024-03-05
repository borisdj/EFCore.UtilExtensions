using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EFCore.UtilExtensions.Annotations;
using EFCore.UtilExtensions.AuditInfo;
using EFCore.UtilExtensions.Tests.Enums;
using EFCore.UtilExtensions.Entity;

namespace EFCore.UtilExtensions.Tests.Entities;

//[Table(nameof(Item), Schema = Schema.dbo)] // remains default so no need for explicit config
public class Item : IEntityId, IAuditOwned, ISoftDelete
{
    // PK in Entity class is named only 'Id' (is in its Entity Item.Id),
    // but in DB column is named TableNameId, in this case 'ItemId' (practical convention for better readability of sql joins).
    [Column(nameof(Item) + nameof(Id))]
    public Guid Id { get; set; }

    [MaxLength(255)]
    [IndexExtension]
    public string Name { get; set; } = null!;

    [MaxLength(10)]
    [UniqueIndex]
    public string Code { get; set; } = null!;

    [Column("Description")]
    public string? CustomDescription { get; set; }

    [Precision(20, 4)]
    public decimal? Price { get; set; }

    [DefaultValue(true)]
    public bool Active { get; set; }

    [ForeignKeyExtension(DeleteBehavior.NoAction)]
    public int ItemTypeId { get; set; }
    public virtual ItemType ItemType { get; set; } = null!;

    [ForeignKeyExtension(DeleteBehavior.NoAction)]
    public Guid? ItemCategoryId { get; set; }
    public virtual ItemCategory? Category { get; set; }

    public ICollection<ItemDetail> ItemDetails { get; set; } = null!;

    public Audit? Audit { get; set; }
    public bool Deleted { get; set; }
}