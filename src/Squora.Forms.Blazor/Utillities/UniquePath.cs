using Squora.Forms.Core.Models;

namespace Squora.Forms.Blazor.Utillities
{
    public static class UniquePath
    {
        /// <summary>
        /// Gives a new element a path no other element in the form uses, by adding a number to <paramref name="basePath"/> when it is taken.
        /// </summary>
        public static string For(DynamicFormModel root, string basePath)
        {
            var taken = new HashSet<string>();
            Collect(root, taken);

            var candidate = basePath;
            for (var n = 2; taken.Contains(candidate); n++)
            {
                candidate = $"{basePath}{n}";
            }
            return candidate;
        }

        private static void Collect(DynamicFormModel node, HashSet<string> paths)
        {
            if (node.Parameters.Entries.TryGetValue("Path", out var path) && path is string p)
            {
                paths.Add(p);
            }
            foreach (var child in node.Elements)
            {
                Collect(child, paths);
            }
        }
    }
}
