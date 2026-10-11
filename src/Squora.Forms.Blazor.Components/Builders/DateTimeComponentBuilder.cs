using Squora.Forms.Common.Enums;
using Squora.Forms.Blazor.Components.Input;
using Squora.Forms.Core.Models;
using Squora.Forms.Core.Models.ParameterModels;

namespace Squora.Forms.Blazor.Components.Builders
{
    public class DateTimeComponentBuilder : FormComponentBuilder<DateTimeComponentBase, DateTimeComponentBuilder>
    {
        public DateTimeComponentBuilder WithLabel(string label)
        {
            _component.Label = label;
            return this;
        }

        public DateTimeComponentBuilder WithMinimumDateTime(DateTime? minimumDateTime)
        {
            _component.MinimumDateTime = minimumDateTime;
            return this;
        }

        public DateTimeComponentBuilder WithMaximumDateTime(DateTime? maximumDateTime)
        {
            _component.MaximumDateTime = maximumDateTime;
            return this;
        }

        public new DynamicFormModel Build()
        {
            var model = base.Build();
            model.DynamicType = new ComponentSelectionKey(ComponentType.DateTime);
            model.Parameters.Add(ParameterNames.Label, _component.Label);
            model.Parameters.Add(ParameterNames.MinimumDateTime, _component.MinimumDateTime);
            model.Parameters.Add(ParameterNames.MaximumDateTime, _component.MaximumDateTime);
            return model;
        }
    }

}
