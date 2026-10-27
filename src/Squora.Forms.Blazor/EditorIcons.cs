using Microsoft.AspNetCore.Components;
using Squora.Forms.Common.Enums;

namespace Squora.Forms.Blazor
{
    /// <summary>Small line icons for the editor's own controls. They take the colour of the text around them, so they work in any theme.</summary>
    public static class EditorIcons
    {
        private static MarkupString Svg(string paths) => new(
            $"<svg class=\"bd-icon\" viewBox=\"0 0 24 24\" width=\"16\" height=\"16\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\" aria-hidden=\"true\" focusable=\"false\">{paths}</svg>");

        public static MarkupString Settings => Svg("<line x1=\"4\" y1=\"7\" x2=\"20\" y2=\"7\"/><line x1=\"4\" y1=\"17\" x2=\"20\" y2=\"17\"/><circle cx=\"9\" cy=\"7\" r=\"2.2\" fill=\"currentColor\"/><circle cx=\"15\" cy=\"17\" r=\"2.2\" fill=\"currentColor\"/>");
        public static MarkupString Parent => Svg("<path d=\"M12 19V5\"/><path d=\"M6 11l6-6 6 6\"/>");
        public static MarkupString Copy => Svg("<rect x=\"9\" y=\"9\" width=\"11\" height=\"11\" rx=\"2\"/><path d=\"M5 15V6a2 2 0 0 1 2-2h8\"/>");
        public static MarkupString Trash => Svg("<path d=\"M4 7h16\"/><path d=\"M9 7V4h6v3\"/><path d=\"M6 7l1 13h10l1-13\"/>");
        public static MarkupString Close => Svg("<path d=\"M6 6l12 12\"/><path d=\"M18 6L6 18\"/>");
        public static MarkupString Panel => Svg("<rect x=\"3\" y=\"4\" width=\"18\" height=\"16\" rx=\"2\"/><path d=\"M15 4v16\"/>");
        public static MarkupString PanelLeft => Svg("<rect x=\"3\" y=\"4\" width=\"18\" height=\"16\" rx=\"2\"/><path d=\"M9 4v16\"/>");
        public static MarkupString Plus => Svg("<path d=\"M12 5v14\"/><path d=\"M5 12h14\"/>");
        public static MarkupString Pointer => Svg("<path d=\"M5 3l14 7-6 2-2 6z\"/>");

        /// <summary>The icon that stands for a kind of component in the toolbox.</summary>
        public static MarkupString For(ComponentType type) => type switch
        {
            ComponentType.String => Svg("<path d=\"M5 7V5h14v2\"/><path d=\"M12 5v14\"/><path d=\"M9 19h6\"/>"),
            ComponentType.Boolean => Svg("<rect x=\"4\" y=\"4\" width=\"16\" height=\"16\" rx=\"3\"/><path d=\"M8 12l3 3 5-6\"/>"),
            ComponentType.Int => Svg("<path d=\"M9 4L7 20\"/><path d=\"M17 4l-2 16\"/><path d=\"M4 9h16\"/><path d=\"M3 15h16\"/>"),
            ComponentType.Number => Svg("<circle cx=\"7\" cy=\"17\" r=\"1.2\" fill=\"currentColor\"/><path d=\"M4 6h6\"/><path d=\"M7 3v6\"/><path d=\"M14 6h6\"/><path d=\"M14 15h6\"/><path d=\"M14 19h6\"/>"),
            ComponentType.DateTime => Svg("<rect x=\"4\" y=\"5\" width=\"16\" height=\"15\" rx=\"2\"/><path d=\"M4 10h16\"/><path d=\"M9 3v4\"/><path d=\"M15 3v4\"/>"),
            ComponentType.Dropdown => Svg("<rect x=\"3\" y=\"6\" width=\"18\" height=\"12\" rx=\"2\"/><path d=\"M9 11l3 3 3-3\"/>"),
            ComponentType.VerticalLayout => Svg("<rect x=\"5\" y=\"4\" width=\"14\" height=\"6\" rx=\"1.5\"/><rect x=\"5\" y=\"14\" width=\"14\" height=\"6\" rx=\"1.5\"/>"),
            ComponentType.HorizontalLayout => Svg("<rect x=\"4\" y=\"5\" width=\"6\" height=\"14\" rx=\"1.5\"/><rect x=\"14\" y=\"5\" width=\"6\" height=\"14\" rx=\"1.5\"/>"),
            ComponentType.GroupLayout => Svg("<rect x=\"3\" y=\"5\" width=\"18\" height=\"14\" rx=\"2\"/><path d=\"M3 10h18\"/>"),
            ComponentType.List => Svg("<path d=\"M9 6h11\"/><path d=\"M9 12h11\"/><path d=\"M9 18h11\"/><circle cx=\"4.5\" cy=\"6\" r=\"1\" fill=\"currentColor\"/><circle cx=\"4.5\" cy=\"12\" r=\"1\" fill=\"currentColor\"/><circle cx=\"4.5\" cy=\"18\" r=\"1\" fill=\"currentColor\"/>"),
            _ => Svg("<rect x=\"4\" y=\"4\" width=\"16\" height=\"16\" rx=\"3\"/>")
        };
    }
}
