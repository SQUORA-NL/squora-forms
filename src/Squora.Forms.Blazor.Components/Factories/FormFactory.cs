using Squora.Forms.Blazor.Components.Builders;
using Squora.Forms.Common.Enums;
using Newtonsoft.Json.Linq;
using Squora.Forms.Core.Models.ParameterModels;
using Squora.Forms.Core.Models;

namespace Squora.Forms.Blazor.Components.Factories
{
    public static partial class FormFactory
    {
        // Vertical Layout Methods
        public static DynamicFormModel VerticalLayout()
        {
            return new DynamicFormModel(ComponentType.VerticalLayout);
        }

        public static DynamicFormModel VerticalLayout(string typeDefinionName)
        {
            return new DynamicFormModel(ComponentType.VerticalLayout, typeDefinionName);
        }

        public static DynamicFormModel VerticalLayout(params DynamicFormModel[] elements)
        {
            return new DynamicFormModel()
            {
                DynamicType = new ComponentSelectionKey(ComponentType.VerticalLayout),
                Elements = elements.ToList()
            };
        }

        public static VerticalLayoutBuilder VerticalLayoutBuilder()
        {
            return new VerticalLayoutBuilder();
        }

        public static DynamicFormModel VerticalLayout(Action<VerticalLayoutBuilder>? configure)
        {
            var builder = new VerticalLayoutBuilder();
            if (configure != null) { configure(builder); }
            return builder.Build();
        }

        // horizontallayout

        public static DynamicFormModel HorizontalLayout()
        {
            return new DynamicFormModel(ComponentType.HorizontalLayout);
        }

        public static DynamicFormModel HorizontalLayout(string typeDefinionName)
        {
            return new DynamicFormModel(ComponentType.HorizontalLayout, typeDefinionName);
        }

        public static DynamicFormModel HorizontalLayout(Action<HorizontalLayoutBuilder>? configure)
        {
            var builder = new HorizontalLayoutBuilder();
            if (configure != null) { configure(builder); }
            return builder.Build();
        }

        public static HorizontalLayoutBuilder HorizontalLayoutBuilder()
        {
            return new HorizontalLayoutBuilder();
        }

        public static DynamicFormModel HorizontalLayout(params DynamicFormModel[] elements)
        {
            return new DynamicFormModel()
            {
                DynamicType = new ComponentSelectionKey(ComponentType.HorizontalLayout),
                Elements = elements.ToList()
            };
        }


        // Group Layout Methods
        public static DynamicFormModel GroupLayout()
        {
            return new DynamicFormModel(ComponentType.GroupLayout);
        }

        public static DynamicFormModel GroupLayout(string typeDefinionName)
        {
            return new DynamicFormModel(ComponentType.GroupLayout, typeDefinionName);
        }

        public static DynamicFormModel GroupLayout(params DynamicFormModel[] elements)
        {
            return new DynamicFormModel()
            {
                DynamicType = new ComponentSelectionKey(ComponentType.GroupLayout),
                Elements = elements.ToList()
            };
        }

        public static DynamicFormModel GroupLayout(Action<GroupLayoutBuilder>? configure)
        {
            var builder = new GroupLayoutBuilder();
            if (configure != null) { configure(builder); }
            return builder.Build();
        }

        public static DynamicFormModel GroupLayout(Action<GroupLayoutBuilder>? configure, params DynamicFormModel[] elements)
        {
            var builder = new GroupLayoutBuilder();
            if (configure != null) { configure(builder); }
            var result = builder.Build();
            result.Elements = elements.ToList();
            return result;
        }

        public static GroupLayoutBuilder GroupLayoutBuilder()
        {
            return new GroupLayoutBuilder();
        }


        // extention
        public static DynamicFormModel WithTypeDefinionName(this DynamicFormModel model, string typeDefinition)
        {
            model.DynamicType = new ComponentSelectionKey(model.DynamicType.ComponentType, typeDefinition);
            return model;
        }
        public static DynamicFormModel AddSchemaRule(this DynamicFormModel model, RuleEffect ruleEffect, string scope, string jsonSchema)
        {
            return model.AddSchemaRule(ruleEffect, scope, JObject.Parse(jsonSchema));
        }

        public static DynamicFormModel AddSchemaRule(this DynamicFormModel model, RuleEffect ruleEffect, string scope, JToken jsonSchema)
        {
            if(model.Rules == null) { model.Rules = new List<DynamicFormModelRule>();  }
            model.Rules.Add(new DynamicFormModelRule(ruleEffect, new DynamicFormModelRuleCondition(scope, jsonSchema)));
            return model;
        }

