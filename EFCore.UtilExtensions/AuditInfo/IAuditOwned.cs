using EFCore.UtilExtensions.Annotations;
using Microsoft.EntityFrameworkCore;
using System;

namespace EFCore.UtilExtensions.AuditInfo;

public interface IAuditOwned
{
    Audit Audit { get; set; }
}

public interface IAuditCreateOwned
{
    AuditCreate AuditCreate { get; set; }
}

public interface IAuditExtOwned
{
    AuditExt AuditExt { get; set; }
}

[Owned]
public class Audit : IAudit // IAuditFlat (TODO consider)
{
    [DefaultValue("")]
    public string ChangedBy { get; set; } = null!;

    [DefaultValueSql("getdate()")]
    public DateTime ChangedTime { get; set; }


    [DefaultValue(1)]
    public int RowVersion { get; set; }
    public string? ChangeHistory { get; set; }
}

[Owned]
public class AuditCreate : IAuditCreate
{
    [DefaultValue("")]
    public string CreatedBy { get; set; } = null!;

    [DefaultValueSql("getdate()")]
    public DateTime CreatedTime { get; set; }
}

[Owned]
public class AuditExt : IAuditExt // Ext - Extended
{
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedTime { get; set; }

    public string ChangedBy { get; set; } = null!;
    public DateTime ChangedTime { get; set; }

    public int RowVersion { get; set; }
    public string? ChangeHistory { get; set; }
}