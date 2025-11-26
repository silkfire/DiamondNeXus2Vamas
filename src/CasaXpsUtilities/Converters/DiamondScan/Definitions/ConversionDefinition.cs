namespace CasaXpsUtilities.Converters.DiamondScan.Definitions;

using Xps.Synchrotron.Diamond.Scans;

using Ultimately;
using Ultimately.Collections;
using Ultimately.Reasons;
using Ultimately.Utilities;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Represents a conversion definition, including file paths and associated sample information, used to describe and
/// manage scan file conversions.
/// </summary>
public partial class ConversionDefinition
{
    /// <summary>
    /// The filename of the conversion definition file.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets the full path to the file associated with this instance.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Gets the full file system path to the directory where scan files are stored.
    /// </summary>
    public string ScanFilesDirectoryPath { get; }

    /// <summary>
    /// Gets a collection of sample information strings associated with this instance.
    /// </summary>
    public ReadOnlyCollection<SampleInformationString> SampleInformationStrings { get; }

    private ConversionDefinition(string filePath, string scanFilesDirectoryPath, IEnumerable<SampleInformationString> sampleInformationStrings)
    {
        FileName = Path.GetFileName(filePath);
        FilePath = filePath;

        ScanFilesDirectoryPath = scanFilesDirectoryPath;
        SampleInformationStrings = sampleInformationStrings.ToList().AsReadOnly();
    }

    /// <summary>
    /// Creates a new conversion definition instance after validating the provided parameters.
    /// </summary>
    /// <param name="filePath">The file path to the conversion definition file.</param>
    /// <param name="scanFilesDirectoryPath">The directory path containing the scan files.</param>
    /// <param name="sampleInformationStrings">The sample information strings.</param>
    public static Option<ConversionDefinition> Create(string filePath, string scanFilesDirectoryPath, IEnumerable<SampleInformationString>? sampleInformationStrings)
    {
        var validationRules = new List<LazyOption>
                              {
                                  Optional.Lazy(() => !string.IsNullOrWhiteSpace(filePath), "Filepath to conversion definition file cannot be empty"),
                                  Optional.Lazy(() => !string.IsNullOrWhiteSpace(scanFilesDirectoryPath), "Scan files directory path cannot be empty"),
                                  Optional.Lazy(() => sampleInformationStrings != null, "List of sample information strings cannot be null")
                              };

        return validationRules.Reduce()
                              .FlatMap(() =>
                              {
                                  var sampleInformationStringList = sampleInformationStrings!.ToList();

                                  return sampleInformationStringList.SomeWhen(siss => siss.Count > 0, "Definition must contain at least one sample information string");
                              })
                              .Map(siss => new ConversionDefinition(filePath, scanFilesDirectoryPath, siss), "Conversion definition validation failed");
    }

    public partial class SampleInformationString
    {
        [GeneratedRegex("""^(\S+)\s+((?:\d+(?:-\d+)?)(?:,\d+(?:-\d+)?)*)(?:\s*?)(?:\s+(\d+))?\s*$""", RegexOptions.Compiled)]
        private static partial Regex Parts { get; }

        /// <summary>
        /// Gets the name of the sample.
        /// </summary>
        public string SampleName { get; }

        /// <summary>
        /// Gets the collection of scan number ranges associated with this instance.
        /// </summary>
        public ReadOnlyCollection<ScanNumberRange> ScanNumberRanges { get; }

        /// <summary>
        /// Gets the kinetic energy value, if available.
        /// </summary>
        public Option<ushort> KineticEnergy { get; }

        public override string ToString()
        {
            return $"{string.Join(", ", ScanNumberRanges)}{KineticEnergy.Match(ke => $" | {ke}", _ => "")}";
        }

        private SampleInformationString(string sampleName, IEnumerable<ScanNumberRange> scanNumberRanges, Option<ushort> kineticEnergy)
        {
            SampleName = sampleName;
            ScanNumberRanges = scanNumberRanges.ToList().AsReadOnly();
            KineticEnergy = kineticEnergy;
        }

