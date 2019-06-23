namespace CasaXpsUtilities.IO
{
    using System.Collections.Generic;


    /// <summary>
    /// Represents a service to retrieve a list of filenames.
    /// </summary>
    public interface IFileProvider
    {
        /// <summary>
        /// Retrieves a read-only list of filenames.
        /// </summary>
        IReadOnlyList<string> GetFiles();
    }
}
