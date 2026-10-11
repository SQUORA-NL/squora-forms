using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Squora.Forms.Hub.Api.Models.Linting;

public class LintRule
{
    [BsonId]
    public string Id { get; set; } = ""; // e.g. REQUIRED_FIELD_NO_LABEL

    public string? WorkspaceId { get; set; } // null for built-in rules
    public string DisplayName { get; set; } = "";
    public string Severity { get; set; } = "Error"; // Error | Warning | Info
    public string ConditionExpression { get; set; } = ""; // JSON DSL
    public string RemediationMessage { get; set; } = "";
    public string? DocsUrl { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool IsPlatformMandatory { get; set; }
    public bool IsBuiltIn { get; set; }
}

public class LintViolation
{
    public string RuleId { get; set; } = "";
    public string Severity { get; set; } = "";
    public string? AffectedElementId { get; set; }
    public string Message { get; set; } = "";
    public string? RemediationHint { get; set; }
}

public class LintReport
{
    public string FormId { get; set; } = "";
    public int FormVersion { get; set; }
    public DateTime RunAt { get; set; } = DateTime.UtcNow;
    public bool Passed { get; set; }
    public List<LintViolation> Violations { get; set; } = new();
    public List<string> SkippedRules { get; set; } = new();
    public int DurationMs { get; set; }
}
