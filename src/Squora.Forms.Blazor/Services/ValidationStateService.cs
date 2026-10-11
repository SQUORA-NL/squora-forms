using Squora.Forms.Blazor.Components;

namespace Squora.Forms.Blazor.Services;

public class ValidationStateService
{
    private readonly List<FormComponentBase> _components = new();

    public event Action? ValidationRequested;

    /// <summary>
    /// Components currently on screen register here so a submit can ask whether the form as a whole is valid.
    /// Components that are not rendered (hidden by a rule, removed from a list) are unregistered and do not count.
    /// </summary>
    public void Register(FormComponentBase component)
    {
        lock (_components) { _components.Add(component); }
    }

    public void Unregister(FormComponentBase component)
    {
        lock (_components) { _components.Remove(component); }
    }

    /// <summary>True when no registered component is currently invalid. Call after <see cref="RequestValidation"/>.</summary>
    public bool AllValid
    {
        get
        {
            lock (_components) { return _components.All(c => c.IsValid); }
        }
    }

    public void RequestValidation()
    {
        ValidationRequested?.Invoke();
    }
}
