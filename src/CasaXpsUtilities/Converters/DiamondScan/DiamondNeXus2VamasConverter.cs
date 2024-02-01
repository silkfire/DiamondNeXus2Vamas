namespace CasaXpsUtilities.Converters.DiamondScan
{
    using Definitions;

    using IO;
    using Vamas;
    using Vamas.Internal.Time;
    using Xps.Synchrotron.Diamond.Scans;
    using Xps.Synchrotron.Diamond.Scans.IO;

    using Ultimately;

    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    public partial class DiamondNeXus2VamasConverter
    {
        [GeneratedRegex("""_\d+$""", RegexOptions.Compiled)]
        private static partial Regex SpeciesSanitizeRegex();

        private readonly IFileProvider _fileProvider;
        private readonly IScanFileReader _scanFileReader;
        private readonly ILocalTimeFactory<ILocalTime> _localTimeFactory;

        public DiamondNeXus2VamasConverter(IFileProvider fileProvider, IScanFileReader scanFileReader, ILocalTimeFactory<ILocalTime> localTimeFactory)
        {
            _fileProvider = fileProvider;
            _scanFileReader = scanFileReader;
            _localTimeFactory = localTimeFactory;
        }

        public Option<VamasDataSet> Convert(ConversionDefinition conversionDefinition)
        {
            return Optional.SomeWhen(conversionDefinition != null, "Conversion definition cannot be null").FlatMap(() =>
            {
                var sampleIdentifiers = new HashSet<string>();
                var measurementIdentifiers = new Dictionary<string, int>();

                var blocks = new List<Block>();

                foreach (var sampleInformationString in conversionDefinition!.SampleInformationStrings)
                {
                    var matchedFilesResult = ScanFile.FilterByRanges(_fileProvider, sampleInformationString.ScanNumberRanges);

                    if (matchedFilesResult.HasValue)
                    {
                        foreach (var (matchedFiles, _) in matchedFilesResult)
                        {
                            foreach (var scanFile in matchedFiles)
                            {
                                var scanReadResult = _scanFileReader.Read(scanFile);

                                if (scanReadResult.HasValue)
                                {
                                    foreach (var (scan, _) in scanReadResult)
                                    {
                                        foreach (var region in scan.Regions)
                                        {
                                            var sampleIdentifier = $"{sampleInformationString.KineticEnergy.Match(ke => $"{ke}KE", _ => region.ExcitationEnergy.ToString())}-{sampleInformationString.SampleName}";
                                            sampleIdentifiers.Add(sampleIdentifier);

                                            var regionName = region.Name;
                                            var measurementIdentifier = $"{sampleIdentifier}-{regionName}";
                                            
                                            if (!measurementIdentifiers.TryAdd(measurementIdentifier, 0))
                                            {
                                                regionName = $"{regionName}-{++measurementIdentifiers[measurementIdentifier]}";
                                            }

                                            var blockCreationResult = Block.Create(FormatBlockName(region.StepTime, scan.Number, regionName),
                                                                                   sampleIdentifier,
                                                                                   _localTimeFactory.Create(region.CreationTimeUnix),
                                                                                   scanFile.Filepath,
                                                                                   regionName,
                                                                                   regionName.StartsWith("Survey") ? "Survey" : SpeciesSanitizeRegex().Replace(regionName, ""),
                                                                                   region.StartingEnergyValue,
                                                                                   region.EnergyStep,
                                                                                   region.Counts);

                                            if (blockCreationResult.HasValue)
                                            {
                                                foreach (var (block, _) in blockCreationResult)
                                                {
                                                    blocks.Add(block);
                                                }
                                            }
                                            else
                                            {
                                                return Optional.None<Block, VamasDataSet>(blockCreationResult);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    return Optional.None<Scan, VamasDataSet>(scanReadResult);
                                }
                            }
                        }
                    }
                    else
                    {
                        return Optional.None<IReadOnlyList<ScanFile>, VamasDataSet>(matchedFilesResult);
                    }
                }

                return VamasDataSet.Create(new DirectoryInfo(conversionDefinition.ScanFilesDirectoryPath).Name, sampleIdentifiers, blocks);
            });
        }

        private static string FormatBlockName(double stepTime, uint scanNumber, string regionName)
        {
            const double stepTimeFrameRatio = 0.058823529411764705;

            return $"[{System.Convert.ToByte(stepTime / stepTimeFrameRatio)}] {scanNumber}-{regionName}";
        }
    }
}
