namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO.Services
{
    using CasaXpsUtilities.IO;

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;


    public class ScanFileProvider : IFileProvider
    {
        private readonly string _sourceDirectoryPath;


        public ScanFileProvider(string sourceDirectoryPath)
        {
            _sourceDirectoryPath = sourceDirectoryPath;
        }


        public IReadOnlyList<string> GetFiles()
        {
            return Directory.GetFiles(_sourceDirectoryPath, "*.nxs").ToList().AsReadOnly();
        }
    }
}
