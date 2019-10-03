namespace CasaXpsUtilities.Converters.DiamondScan.Internal
{
    using Xps.Synchrotron.Diamond.Scans;

    using Ultimately;
    using Ultimately.Collections;
    using Ultimately.Reasons;
    using Ultimately.Utilities;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;


    public class ConversionDefinition
    {
        public string Filename { get; }

        public string Filepath { get; }


        public string ScanFilesDirectoryPath { get; }

        public IReadOnlyList<SampleInformationString> SampleInformationStrings { get; }


        public override string ToString() => $"CONVERSION DEFINITION: '{Filename}'";


        private ConversionDefinition(string filepath, string scanFilesDirectoryPath, IEnumerable<SampleInformationString> sampleInformationStrings)
        {
            Filename = Path.GetFileName(filepath);
            Filepath = filepath;

            ScanFilesDirectoryPath = scanFilesDirectoryPath;
            SampleInformationStrings = sampleInformationStrings.ToList().AsReadOnly();
        }

        public static Option<ConversionDefinition> Create(string filepath, string scanFilesDirectoryPath, IEnumerable<SampleInformationString> sampleInformationStrings)
        {
            var validationRules = new List<LazyOption>
            {
                Optional.Lazy(() => !string.IsNullOrWhiteSpace(filepath), "Filepath to conversion definition file cannot be empty"),
                Optional.Lazy(() => !string.IsNullOrWhiteSpace(scanFilesDirectoryPath), "Scan files directory path cannot be empty"),
                Optional.Lazy(() => sampleInformationStrings != null, "List of sample information strings cannot be null")
            };

            return validationRules.Reduce()
                                  .FlatMap(() =>
                                  {
                                      var sampleInformationStringList = sampleInformationStrings.ToList();

                                      return sampleInformationStringList.SomeWhen(siss => siss.Count > 0, "Definition must contain at least one sample information string");
                                  })
                                  .Map(siss => new ConversionDefinition(filepath, scanFilesDirectoryPath, siss), "Conversion definition validation failed");
        }


        public class SampleInformationString
        {
            private static readonly Regex _parseRegex = new Regex(@"^(\S+)\s+((?:\d+(?:-\d+)?)(?:,\d+(?:-\d+)?)*)(?:\s*?)(?:\s+(\d+))?\s*$");


            public string SampleName { get; }

            public IReadOnlyList<ScanNumberRange> ScanNumberRanges { get; }

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

            public static Option<SampleInformationString> Create(string sampleName, IEnumerable<ScanNumberRange> scanNumberRanges, Option<ushort> kineticEnergy)
            {
                return Optional.SomeWhen(scanNumberRanges != null, "List of scan number ranges in sample information string cannot be null").FlatMap(() =>
                {
                    var scanNumberRangesList = scanNumberRanges.ToList();

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

            public static Option<SampleInformationString> Parse(string @string)
            {
                return Optional.SomeWhen(@string != null, "String to parse sample information from cannot be null")
                               .FlatMap(() => _parseRegex.Match(@string).SomeWhen(m => m.Success, $"Invalid sample information string encountered: {@string.Substring(0, Math.Min(@string.Length, 50))}{(@string.Length > 50 ? "..." : "")}"))
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
                                             return ParseNumber(TryParse.ToUShort, m.Groups[3].Value, "kinetic energy value").Map(ke => ke.Some(Success.Create("Override excitation energy read from the sample with the specified kinetic energy value")))
                                                                                                                             .FlatMap(ke => Create(m.Groups[1].Value, rr, ke));
                                         }

                                         return Create(m.Groups[1].Value, rr, Optional.None<ushort>("Use binding energy value from sample"));
                                     });
                               }, "Failed to parse sample information string");

                Option<TValue> ParseNumber<TValue>(Func<string, Error, Option<TValue>> parseFunc, string number, string numberClassification)
                {
                    return parseFunc(number, $"Provided {numberClassification} too big: {number}");
                }
            }
        }
    }
}
