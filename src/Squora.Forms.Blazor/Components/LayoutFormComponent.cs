namespace Squora.Forms.Blazor.Components
{
    public abstract class LayoutFormComponent : FormComponentBase
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();
            AllowElements = true;
        }

    }
}
