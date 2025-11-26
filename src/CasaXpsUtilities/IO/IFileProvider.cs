namespace CasaXpsUtilities.IO;

using Ultimately;

using System.Collections.ObjectModel;

/// <summary>
/// Provides file paths.
/// </summary>
public interface IFileProvider
{
    /// <summary>
    /// Retrieves the file paths.
    /// </summary>
    Option<ReadOnlyCollection<string>> GetFiles();
}
