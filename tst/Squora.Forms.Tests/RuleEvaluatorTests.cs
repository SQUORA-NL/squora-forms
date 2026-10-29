using Newtonsoft.Json.Linq;
using Squora.Forms.Blazor.Utillities;
using Squora.Forms.Common.Enums;
using Squora.Forms.Core.Models;

namespace Squora.Forms.Tests;

public class RuleEvaluatorTests
{
    private static DynamicFormModelRule Rule(RuleEffect effect, string scope, string schema) =>
        new(effect, new DynamicFormModelRuleCondition(scope, JToken.Parse(schema)));

    private static readonly string IsFalse = "{ \"const\": false }";

    [Test]
    public void No_rules_means_shown_and_enabled()
    {
        Assert.That(RuleEvaluator.Resolve(null, new { isActive = true }), Is.EqualTo(RuleEffect.SHOW));
        Assert.That(RuleEvaluator.Resolve(new List<DynamicFormModelRule>(), new { isActive = true }), Is.EqualTo(RuleEffect.SHOW));
    }

    [TestCase(false, RuleEffect.HIDE)]
    [TestCase(true, RuleEffect.SHOW)]
    public void Hide_applies_when_the_condition_matches(bool isActive, RuleEffect expected)
    {
        var rules = new[] { Rule(RuleEffect.HIDE, "$.isActive", IsFalse) };

        Assert.That(RuleEvaluator.Resolve(rules, new { isActive }), Is.EqualTo(expected));
    }

    [TestCase(false, RuleEffect.SHOW)]
    [TestCase(true, RuleEffect.HIDE)]
    public void Show_keeps_the_component_visible_only_while_the_condition_matches(bool isActive, RuleEffect expected)
    {
        var rules = new[] { Rule(RuleEffect.SHOW, "$.isActive", IsFalse) };

        Assert.That(RuleEvaluator.Resolve(rules, new { isActive }), Is.EqualTo(expected));
    }

    [TestCase(false, RuleEffect.DISABLE)]
    [TestCase(true, RuleEffect.SHOW)]
    public void Disable_applies_when_the_condition_matches(bool isActive, RuleEffect expected)
    {
        var rules = new[] { Rule(RuleEffect.DISABLE, "$.isActive", IsFalse) };

        Assert.That(RuleEvaluator.Resolve(rules, new { isActive }), Is.EqualTo(expected));
    }

    [TestCase(false, RuleEffect.SHOW)]
    [TestCase(true, RuleEffect.DISABLE)]
    public void Enable_keeps_the_component_enabled_only_while_the_condition_matches(bool isActive, RuleEffect expected)
    {
        var rules = new[] { Rule(RuleEffect.ENABLE, "$.isActive", IsFalse) };

        Assert.That(RuleEvaluator.Resolve(rules, new { isActive }), Is.EqualTo(expected));
    }

    [Test]
    public void The_same_rule_and_value_can_be_resolved_more_often_than_the_schema_library_allows_without_a_licence()
    {
        // a form resolves its rules on every render: the library stops validating after 1000 an hour
        var rules = new[] { Rule(RuleEffect.HIDE, "$.isActive", IsFalse) };

        for (var i = 0; i < 1500; i++)
        {
            Assert.That(RuleEvaluator.Resolve(rules, new { isActive = false }), Is.EqualTo(RuleEffect.HIDE));
        }
    }

    [Test]
    public void Hidden_wins_over_disabled()
    {
        var rules = new[]
        {
            Rule(RuleEffect.DISABLE, "$.isActive", IsFalse),
            Rule(RuleEffect.HIDE, "$.isActive", IsFalse),
        };

        Assert.That(RuleEvaluator.Resolve(rules, new { isActive = false }), Is.EqualTo(RuleEffect.HIDE));
    }

    [Test]
    public void A_missing_value_never_matches()
    {
        var hide = new[] { Rule(RuleEffect.HIDE, "$.isActive", IsFalse) };
        var show = new[] { Rule(RuleEffect.SHOW, "$.isActive", IsFalse) };
        var data = new { other = 1 };

        Assert.That(RuleEvaluator.Resolve(hide, data), Is.EqualTo(RuleEffect.SHOW));
        Assert.That(RuleEvaluator.Resolve(show, data), Is.EqualTo(RuleEffect.HIDE));
        Assert.That(RuleEvaluator.Resolve(hide, null), Is.EqualTo(RuleEffect.SHOW));
    }

    [Test]
    public void A_condition_that_cannot_be_evaluated_does_not_match()
    {
        var rules = new[] { Rule(RuleEffect.HIDE, "$.count", "{ \"type\": \"not-a-type\" }") };

        Assert.That(RuleEvaluator.Resolve(rules, new { count = 3 }), Is.EqualTo(RuleEffect.SHOW));
    }

    [Test]
    public void Ui_schema_style_scopes_are_supported()
    {
        var rules = new[] { Rule(RuleEffect.HIDE, "#/properties/counter", "{ \"const\": 10 }") };

        Assert.That(RuleEvaluator.Resolve(rules, new { counter = 10 }), Is.EqualTo(RuleEffect.HIDE));
        Assert.That(RuleEvaluator.Resolve(rules, new { counter = 3 }), Is.EqualTo(RuleEffect.SHOW));
    }
}
