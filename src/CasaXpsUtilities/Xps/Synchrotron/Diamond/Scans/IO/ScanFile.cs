namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO
{
    using CasaXpsUtilities.IO;

    using Ultimately;
    using Ultimately.Collections;
    using Ultimately.Utilities;

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a Diamond beamline scan file.
    /// </summary>
    public partial class ScanFile
    {
        [GeneratedRegex("""^i09-(\d+)""", RegexOptions.Compiled)]
        internal static partial Regex MatchScanNumberRegex();

        public string ScanDirectory { get; }

        public string Filename { get; }

        public uint Number { get; }

        public string Filepath => $"{ScanDirectory}/{Filename}";

        private ScanFile(string scanDirectory, string filename, uint number)
        {
            ScanDirectory = scanDirectory;
            Filename = filename;
            Number = number;
        }

        public static Option<ScanFile> Create(string filepath, uint number)
        {
            return NotEmptyFilepath(filepath).Map(() => new ScanFile(Path.GetDirectoryName(filepath)!, Path.GetFileName(filepath), number));
        }

        public static Option<ScanFile> Create(string filepath)
        {
            return NotEmptyFilepath(filepath).Map(() => Path.GetFileName(filepath))
                                             .FlatMap(fn => MatchScanNumberRegex().Match(fn).SomeWhen(m => m.Success, $"Could not parse scan number from filename '{fn}'")
                                                                                            .Map(m => new
                                                                                            {
                                                                                                Filename = fn,
                                                                                                Match = m
                                                                                            }))
                                             .FlatMap(m => TryParse.ToUInt(m.Match.Groups[1].Value, $"Parsed scan number too big. Filename: '{m.Filename}'").Map(sn => new
                                                                                                                                                             {
                                                                                                                                                                 m.Filename,
                                                                                                                                                                 Number = sn
                                                                                                                                                             }))
                                             .Map(sf => new ScanFile(Path.GetDirectoryName(filepath)!, sf.Filename, sf.Number));
        }



        private static Option NotEmptyFilepath(string filepath)
        {
            return Optional.SomeWhen(!string.IsNullOrWhiteSpace(filepath), "Filepath to scan cannot be empty");
        }

        public static Option<IReadOnlyList<ScanFile>> FilterByRanges(IFileProvider? scanFileProvider, IEnumerable<ScanNumberRange>? scanNumberRanges)
        {
            var validationRules = new List<LazyOption>
            {
                Optional.Lazy(() => scanFileProvider != null, "Scan file provider cannot be null"),
                Optional.Lazy(() => scanNumberRanges != null, "List of scan number ranges to match against cannot be null")
            };

            return validationRules.Reduce()
                                  .FlatMap(() => scanFileProvider!.GetFiles().Filter(sfps => sfps != null, "List of scan files to filter cannot be null"))
                                  .FlatMap(sfps => sfps.Select(Create).Transform(sf => sf))
                                  .FlatMap(sfs =>
                                  {
                                      var scanNumberRangesList = scanNumberRanges!.ToList();

                                      return Optional.SomeWhen(sfs.Count > 0, "List of scan files to filter cannot be empty")
                                                     .FlatMap(() => Optional.SomeWhen(scanNumberRangesList.Count > 0, "List of scan number ranges to match against cannot be empty"))
                                                     .Map(() => sfs.Where(sf => scanNumberRangesList.Any(sn => sf.Number >= sn.StartingValue && sf.Number <= sn.FinalValue))
                                                                   .ToList()
                                                                   .AsReadOnly() as IReadOnlyList<ScanFile>);
                                  });
        }

        public override string ToString() => Filename;
    }
}
