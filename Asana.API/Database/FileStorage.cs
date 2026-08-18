using System.IO;

namespace Asana.API.Database
{
    /// <summary>
    /// Resolves where the JSON-backed stores keep their files, and makes sure
    /// those folders exist before anything tries to read them.
    ///
    /// The roots were previously hardcoded to <c>C:\temp</c> with backslash
    /// separators, so the API only worked on Windows, and then only if that
    /// folder had been created by hand. Every request failed with a
    /// DirectoryNotFoundException otherwise.
    /// </summary>
    public static class FileStorage
    {
        private static string _root = Path.Combine(Directory.GetCurrentDirectory(), "data");

        /// <summary>Folder holding one JSON file per ToDo.</summary>
        public static string ToDoRoot => Path.Combine(_root, "ToDos");

        /// <summary>Folder holding one JSON file per Project.</summary>
        public static string ProjectRoot => Path.Combine(_root, "Projects");

        /// <summary>
        /// Point storage at <paramref name="root"/> and create the folders.
        /// A relative path is resolved against <paramref name="basePath"/> so
        /// the default "data" lands beside the project rather than wherever
        /// the process happened to be launched from.
        /// </summary>
        public static void Initialize(string? root, string basePath)
        {
            if (string.IsNullOrWhiteSpace(root))
            {
                root = "data";
            }

            _root = Path.IsPathRooted(root)
                ? root
                : Path.Combine(basePath, root);

            Directory.CreateDirectory(ToDoRoot);
            Directory.CreateDirectory(ProjectRoot);
        }
    }
}
