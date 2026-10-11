using System.Diagnostics;
using System.Text.Json;
using Squora.Forms.Hub.Api.Models.Linting;

namespace Squora.Forms.Hub.Api.Services.Linting;

/// <summary>
/// Evaluates lint rules against a form definition body.
/// Built-in rules are hard-coded; custom rules use JSON DSL (v1).
/// </summary>
public class LintEngine
{
    private static readonly List<LintRule> BuiltInRules = new()
    {
        new() { Id = "REQUIRED_FIELD_NO_LABEL", DisplayName = "Required field missing label", Severity = "Error", IsBuiltIn = true, IsPlatformMandatory = false, IsEnabled = true, RemediationMessage = "Add a label to all required fields." },
        new() { Id = "PII_FIELD_NOT_MARKED", DisplayName = "PII field not marked", Severity = "Error", IsBuiltIn = true, IsPlatformMandatory = false, IsEnabled = true, RemediationMessage = "Add x-pii: true annotation to fields containing personal data." },
        new() { Id = "WCAG_MISSING_ALT_TEXT", DisplayName = "Image missing alt text", Severity = "Error", IsBuiltIn = true, IsPlatformMandatory = false, IsEnabled = true, RemediationMessage = "Add alt text to all image elements for accessibility." },
        new() { Id = "SCHEMA_EMPTY_FORM", DisplayName = "Empty form definition", Severity = "Warning", IsBuiltIn = true, IsPlatformMandatory = false, IsEnabled = true, RemediationMessage = "Add at least one field to the form." },
        new() { Id = "SCHEMA_DUPLICATE_FIELD_ID", DisplayName = "Duplicate field ID", Severity = "Error", IsBuiltIn = true, IsPlatformMandatory = false, IsEnabled = true, RemediationMessage = "Ensure all field IDs are unique within the form." },
        new() { Id = "NAMING_FIELD_ID_INVALID_CHARS", DisplayName = "Field ID contains invalid characters", Severity = "Warning", IsBuiltIn = true, IsPlatformMandatory = false, IsEnabled = true, RemediationMessage = "Use only lowercase letters, numbers, and hyphens in field IDs." },
    };

    public LintReport Evaluate(string formId, int formVersion, JsonElement formBody, IEnumerable<LintRule>? customRules = null)
    {
        var sw = Stopwatch.StartNew();
        var violations = new List<LintViolation>();
        var skipped = new List<string>();

        var allRules = BuiltInRules.Where(r => r.IsEnabled).ToList();
        if (customRules != null)
            allRules.AddRange(customRules.Where(r => r.IsEnabled));

        foreach (var rule in allRules)
        {
            try
            {
                var ruleViolations = EvaluateRule(rule, formBody);
                violations.AddRange(ruleViolations);
            }
            catch
            {
                skipped.Add(rule.Id);
            }
        }

        sw.Stop();
        var hasErrors = violations.Any(v => v.Severity == "Error");

        return new LintReport
        {
            FormId = formId,
            FormVersion = formVersion,
            Passed = !hasErrors,
            Violations = violations,
            SkippedRules = skipped,
            DurationMs = (int)sw.ElapsedMilliseconds
        };
    }

    private static List<LintViolation> EvaluateRule(LintRule rule, JsonElement formBody)
    {
        // Built-in rule evaluation based on rule ID
        return rule.Id switch
        {
            "SCHEMA_EMPTY_FORM" => EvaluateEmptyForm(rule, formBody),
            // Other built-in rules would inspect the form body structure
            // Custom rules (JSON DSL) would be parsed and evaluated here
            _ => new List<LintViolation>() // Default: no violations for unimplemented rules
        };
    }

    private static List<LintViolation> EvaluateEmptyForm(LintRule rule, JsonElement formBody)
    {
        if (formBody.ValueKind == JsonValueKind.Object &&
            (!formBody.TryGetProperty("properties", out var props) || props.EnumerateObject().Count() == 0) &&
            (!formBody.TryGetProperty("elements", out var elems) || elems.GetArrayLength() == 0))
        {
            return new List<LintViolation>
            {
                new() { RuleId = rule.Id, Severity = rule.Severity, Message = rule.RemediationMessage }
            };
        }
        return new List<LintViolation>();
    }
}