        // Counter component

        public static DynamicFormModel NumberDisplay(string label, string path)
        {
            return new NumberDisplayBuilder()
                .WithLabel(label)
                .WithPath(path)
                .Build();
        }

        public static DynamicFormModel TemplateDisplay(string typeDefinionName, string path)
        {
            return new TemplateDisplayBuider()
                .WithTypeDefinition(typeDefinionName)
                .WithPath(path)
                .Build();
        }

        public static DynamicFormModel StringDisplay(string label, string path)
        {
            return new StringDisplayBuilder()
                .WithLabel(label)
                .WithPath(path)
                .Build();
        }


        // String Component Methods
        public static DynamicFormModel StringComponent()
        {
            return new DynamicFormModel(ComponentType.String);
        }

        public static DynamicFormModel StringComponent(string label, string path)
        {
            return new StringComponentBuilder()
                .WithLabel(label)
                .WithPath(path)
                .Build();
        }

        public static DynamicFormModel StringComponent(string typeDefinionName)
        {
            return new DynamicFormModel(ComponentType.String, typeDefinionName);
        }

        public static DynamicFormModel StringComponent(string label, string path, Action<StringComponentBuilder>? configure = null)
        {
            var builder = new StringComponentBuilder();
            if (configure != null) { configure(builder); }
            builder.WithPath(path);
            builder.WithLabel(label);
            return builder.Build();
        }

        public static StringComponentBuilder StringComponentBuilder(string path)
        {
            return new StringComponentBuilder().WithPath(path);
        }

        // Boolean Component Methods
        public static DynamicFormModel BooleanComponent()
        {
            return new DynamicFormModel(ComponentType.Boolean);
        }

        public static DynamicFormModel BooleanComponent(string typeDefinionName)
        {
            return new DynamicFormModel(ComponentType.Boolean, typeDefinionName);
        }

        public static DynamicFormModel BoolComponent(string label, string path)
        {
            return new DynamicFormModel()
            {
                DynamicType = new ComponentSelectionKey(ComponentType.Boolean),
                Parameters = BooleanParameters.Set(label, path)
            };
        }

        public static DynamicFormModel BoolComponent(string typeDefinionName, string label, string path)
        {
            return new DynamicFormModel()
            {
                DynamicType = new ComponentSelectionKey(ComponentType.Boolean, typeDefinionName),
                Parameters = BooleanParameters.Set(label, path)
            };
        }

        public static DynamicFormModel BooleanComponent(string label, string path, Action<BooleanComponentBuilder>? configure = null)
        {
            var builder = new BooleanComponentBuilder();
            if (configure != null) { configure(builder); }
            builder.WithPath(path);
            builder.WithLabel(label);
            return builder.Build();
        }


        public static BooleanComponentBuilder BooleanComponentBuilder()
        {
            return new BooleanComponentBuilder();
        }

        // DateTime Component Methods
        public static DynamicFormModel DateTimeComponent()
        {
            return new DynamicFormModel(ComponentType.DateTime);
        }

        public static DynamicFormModel DateTimeComponent(string typeDefinionName)
        {
            return new DynamicFormModel(ComponentType.DateTime, typeDefinionName);
        }

        public static DynamicFormModel DateTimeComponent(string label, string path)
        {
            return new DynamicFormModel()
            {
                DynamicType = new ComponentSelectionKey(ComponentType.DateTime),
                Parameters = DeleteActionParameters.Set(label, path)
            };
        }

        public static DynamicFormModel DateTimeComponent(string label, string path, Action<DateTimeComponentBuilder>? configure = null)
        {
            var builder = new DateTimeComponentBuilder();
            if (configure != null) { configure(builder); }
            builder.WithPath(path);
            builder.WithLabel(label);
            return builder.Build();
        }

        public static DateTimeComponentBuilder DateTimeComponentBuilder()
        {
            return new DateTimeComponentBuilder();
        }

        // Number Component Methods
        public static DynamicFormModel NumberComponent()
        {
            return new DynamicFormModel(ComponentType.Number);
        }

        public static DynamicFormModel NumberComponent(string typeDefinionName)
        {
            return new DynamicFormModel(ComponentType.Number, typeDefinionName);
        }

        public static DynamicFormModel NumberComponent(string label, string path)
        {
            return new DynamicFormModel()
            {
                DynamicType = new ComponentSelectionKey(ComponentType.Number),
                Parameters = BaseParameters.Set(label, path)
            };
        }

