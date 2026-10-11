using Squora.Forms.Core.Helpers;
using Squora.Forms.Blazor.Components;
using Squora.Forms.Blazor.ObjectHandlers;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace Squora.Forms.Blazor.Actions
{
    public class AddActionBase : InputFormComponent
    {
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public object DefaultValue { get; set; }

        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);

        public void AddItem()
        {
            object copyOfDefaultValue = MakeCopyOfDefaultValue(DefaultValue);
            DataObjectHelper.Add(GetInstancePath(Path), Value, copyOfDefaultValue);
            ValueChanged.InvokeAsync(null);
        }

        private object MakeCopyOfDefaultValue(object defaultValue)
        {
            if (defaultValue == null)
            {
                return null;
            }

            Type type = defaultValue.GetType();

            if (type.IsValueType || defaultValue is string)
            {
                return defaultValue;
            }

            return DeepCopyUsingSerialization(defaultValue, type);
        }

        private static object DeepCopyUsingSerialization(object obj, Type type)
        {
            if (obj == null) { return null; }
            var json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject(json, type);
        }
    }
}
