using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Squora.Forms.Blazor.Components;
using Squora.Forms.Blazor.Components.Factories;
using Squora.Forms.Core.Models;

namespace Squora.Forms.Tests
{
    /// <summary>The runtime draws plain forms; an editor registers an <see cref="IFormEditSurface"/> to draw the parts it needs.</summary>
    public class EditSurfaceTests
    {
        private class Element : FormComponentBase
        {
            public Element(DynamicFormModel model) { FormModel = model; }
            public override string ValidationString => string.Empty;
        }

        // what an editor brings: its own wrapper (derived from the runtime's) and its own placeholder
        private class EditorWrapper : ElementWrapperBase
        {
            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenElement(0, "section");
                builder.AddAttribute(1, "class", "editor " + ElementClasses);
                builder.AddContent(2, ChildContent);
                builder.CloseElement();
            }
        }

        private class EditorPlaceholder : ComponentBase
        {
            [Parameter] public FormComponentBase Container { get; set; } = null!;

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenElement(0, "aside");
                builder.AddContent(1, "drop into " + Container.FormModel.DynamicType.ComponentType);
                builder.CloseElement();
            }
        }

        private class Surface : IFormEditSurface
        {
            public Type ElementWrapperType => typeof(EditorWrapper);
            public Type EmptyLayoutType => typeof(EditorPlaceholder);
        }

        private static async Task<string> Render<TComponent>(bool withSurface, Dictionary<string, object?> parameters) where TComponent : IComponent
        {
            var services = new ServiceCollection();
            if (withSurface) services.AddSingleton<IFormEditSurface>(new Surface());
            await using var provider = services.BuildServiceProvider();
            await using var renderer = new HtmlRenderer(provider, NullLoggerFactory.Instance);
            return await renderer.Dispatcher.InvokeAsync(async () =>
                (await renderer.RenderComponentAsync<TComponent>(ParameterView.FromDictionary(parameters))).ToHtmlString());
        }

        private static Dictionary<string, object?> WrapperParameters(FormComponentBase element) => new()
        {
            ["Class"] = "stringcomponent",
            ["FormComponent"] = element,
            ["ChildContent"] = (RenderFragment)(b => b.AddContent(0, "child")),
        };

        [Test]
        public async Task WithoutAnEditor_AnElementIsInThePlainWrapper()
        {
            // Class reaches the component as a parameter, set by the runtime from the form definition
            var element = new Element(FormFactory.StringComponent("Name", "$.name", b => b.WithClass("highlight"))) { Class = "highlight" };

            var html = await Render<BDFormComponentWrapper>(false, WrapperParameters(element));

            Assert.That(html, Does.Contain("class=\"stringcomponent highlight\""));
            Assert.That(html, Does.Contain("child"));
            Assert.That(html, Does.Not.Contain("editor"));
        }

        [Test]
        public async Task WithoutAnEditor_EditModeChangesNothing()
        {
            var element = new Element(FormFactory.StringComponent("Name", "$.name")) { EditMode = true };

            var html = await Render<BDFormComponentWrapper>(false, WrapperParameters(element));

            Assert.That(html, Does.Not.Contain("bd-editable"));
            Assert.That(html, Does.Not.Contain("section"));
        }

        [Test]
        public async Task AnEditorSurface_ReplacesTheWrapperOfEveryElement()
        {
            var element = new Element(FormFactory.StringComponent("Name", "$.name", b => b.WithClass("highlight"))) { Class = "highlight" };

            var html = await Render<BDFormComponentWrapper>(true, WrapperParameters(element));

            Assert.That(html, Does.Contain("<section class=\"editor stringcomponent highlight\">child</section>"));
        }

        [Test]
        public async Task AnEmptyLayout_ShowsNothingWithoutAnEditor()
        {
            var layout = new Element(FormFactory.VerticalLayout());

            var html = await Render<BDEmptyDropZone>(false, new() { ["Container"] = layout });

            Assert.That(html, Is.Empty);
        }

        [Test]
        public async Task AnEmptyLayout_ShowsTheEditorsPlaceholder()
        {
            var layout = new Element(FormFactory.VerticalLayout());

            var html = await Render<BDEmptyDropZone>(true, new() { ["Container"] = layout });

            Assert.That(html, Is.EqualTo("<aside>drop into VerticalLayout</aside>"));
        }

        [Test]
        public void InARow_AnElementTakesItsShareOfTheRowAfterTheGaps()
        {
            var narrow = FormFactory.StringComponent("Nr", "$.nr", b => b.WithWidth("10%"));
            var wide = FormFactory.StringComponent("Description", "$.d", b => b.WithWidth("90%"));
            var row = new Element(FormFactory.HorizontalLayout(narrow, wide));

            var style = new Element(narrow) { Parent = row, Width = "10%" }.ItemStyle;

            Assert.That(style, Does.Contain("calc((100% - 1 * var(--bd-gap, 16px)) * 0.1)"));
        }

        [Test]
        public void InARow_AFixedWidthKeepsItsSize()
        {
            var fixedWidth = FormFactory.StringComponent("Number", "$.n", b => b.WithWidth("120px"));
            var row = new Element(FormFactory.HorizontalLayout(fixedWidth, FormFactory.StringComponent("Street", "$.s")));

            Assert.That(new Element(fixedWidth) { Parent = row, Width = "120px" }.ItemStyle, Does.Contain("flex: 0 0 120px"));
        }

        [Test]
        public void InARowWithWidths_ElementsWithoutOneTakeWhatIsLeft()
        {
            var sized = FormFactory.StringComponent("Code", "$.c", b => b.WithWidth("25%"));
            var plain = FormFactory.StringComponent("Name", "$.n");
            var row = new Element(FormFactory.HorizontalLayout(sized, plain));

            Assert.That(new Element(plain) { Parent = row }.ItemStyle, Does.Contain("flex: 1 1 0%"));
        }

        [Test]
        public void OutsideARow_AWidthDoesNothing()
        {
            var sized = FormFactory.StringComponent("Code", "$.c", b => b.WithWidth("25%"));
            var column = new Element(FormFactory.VerticalLayout(sized));

            Assert.That(new Element(sized) { Parent = column, Width = "25%" }.ItemStyle, Is.Null);
        }

        [Test]
        public void AWidthThatIsNotANumberWithAUnit_IsNotLetIntoTheStyle()
        {
            var evil = FormFactory.StringComponent("X", "$.x", b => b.WithWidth("10%; background: url(x)"));
            var row = new Element(FormFactory.HorizontalLayout(evil));

            Assert.That(new Element(evil) { Parent = row, Width = "10%; background: url(x)" }.ItemStyle, Is.Null);
        }
    }
}
