using NUnit.Framework;
using System.Text.Json;
using Squora.Forms.Hub.Api.Services.Linting;

namespace Squora.Forms.Hub.Api.Tests.Services;

[TestFixture]
public class LintEngineTests
{
    private LintEngine _sut = null!;

    [SetUp]
    public void SetUp() => _sut = new LintEngine();

    [Test]
    public void Evaluate_EmptyForm_ReturnsSchemaEmptyFormViolation()
    {
        var body = JsonDocument.Parse("{}").RootElement;
        var report = _sut.Evaluate("test-form", 1, body);

        // SCHEMA_EMPTY_FORM is a Warning severity, so Passed is still true (only Errors fail)
        Assert.That(report.Violations, Has.Some.Property("RuleId").EqualTo("SCHEMA_EMPTY_FORM"));
        Assert.That(report.Violations.First(v => v.RuleId == "SCHEMA_EMPTY_FORM").Severity, Is.EqualTo("Warning"));
    }

    [Test]
    public void Evaluate_FormWithProperties_Passes()
    {
        var body = JsonDocument.Parse("{\"properties\": {\"name\": {\"type\": \"string\"}}}").RootElement;
        var report = _sut.Evaluate("test-form", 1, body);

        Assert.That(report.Violations.Where(v => v.RuleId == "SCHEMA_EMPTY_FORM"), Is.Empty);
    }

    [Test]
    public void Evaluate_FormWithElements_Passes()
    {
        var body = JsonDocument.Parse("{\"elements\": [{\"type\": \"text\"}]}").RootElement;
        var report = _sut.Evaluate("test-form", 1, body);

        Assert.That(report.Violations.Where(v => v.RuleId == "SCHEMA_EMPTY_FORM"), Is.Empty);
    }

    [Test]
    public void Evaluate_RecordsDurationMs()
    {
        var body = JsonDocument.Parse("{}").RootElement;
        var report = _sut.Evaluate("test-form", 1, body);

        Assert.That(report.DurationMs, Is.GreaterThanOrEqualTo(0));
    }

    [Test]
    public void Evaluate_SetsFormIdAndVersion()
    {
        var body = JsonDocument.Parse("{}").RootElement;
        var report = _sut.Evaluate("my-form", 5, body);

        Assert.That(report.FormId, Is.EqualTo("my-form"));
        Assert.That(report.FormVersion, Is.EqualTo(5));
    }
}