        /// <summary>
        /// Creates a new sample information string instance after validating the provided parameters.
        /// </summary>
        /// <param name="sampleName">The name of the sample.</param>
        /// <param name="scanNumberRanges">The collection of scan number ranges associated with the sample.</param>
        /// <param name="kineticEnergy">The kinetic energy value, if available.</param>
        public static Option<SampleInformationString> Create(string sampleName, IEnumerable<ScanNumberRange>? scanNumberRanges, Option<ushort> kineticEnergy)
        {
            return Optional.SomeWhen(scanNumberRanges != null, "List of scan number ranges in sample information string cannot be null").FlatMap(() =>
            {
                var scanNumberRangesList = scanNumberRanges!.ToList();

                var validationRules = new List<LazyOption>
                                      {
                                          Optional.Lazy(() => !string.IsNullOrWhiteSpace(sampleName), "Name of sample in sample information string cannot be empty"),
                                          Optional.Lazy(() => scanNumberRangesList.Count > 0, "List of scan number ranges in sample information string must contain at least one number range"),
                                          Optional.Lazy(() => !kineticEnergy.HasValue || kineticEnergy.Exists(ke => ke > 0), "Specified kinetic energy value in sample information string must be greater than zero")
                                      };

                return validationRules.Reduce().Map(() => new SampleInformationString(sampleName,
                                                        scanNumberRangesList,
                                                        kineticEnergy));
            });
        }

        /// <summary>
        /// Parses a line from a conversion definition file into a <see cref="SampleInformationString"/> instance, if
        /// the line is valid.
        /// </summary>
        /// <returns>An <see cref="Option{T}"/> containing the parsed <see cref="SampleInformationString"/> if parsing succeeds;
        /// otherwise, an <see cref="Option{T}"/> with an error describing the failure.</returns>
        public static Option<SampleInformationString> Parse(string? conversionDefinitionFileLine)
        {
            return Optional.SomeWhen(conversionDefinitionFileLine != null, "String to parse sample information from cannot be null")
                           .FlatMap(() => Parts.Match(conversionDefinitionFileLine!).SomeWhen(m => m.Success, $"Invalid sample information string encountered: {conversionDefinitionFileLine![..Math.Min(conversionDefinitionFileLine!.Length, 50)]}{(conversionDefinitionFileLine.Length > 50 ? "..." : "")}"))
                           .FlatMap(m =>
                           {
                               return m.Groups[2].Value.Split(',').Select(rs =>
                                       {
                                           var range = rs.Split('-');

                                           return ParseNumber(TryParse.ToUInt, range[0], "scan number").FlatMap(f => ParseNumber(TryParse.ToUInt, range.Length == 1 ? range[0] : range[1], "scan number").FlatMap(l => ScanNumberRange.Create(f, l)));
                                       }).Transform(r => r)
                                       .FlatMap(rr =>
                                       {
                                           if (m.Groups[3].Success)
                                           {
                                               return ParseNumber(TryParse.ToUShort, m.Groups[3].Value, "kinetic energy value").Map(ke => ke.Some("Override excitation energy read from the sample with the specified kinetic energy value"))
                                                   .FlatMap(ke => Create(m.Groups[1].Value, rr, ke));
                                           }

                                           return Create(m.Groups[1].Value, rr, Optional.None<ushort>("Use binding energy value from sample"));
                                       });
                           }, "Failed to parse sample information string");

            static Option<TValue> ParseNumber<TValue>(Func<string, Error, Option<TValue>> parseFunc, string number, string numberClassification)
            {
                return parseFunc(number, $"Provided {numberClassification} too big: {number}");
            }
        }
    }

    public override string ToString() => $"CONVERSION DEFINITION: '{FileName}'";
}
