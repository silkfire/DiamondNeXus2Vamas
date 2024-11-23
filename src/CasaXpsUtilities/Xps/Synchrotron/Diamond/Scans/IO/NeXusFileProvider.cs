namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO
{
    using CasaXpsUtilities.IO;

    using Ultimately;

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class NeXusFileProvider(string sourceFileDirectoryPath) : IFileProvider
    {
        private readonly string _sourceFileDirectoryPath = sourceFileDirectoryPath;

        public Option<IReadOnlyCollection<string>> GetFiles()
        {
            return Optional.SomeWhen(Directory.Exists(_sourceFileDirectoryPath), $"Source file directory '{_sourceFileDirectoryPath}' specified in the definition file does not exist")
                           .Map(() => Directory.GetFiles(_sourceFileDirectoryPath, "*.nxs").ToList().AsReadOnly() as IReadOnlyCollection<string>);
        }
    }
}
