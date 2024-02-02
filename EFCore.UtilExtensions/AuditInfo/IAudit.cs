using System;

namespace EFCore.UtilExtensions.AuditInfo;

public interface IAudit
{
    string? ChangedBy { get; set; }
    DateTime? ChangedTime { get; set; }

    int? RowVersion { get; set; }
    string? ChangeHistory { get; set; }
}

public interface IAuditCreate
{
    string? CreatedBy { get; set; }
    DateTime? CreatedTime { get; set; }
}

public interface IAuditExt : IAuditCreate, IAudit // includes Create Props directly (Ext - Extended)
{
}