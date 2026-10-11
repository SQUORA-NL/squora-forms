using Squora.Forms.Core.Models;
using Squora.Forms.Core.Models.ParameterModels;
using Squora.Forms.Blazor.Components;
using Microsoft.AspNetCore.Components;

namespace Squora.Forms.Blazor.Components.Builders
{
    public abstract class FormComponentBuilder<TComponent, TBuilder>
        where TComponent : FormComponentBase, new()
        where TBuilder : FormComponentBuilder<TComponent, TBuilder>, new()
    {
        internal string? typeDefinitionName = null;
        internal readonly TComponent _component = new TComponent();
        private readonly ParameterList _customParameters = new ParameterList();
        public TBuilder WithComponents(ComponentsList components)
        {
            _component.Components = components;
            return this as TBuilder;
        }

        public TBuilder WithTypeDefinition(string typeDefinitionName)
        {
            this.typeDefinitionName = typeDefinitionName;
            return this as TBuilder;
        }

        public TBuilder WithPath(string path)
        {
            _component.Path = path;
            return this as TBuilder;
        }

        public TBuilder WithInvalidMessage(string invalidMessage)
        {
            _component.InvalidMessage = invalidMessage;
            return this as TBuilder;
        }

        public TBuilder WithStyle(string style)
        {
            _component.Style = style;
            return this as TBuilder;
        }

        public TBuilder WithClass(string className)
        {
            _component.Class = className;
            return this as TBuilder;
        }

        public TBuilder WithParameter(string parameterName, object value)
        {
            _customParameters.Add(parameterName, value);
            return this as TBuilder;
        }

        //public TBuilder WithFormModel(DynamicFormModel formModel)
        //{
        //    _component.FormModel = formModel;
        //    return this as TBuilder;
        //}

        public TBuilder WithElements(params DynamicFormModel[] elements)
        {
            _component.FormModel.Elements.AddRange(elements);
            return this as TBuilder;
        }

        public TBuilder WithElement(DynamicFormModel element)
        {
            _component.FormModel.Elements.Add(element);
            return this as TBuilder;
        }

        public TBuilder WithValueChanged(EventCallback<object?> valueChanged)
        {
            _component.ValueChanged = valueChanged;
            return this as TBuilder;
        }

        //public FormComponentBuilder<TComponent, TBuilder> AddElement(Action<TBuilder> configure)
        //{
        //    // Create a new builder of the same type
        //    var childBuilder = new TBuilder();

        //    // Configure the child builder using the provided action
        //    configure(childBuilder);

        //    WithFormModel(childBuilder.Build());

        //    // Return the original builder
        //    return this;
        //}


        public TBuilder ConfigureFormModel(Action<DynamicFormModel> configure)
        {
            if (_component.FormModel == null)
            {
                _component.FormModel = new DynamicFormModel();
            }

            configure(_component.FormModel);
            return this as TBuilder;
        }


        public virtual DynamicFormModel Build()
        {
            var model = new DynamicFormModel();
            model.Parameters.AddRange(_customParameters);
            model.Parameters.Add(ParameterNames.Path, _component.Path);
            model.Parameters.Add(ParameterNames.InvalidMesssage, _component.InvalidMessage);
            model.Elements = _component.FormModel.Elements;
            return model;
        }
    }
}
