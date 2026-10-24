using Squora.Forms.Common.Enums;
using Squora.Forms.Blazor.Components.Input;
using Squora.Forms.Core.Models;
using Squora.Forms.Core.Models.ParameterModels;

namespace Squora.Forms.Blazor.Components.Builders
{
    public class FileUploadComponentBuilder : FormComponentBuilder<FileUploadComponentBase, FileUploadComponentBuilder>
    {
        public FileUploadComponentBuilder WithLabel(string label)
        {
            _component.Label = label;
            return this;
        }

        public FileUploadComponentBuilder WithMultiple(bool multiple = true)
        {
            _component.Multiple = multiple;
            return this;
        }

        public FileUploadComponentBuilder WithAccept(string accept)
        {
            _component.Accept = accept;
            return this;
        }

        public FileUploadComponentBuilder WithMaximumFileSize(long bytes)
        {
            _component.MaximumFileSize = bytes;
            return this;
        }

        public FileUploadComponentBuilder WithMaximumFiles(int maximumFiles)
        {
            _component.MaximumFiles = maximumFiles;
            return this;
        }

        public new DynamicFormModel Build()
        {
            var model = base.Build();
            model.DynamicType = new ComponentSelectionKey(ComponentType.FileUpload);
            model.Parameters.Add(ParameterNames.Label, _component.Label);
            model.Parameters.Add(ParameterNames.Multiple, _component.Multiple);
            model.Parameters.Add(ParameterNames.Accept, _component.Accept);
            model.Parameters.Add(ParameterNames.MaximumFileSize, _component.MaximumFileSize);
            model.Parameters.Add(ParameterNames.MaximumFiles, _component.MaximumFiles);
            return model;
        }
    }
}
