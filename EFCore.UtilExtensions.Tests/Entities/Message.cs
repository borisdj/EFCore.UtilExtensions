using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.UtilExtensions.Tests.Entities;

[Table(nameof(Entities.Message), Schema = DbSchema.Util)] // example of different schema and Id with 'int' (autoincrement)
public class Message
{
    [System.ComponentModel.DataAnnotations.Schema.ColumnAttribute(nameof(Entities.Message) + nameof(Id))]
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? TimeCreated { get; set; }
}