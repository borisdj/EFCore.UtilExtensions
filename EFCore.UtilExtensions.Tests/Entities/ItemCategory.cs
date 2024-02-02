using EFCore.UtilExtensions.Annotations;
using EFCore.UtilExtensions.AuditInfo;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.UtilExtensions.Tests.Entities;

//[Index(nameof(Name), nameof(Number), IsUnique = true, )] // Alternatively for multiple Index
//[Index(new string[] { nameof(Name), nameof(Number) }, IsUnique = true, Name = "ItemCategory_Unique_Index")] // Alternatively for multiple Index
public partial class ItemCategory : IEntityId
{
    //[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] // not required since property named 'TableNameId' or just 'Id' / 'ID' is PK-PrimaryKey and Identity(AutoIncrement) by convention (default)
    [Column(nameof(ItemCategory) + nameof(Id))]
    public Guid Id { get; set; }

    //[Required] // not needed, is based on property nullability
    [MaxLength(50)]
    [UniqueIndex("U1")]
    public string Name { get; set; } = null!;

    [MaxLength(10)]
    [UniqueIndex("U1")]
    public string? Number { get; set; }// = null!;
}
public partial class ItemCategory : IAudit, ISoftDelete
{
    // IAudit
    [DefaultValue("")]
    public string? ChangedBy { get; set; }// = null!;

    [DefaultValueSql("getdate()")]
    public DateTime? ChangedTime { get; set; }

    [DefaultValue(1)]
    public int? RowVersion { get; set; }
    public string? ChangeHistory { get; set; }

    //ISoftDelete
    public bool Deleted { get; set; }
}