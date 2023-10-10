using System;

namespace EFCore.UtilExtensions.Audit;

public interface IAudit
{
    string ChangedBy { get; set; }
    DateTime? ChangedTime { get; set; }

    int RowVersion { get; set; }
    string ChangeHistory { get; set; }
}

public interface IAuditCreate
{
    string CreatedBy { get; set; }
    DateTime? CreatedTime { get; set; }
}

public interface IAuditFull // includes Create Props directly 
{
    string CreatedBy { get; set; }
    DateTime? CreatedTime { get; set; }

    string ChangedBy { get; set; }
    DateTime? ChangedTime { get; set; }

    int RowVersion { get; set; }
    string ChangeHistory { get; set; }
}