        public static DynamicFormModel NumberComponent(string label, string path, Action<NumberComponentBuilder>? configure)
        {
            var builder = new NumberComponentBuilder();
            if (configure != null) { configure(builder); }
            builder.WithPath(path);
            builder.WithLabel(label);
            return builder.Build();
        }

        public static NumberComponentBuilder NumberComponentBuilder()
        {
            return new NumberComponentBuilder();
        }

        // Int Component Methods
        public static DynamicFormModel IntComponent()
        {
            return new DynamicFormModel(ComponentType.Int);
        }

        public static DynamicFormModel IntComponent(string typeDefinionName)
        {
            return new DynamicFormModel(ComponentType.Int, typeDefinionName);
        }

        public static DynamicFormModel IntComponent(string label, string path)
        {
            return new DynamicFormModel()
            {
                DynamicType = new ComponentSelectionKey(ComponentType.Int),
                Parameters = BaseParameters.Set(label, path)
            };
        }


        public static DynamicFormModel IntComponent(string typeDefinionName, string label, string path)
        {
            return new DynamicFormModel()
            {
                DynamicType = new ComponentSelectionKey(ComponentType.Int, typeDefinionName),
                Parameters = BaseParameters.Set(label, path)
            };
        }

        public static DynamicFormModel IntComponent(string label, string path, Action<IntComponentBuilder>? configure = null)
        {
            var builder = new IntComponentBuilder();
            if (configure != null) { configure(builder); }
            builder.WithPath(path);
            builder.WithLabel(label);
            return builder.Build();
        }


        public static IntComponentBuilder IntComponentBuilder()
        {
            return new IntComponentBuilder();
        }

        // List Component Methods
        public static DynamicFormModel ListComponent()
        {
            return new DynamicFormModel(ComponentType.List);
        }

        public static DynamicFormModel ListComponent(string typeDefinionName)
        {
            return new DynamicFormModel(ComponentType.List, typeDefinionName);
        }

        public static DynamicFormModel ListComponent(string label, string path, params DynamicFormModel[] elements)
        {
            return new DynamicFormModel()
            {
                DynamicType = new ComponentSelectionKey(ComponentType.List),
                Parameters = BaseParameters.Set(label, path),
                Elements = elements.ToList()
            };
        }

        public static DynamicFormModel ListComponent(string label, string path, Action<ListComponentBuilder>? configure = null)
        {
            var builder = new ListComponentBuilder();
            if (configure != null) { configure(builder); }
            builder.WithPath(path);
            builder.WithLabel(label);
            return builder.Build();
        }

        public static ListComponentBuilder ListComponentBuilder()
        {
            return new ListComponentBuilder();
        }

        // Dropdown Component Methods
        public static DynamicFormModel DropDownComponent()
        {
            return new DynamicFormModel(ComponentType.Dropdown);
        }

        public static DynamicFormModel DropDownComponent(string typeDefinionName)
        {
            return new DynamicFormModel(ComponentType.Dropdown, typeDefinionName);
        }

        public static DynamicFormModel DropDownComponent(string label, string path, Dictionary<object, string> options)
        {
            return new DynamicFormModel()
            {
                DynamicType = new ComponentSelectionKey(ComponentType.Dropdown),
                Parameters = DropdownParameters.Set(label, options, path)
            };
        }

        public static DynamicFormModel DropDownComponent(string label, Dictionary<object, string> options, string path, Action<DropDownComponentBuilder>? configure = null)
        {
            var builder = new DropDownComponentBuilder();
            if (configure != null) { configure(builder); }
            builder.WithPath(path);
            builder.WithLabel(label);
            builder.WithOptions(options);
            return builder.Build();
        }

        public static DropDownComponentBuilder DropDownComponentBuilder()
        {
            return new DropDownComponentBuilder();
        }

        // Action Component Methods
        public static DynamicFormModel DeleteAction(string label, string path)
        {
            return new DynamicFormModel()
            {
                DynamicType = new ComponentSelectionKey(ComponentType.DeleteAction),
                Parameters = DeleteActionParameters.Set(label, path)
            };
        }

        public static DynamicFormModel AddAction(string label, string path, object defaultValue)
        {
            return new DynamicFormModel()
            {
                DynamicType = new ComponentSelectionKey(ComponentType.AddAction),
                Parameters = AddActionParameters.Set(label, path, defaultValue)
            };
        }

        public static DynamicFormModel SubmitAction(string label)
        {
            return new DynamicFormModel()
            {
                DynamicType = new ComponentSelectionKey(ComponentType.SubmitAction),
                Parameters = SubmitActionParameters.Set(label)
            };
        }
    }
}
