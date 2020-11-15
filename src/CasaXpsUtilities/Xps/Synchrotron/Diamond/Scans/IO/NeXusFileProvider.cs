namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO
{
    using CasaXpsUtilities.IO;

    using Ultimately;

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;


    public class NeXusFileProvider : IFileProvider
    {
        private readonly string _sourceFileDirectoryPath;


        public NeXusFileProvider(string sourceFileDirectoryPath)
        {
            _sourceFileDirectoryPath = sourceFileDirectoryPath;
        }


        public Option<IReadOnlyList<string>> GetFiles()
        {
            return Optional.SomeWhen(Directory.Exists(_sourceFileDirectoryPath), $"Specified source file directory '{_sourceFileDirectoryPath}' does not exist")
                           .Map(() => Directory.GetFiles(_sourceFileDirectoryPath, "*.nxs").ToList().AsReadOnly() as IReadOnlyList<string>);
        }
    }
}
