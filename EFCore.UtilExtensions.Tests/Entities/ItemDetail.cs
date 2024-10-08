using EFCore.UtilExtensions.Annotations;
using EFCore.UtilExtensions.AuditInfo;
using EFCore.UtilExtensions.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.UtilExtensions.Tests.Entities;

[Table(nameof(ItemDetail), Schema = "dba")] // example of different schema
public class ItemDetail : IEntityId, IAuditOwned
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)] // when NotNone, Key is generated by EF before going into Db, otherwise we explicitly generate it in memory
    [Column(nameof(ItemDetail) + nameof(Id))]
    public Guid Id { get; set; }

    public Guid ItemId { get; set; }
    [ForeignKey(nameof(ItemId))]
    public virtual Item Item { get; set; } = null!;

    //default Precision is (18, 2)
    public decimal Price { get; set; }

    public string? Remark { get; set; }

    [Column(TypeName = (nameof(DateTime)))] // Column to be of older DbType 'datetime' instead of default 'datetime2'
    [DefaultValueSql("getdate()")]
    public DateTime? TimeUpdated { get; set; }

    public Audit? Audit { get; set; }
}