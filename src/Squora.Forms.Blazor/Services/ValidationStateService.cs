namespace Squora.Forms.Blazor.Services;

public class ValidationStateService
{
    public event Action? ValidationRequested;

    public void RequestValidation()
    {
        ValidationRequested?.Invoke();
    }
}
