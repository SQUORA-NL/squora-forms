using System.Text.RegularExpressions;
using Squora.Forms.Common.Enums;
using Squora.Forms.Core.Models;
using Squora.Forms.Core.Parser;

namespace Squora.Forms.Blazor.Utillities
{
    /// <summary>
    /// Finding, removing, copying and naming elements in a form model. Shared by the editor's property pane and
    /// the action bar drawn on the selected element, so both do the same thing.
    /// </summary>
    public static class FormTree
    {
        /// <summary>The parent of a direct child is the root, which stands for the whole form; the root itself has none.</summary>
        public static DynamicFormModel? FindParent(DynamicFormModel? root, DynamicFormModel? element)
        {
            if (root == null || element == null || root == element) return null;
            if (root.Elements.Contains(element)) return root;
            foreach (var child in root.Elements)
            {
                var found = FindParent(child, element);
                if (found != null) return found;
            }
            return null;
        }

        /// <summary>Removes the element from wherever it sits. The root cannot be removed.</summary>
        public static bool Remove(DynamicFormModel root, DynamicFormModel element)
        {
            var parent = FindParent(root, element);
            return parent != null && parent.Elements.Remove(element);
        }

        /// <summary>True for the layouts that elements can be dropped into or added to.</summary>
        public static bool IsContainer(DynamicFormModel element) => element.DynamicType.ComponentType is
            ComponentType.VerticalLayout or ComponentType.HorizontalLayout or ComponentType.GroupLayout;

        /// <summary>
        /// Puts a copy of the element right after it, with paths of its own so the copy does not share its values
        /// with the original. Returns the copy, or null when the element cannot be copied (the root, or one that does not serialize).
        /// </summary>
        public static DynamicFormModel? Duplicate(DynamicFormModel root, DynamicFormModel element)
        {
            var parent = FindParent(root, element);
            if (parent == null) return null;

            var parser = new DynamicFormModelParser();
            var copy = parser.Deserialize(parser.Serialize(element));
            if (copy == null) return null;

            var taken = new HashSet<string>();
            CollectPaths(root, taken);
            var renamed = new Dictionary<string, string>();
            Renew(copy, taken, renamed);

            parent.Elements.Insert(parent.Elements.IndexOf(element) + 1, copy);
            return copy;
        }

        /// <summary>
        /// Adds a new element where the person is working: into the selected layout, after the selected field,
        /// or at the end of the form when nothing is selected. Returns the layout it went into.
        /// </summary>
        public static DynamicFormModel Add(DynamicFormModel root, DynamicFormModel? selected, DynamicFormModel added)
        {
            if (selected != null && selected != root && IsContainer(selected))
            {
                selected.Elements.Add(added);
                return selected;
            }

            var parent = selected == null ? null : FindParent(root, selected);
            if (parent != null && selected != null)
            {
                parent.Elements.Insert(parent.Elements.IndexOf(selected) + 1, added);
                return parent;
            }

            root.Elements.Add(added);
            return root;
        }

        /// <summary>What the person sees the element as: its label when it has one, otherwise what kind of element it is.</summary>
        public static string Describe(DynamicFormModel element)
        {
            if (element.Parameters.Entries.TryGetValue("Label", out var label) && label is string text && !string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
            return TypeName(element);
        }

        /// <summary>What kind of element it is, in the words the toolbox uses: "Text Field", "Vertical Layout".</summary>
        public static string TypeName(DynamicFormModel element) => DisplayName(element.DynamicType.ComponentType);

        public static string DisplayName(ComponentType type) => type switch
        {
            ComponentType.String => "Text Field",
            ComponentType.Boolean => "Checkbox",
            ComponentType.Int => "Integer",
            ComponentType.Number => "Number",
            ComponentType.DateTime => "Date/Time",
            ComponentType.Dropdown => "Dropdown",
            ComponentType.VerticalLayout => "Vertical Layout",
            ComponentType.HorizontalLayout => "Horizontal Layout",
            ComponentType.GroupLayout => "Group Layout",
            ComponentType.List => "List",
            ComponentType.FileUpload => "File Upload",
            _ => Regex.Replace(type.ToString(), "(?<=[a-z])(?=[A-Z])", " ")
        };

        private static void CollectPaths(DynamicFormModel node, HashSet<string> paths)
        {
            if (node.Parameters.Entries.TryGetValue("Path", out var path) && path is string p)
            {
                paths.Add(p);
            }
            foreach (var child in node.Elements)
            {
                CollectPaths(child, paths);
            }
        }

        // two elements that shared a path in the original keep sharing one in the copy
        private static void Renew(DynamicFormModel node, HashSet<string> taken, Dictionary<string, string> renamed)
        {
            if (node.Parameters.Entries.TryGetValue("Path", out var path) && path is string basePath && basePath.Length > 0)
            {
                if (!renamed.TryGetValue(basePath, out var fresh))
                {
                    fresh = basePath;
                    for (var n = 2; taken.Contains(fresh); n++)
                    {
                        fresh = $"{basePath}{n}";
                    }
                    taken.Add(fresh);
                    renamed[basePath] = fresh;
                }
                node.Parameters.Add("Path", fresh);
            }
            foreach (var child in node.Elements)
            {
                Renew(child, taken, renamed);
            }
        }
    }
}
