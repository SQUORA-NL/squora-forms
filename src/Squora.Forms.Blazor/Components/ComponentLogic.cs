using Squora.Forms.Blazor.ObjectHandlers;

namespace Squora.Forms.Blazor.Components
{
    public class ComponentLogic
    {
        public static void UpdateValue(FormComponentBase component, object propertyValue)
        {
            if (string.IsNullOrEmpty(component.Path))
            {
                component.ValueChanged.InvokeAsync(propertyValue);
                return;
            }

            if (propertyValue != DataObjectHelper.GetValue(component.Path, component.Value))
            {
                DataObjectHelper.SetValue(component.GetInstancePath(component.Path), component.Value, propertyValue);
                component.ValueChanged.InvokeAsync(propertyValue);
                component.Validate();
            }
        }
    }
}
