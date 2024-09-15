namespace EFCore.UtilExtensions.AuditInfo;

public interface ISoftDelete
{
    bool Deleted { get; set; }
}