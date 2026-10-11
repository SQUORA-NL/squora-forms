using Squora.Forms.Blazor.Components;
using Squora.Forms.Blazor.Components.Input;
using Squora.Forms.Blazor.ObjectHandlers;
using Squora.Forms.Blazor.Services;

namespace Squora.Forms.Tests;

public class BlazorFormBehaviourTests
{
    private sealed class FakeComponent : FormComponentBase { public override string ValidationString => string.Empty; }

    [Test]
    public void Integer_json_values_are_converted_to_the_type_of_the_component_property()
    {
        // JSON "Minimum": 1 arrives as a long; NumberComponentBase.Minimum is a double and Blazor throws when it is set from a long.
        var all = new List<KeyValuePair<string, object>>
        {
            new("Minimum", 1L),
            new("Maximum", 500L),
            new("Label", "Seats"),
        };

        var result = RegularObjectHandler.GetParameters(typeof(NumberComponentBase), new Dictionary<string, object>(), all);

        Assert.That(result["Minimum"], Is.TypeOf<double>().And.EqualTo(1d));
        Assert.That(result["Maximum"], Is.TypeOf<double>().And.EqualTo(500d));
        Assert.That(result["Label"], Is.EqualTo("Seats"));
    }

    [Test]
    public void Values_that_already_fit_or_are_not_numbers_are_left_alone()
    {
        var all = new List<KeyValuePair<string, object>>
        {
            new("MinimumLength", 3L),   // long? property: stays a long
            new("Pattern", "[a-z]+"),
        };

        var result = RegularObjectHandler.GetParameters(typeof(StringComponentBase), new Dictionary<string, object>(), all);

        Assert.That(result["MinimumLength"], Is.TypeOf<long>().And.EqualTo(3L));
        Assert.That(result["Pattern"], Is.EqualTo("[a-z]+"));
    }

    [Test]
    public void A_form_is_valid_only_while_every_registered_component_is_valid()
    {
        var state = new ValidationStateService();
        var good = new FakeComponent();
        var bad = new FakeComponent { IsValid = false };
        state.Register(good);
        state.Register(bad);

        Assert.That(state.AllValid, Is.False);

        state.Unregister(bad); // e.g. hidden by a rule, so no longer part of the form
        Assert.That(state.AllValid, Is.True);
    }

    [Test]
    public void An_empty_form_is_valid()
    {
        Assert.That(new ValidationStateService().AllValid, Is.True);
    }
}
