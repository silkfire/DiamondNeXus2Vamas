namespace CasaXpsUtilities.DiamondNeXus2Vamas
{
    using Converters.DiamondScan;
    using Converters.DiamondScan.Internal;
    using Vamas.Internal.Time;
    using Vamas.IO;
    using Xps.Synchrotron.Diamond.Scans.IO;

    using Ultimately;
    using Ultimately.Async;

    using System.IO;
    using System.Threading.Tasks;


    public class ConversionService
    {
        private const string _outputFilename = "outputFile.vms";

        private readonly IScanFileReader _scanFileReader;
        private readonly ILocalTimeFactory<ILocalTime> _localTimeFactory;
        private readonly VamasWriter _vamasWriter;


        public ConversionService(IScanFileReader scanFileReader, ILocalTimeFactory<ILocalTime> localTimeFactory, VamasWriter vamasWriter)
        {
            _scanFileReader = scanFileReader;
            _localTimeFactory = localTimeFactory;
            _vamasWriter = vamasWriter;
        }


        public async Task<Option<(string outputDirectoryPath, string outputFilename)>> ConvertAndCreateOutputFile(string conversionDefinitionFilepath)
        {
            return await ConversionDefinitionReader.Read(conversionDefinitionFilepath).FlatMapAsync(cd => new DiamondNeXus2VamasConverter(new NeXusFileProvider(cd.ScanFilesDirectoryPath), _scanFileReader, _localTimeFactory).Convert(cd).Map(ds => (dataSet: ds, outputDirectoryPath: cd.ScanFilesDirectoryPath)))
                                                                                      .FlatMapAsync(async r =>
                                                                                      {
                                                                                          var outputFilepath = Path.Combine(r.outputDirectoryPath, _outputFilename);

                                                                                          var writeResult = await _vamasWriter.Write(r.dataSet, outputFilepath);

                                                                                          foreach (var _ in writeResult)
                                                                                          {
                                                                                              return Optional.Some((r.outputDirectoryPath, outputFilename: _outputFilename));
                                                                                          }

                                                                                          File.Delete(outputFilepath);

                                                                                          return Optional.None<(string outputDirectoryPath, string outputFilename)>(writeResult);
                                                                                      }, "Conversion operation failed");
        }
    }
}
