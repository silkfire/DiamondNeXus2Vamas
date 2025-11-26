namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO;

using CasaXpsUtilities.IO;

using Ultimately;

using System.IO;
using System.Linq;
using System.Collections.ObjectModel;

/// <summary>
/// Provides NeXus files from a specified directory.
/// </summary>
/// <param name="sourceFileDirectoryPath">The path to the source file directory.</param>
public class NeXusFileProvider(string sourceFileDirectoryPath) : IFileProvider
{
    public Option<ReadOnlyCollection<string>> GetFiles()
    {
        return Optional.SomeWhen(Directory.Exists(sourceFileDirectoryPath), $"Source file directory '{sourceFileDirectoryPath}' specified in the definition file does not exist")
                       .Map(() => Directory.GetFiles(sourceFileDirectoryPath, "*.nxs").ToList().AsReadOnly());
    }
}
