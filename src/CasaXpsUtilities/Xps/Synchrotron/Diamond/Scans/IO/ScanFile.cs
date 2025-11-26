namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO;

using CasaXpsUtilities.IO;

using Ultimately;
using Ultimately.Collections;
using Ultimately.Utilities;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Represents a Diamond Light Source beamline scan file.
/// </summary>
public partial class ScanFile
{
    [GeneratedRegex("""^i09-(\d+)""", RegexOptions.Compiled)]
    internal static partial Regex MatchScanNumberRegex();

    /// <summary>
    /// Gets the full file system path to the directory where the scan file is stored.
    /// </summary>
    public string ScanDirectory { get; }

    /// <summary>
    /// Gets the name of the file associated with this instance.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets the scan number associated with this instance.
    /// </summary>
    public uint Number { get; }

    /// <summary>
    /// Gets the full file path by combining the scan directory and file name.
    /// </summary>
    public string FilePath => $"{ScanDirectory}/{FileName}";

    private ScanFile(string scanDirectory, string fileName, uint number)
    {
        ScanDirectory = scanDirectory;
        FileName = fileName;
        Number = number;
    }

    /// <summary>
    /// Creates a <see cref="ScanFile"/> instance from the given file path and scan number.
    /// </summary>
    /// <param name="filePath">The file path of the scan file.</param>
    /// <param name="number">The scan number.</param>
    public static Option<ScanFile> Create(string filePath, uint number)
    {
        return NotEmptyFilepath(filePath).Map(() => new ScanFile(Path.GetDirectoryName(filePath)!, Path.GetFileName(filePath), number));
    }

    /// <summary>
    /// Creates a <see cref="ScanFile"/> instance from the given file path by parsing the scan number from the filename.
    /// </summary>
    /// <param name="filePath">The file path of the scan file.</param>
    public static Option<ScanFile> Create(string filePath)
    {
        return NotEmptyFilepath(filePath).Map(() => Path.GetFileName(filePath))
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
                                         .Map(sf => new ScanFile(Path.GetDirectoryName(filePath)!, sf.Filename, sf.Number));
    }

    private static Option NotEmptyFilepath(string filePath)
    {
        return Optional.SomeWhen(!string.IsNullOrWhiteSpace(filePath), "File path to scan cannot be empty");
    }

    /// <summary>
    /// Filters scan files provided by the specified file provider to include only those whose scan numbers fall within
    /// any of the specified scan number ranges.
    /// </summary>
    /// <param name="scanFileProvider">The file provider that supplies the collection of scan files to be filtered.</param>
    /// <param name="scanNumberRanges">A collection of scan number ranges to match against. Only scan files with scan numbers within these ranges are
    /// included.</param>
    public static Option<ReadOnlyCollection<ScanFile>> FilterByRanges(IFileProvider? scanFileProvider, IEnumerable<ScanNumberRange>? scanNumberRanges)
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
                                                               .AsReadOnly());
                              });
    }

    public override string ToString() => FileName;
}
