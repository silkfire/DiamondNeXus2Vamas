namespace DiamondNeXus2Vamas;

using CasaXpsUtilities.Converters.DiamondScan;
using CasaXpsUtilities.Converters.DiamondScan.Definitions;
using CasaXpsUtilities.Vamas.Internal.Time;
using CasaXpsUtilities.Vamas.IO;
using CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO;

using Ultimately;
using Ultimately.Async;

using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Service for converting Diamond Light Source NeXus scan files to VAMAS format.
/// </summary>
public class ConversionService(IScanFileReader scanFileReader, ILocalTimeFactory<ILocalTime> localTimeFactory, VamasWriter vamasWriter)
{
    private const string OutputFilename = "outputFile.vms";

    /// <summary>
    /// Converts NeXus scan files to VAMAS format based on the specified conversion definition.
    /// </summary>
    /// <param name="conversionDefinitionFilePath">The file path to the conversion definition file.</param>
    public async Task<Option<(string OutputDirectoryPath, string OutputFilename)>> ConvertAndCreateOutputFile(string conversionDefinitionFilePath)
    {
        return await ConversionDefinitionReader.Read(conversionDefinitionFilePath).FlatMapAsync(cd => new DiamondNeXus2VamasConverter(new NeXusFileProvider(cd.ScanFilesDirectoryPath), scanFileReader, localTimeFactory).Convert(cd).Map(ds => (DataSet: ds, OutputDirectoryPath: cd.ScanFilesDirectoryPath)))
                                               .FlatMapAsync(async r =>
                                               {
                                                   var outputFilepath = Path.Combine(r.OutputDirectoryPath, OutputFilename);

                                                   var writeResult = await vamasWriter.Write(r.DataSet, outputFilepath);

                                                   foreach (var _ in writeResult)
                                                   {
                                                       return Optional.Some((r.OutputDirectoryPath, OutputFilename));
                                                   }

                                                   File.Delete(outputFilepath);

                                                   return Optional.None<(string outputDirectoryPath, string outputFilename)>(writeResult);
                                               }, "Conversion operation failed");
    }
}
