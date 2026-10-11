namespace Squora.Forms.Core.Models
{
    public class ComponentsList : Dictionary<ComponentSelectionKey, Type>
    {
        public ComponentsList()
        {

        }

        public ComponentsList(ComponentsList components)
        {
            components.ToList().ForEach(component => { Add(component.Key, component.Value); });
        }
    }
}
