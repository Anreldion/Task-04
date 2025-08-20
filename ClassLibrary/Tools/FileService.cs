using System.IO;
using ClassLibrary.Utilities;

namespace Messenger.Tools
{
    /// <summary>
    /// Provides basic file operations such as saving, reading, and deleting files.
    /// </summary>
    public class FileService : IFileService
    {
        /// <summary>
        /// Saves the given content to a file at the specified path. Overwrites if the file exists.
        /// </summary>
        /// <param name="content">The content to write to the file.</param>
        /// <param name="path">The path of the file to write.</param>
        public void Save(string content, string path)
        {
            Guard.NotNull(content, nameof(content));
            Guard.NotEmpty(content, nameof(content));
            Guard.NotNull(path, nameof(path));
            Guard.NotEmpty(path, nameof(path));

            File.WriteAllText(path, content);
        }

        /// <summary>
        /// Reads the contents of a file if it exists.
        /// </summary>
        /// <param name="path">The path of the file to read.</param>
        /// <param name="content">The read content, or <c>null</c> if the file does not exist.</param>
        /// <returns><c>true</c> if the file was successfully read; otherwise, <c>false</c>.</returns>
        public bool TryRead(string path, out string content)
        {
            content = null;
            if (!Exists(path)) return false;

            content = File.ReadAllText(path);
            return true;
        }

        /// <summary>
        /// Attempts to delete a file at the specified path.
        /// </summary>
        /// <param name="path">The path of the file to delete.</param>
        /// <returns><c>true</c> if the file was deleted; otherwise, <c>false</c>.</returns>
        public bool TryDelete(string path)
        {
            if (!Exists(path)) return false;

            File.Delete(path);
            return true;
        }

        /// <summary>
        /// Determines whether a file exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the file exists; otherwise, <c>false</c>.</returns>
        public bool Exists(string path)
        {
            Guard.NotNull(path, nameof(path));
            Guard.NotEmpty(path, nameof(path));

            return File.Exists(path);
        }
    }
}
