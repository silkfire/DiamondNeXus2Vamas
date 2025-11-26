namespace CasaXpsUtilities.Converters.DiamondScan;

using Definitions;
using Shared;

using IO;
using Vamas;
using Vamas.Internal.Time;
using Xps.Synchrotron.Diamond.Scans;
using Xps.Synchrotron.Diamond.Scans.IO;

using Ultimately;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// Converts NeXus scan files to VAMAS format.
/// </summary>
/// <param name="fileProvider">The file provider for accessing scan files.</param>
/// <param name="scanFileReader">The scan file reader for reading NeXus scan files.</param>
/// <param name="localTimeFactory">The factory for creating local time instances.</param>
public partial class DiamondNeXus2VamasConverter(IFileProvider fileProvider, IScanFileReader scanFileReader, ILocalTimeFactory<ILocalTime> localTimeFactory)
{
    [GeneratedRegex("""_\d+$""", RegexOptions.Compiled)]
    private static partial Regex SpeciesSanitization { get; }

    private readonly IFileProvider _fileProvider = fileProvider;
    private readonly IScanFileReader _scanFileReader = scanFileReader;
    private readonly ILocalTimeFactory<ILocalTime> _localTimeFactory = localTimeFactory;

    /// <summary>
    /// Converts NeXus scan files to a VAMAS data set based on the provided conversion definition.
    /// </summary>
    /// <param name="conversionDefinition">The conversion definition.</param>
    public Option<VamasDataSet> Convert(ConversionDefinition conversionDefinition)
    {
        var sampleIdentifiers = new OrderedSetExt<string>();
        var measurementIdentifiers = new Dictionary<string, int>();

        var blocks = new List<Block>();

        foreach (var sampleInformationString in conversionDefinition.SampleInformationStrings)
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
                                                                           scanFile.FilePath,
                                                                           regionName,
                                                                           regionName.StartsWith("Survey") ? "Survey" : SpeciesSanitization.Replace(regionName, ""),
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
                return Optional.None<ReadOnlyCollection<ScanFile>, VamasDataSet>(matchedFilesResult);
            }
        }

        return VamasDataSet.Create(new DirectoryInfo(conversionDefinition.ScanFilesDirectoryPath).Name, sampleIdentifiers, blocks);
    }

    private static string FormatBlockName(double stepTime, uint scanNumber, string regionName)
    {
        const double stepTimeFrameRatio = 1 / 17D;

        return $"[{System.Convert.ToByte(stepTime / stepTimeFrameRatio)}] {scanNumber}-{regionName}";
    }
}
