using Squora.Forms.Blazor.Components.Input;

namespace Squora.Forms.Tests;

public class BooleanComponentValidationTests
{
    private static bool IsValidAfterValidate(bool? checkedValue, bool? needsToBeChecked)
    {
        var component = new BooleanComponentBase { NeedsToBeChecked = needsToBeChecked, _booleanValue = checkedValue };
        component.Validate();
        return component.IsValid;
    }

    [TestCase(false, null)]
    [TestCase(false, false)]
    [TestCase(true, null)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public void A_checkbox_that_does_not_have_to_be_checked_is_valid_either_way(bool? isChecked, bool? needsToBeChecked)
    {
        Assert.That(IsValidAfterValidate(isChecked, needsToBeChecked), Is.True);
    }

    [Test]
    public void An_unchecked_checkbox_that_must_be_checked_is_invalid()
    {
        Assert.That(IsValidAfterValidate(false, true), Is.False);
    }

    [Test]
    public void A_checkbox_without_a_value_counts_as_unchecked()
    {
        Assert.That(IsValidAfterValidate(null, true), Is.False);
        Assert.That(IsValidAfterValidate(null, null), Is.True);
    }

    [Test]
    public void A_checked_checkbox_that_must_be_checked_is_valid()
    {
        Assert.That(IsValidAfterValidate(true, true), Is.True);
    }
}
