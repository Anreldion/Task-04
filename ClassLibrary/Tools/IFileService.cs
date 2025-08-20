namespace Messenger.Tools
{
    public interface IFileService
    {
        /// <summary>
        /// Saves content to a file. Overwrites existing content.
        /// </summary>
        void Save(string content, string path);

        /// <summary>
        /// Tries to read the content of a file. Returns true if successful.
        /// </summary>
        bool TryRead(string path, out string content);

        /// <summary>
        /// Checks if the file exists at the specified path.
        /// </summary>
        bool Exists(string path);

        /// <summary>
        /// Deletes the file at the specified path if it exists.
        /// </summary>
        bool TryDelete(string path);
    }

}
