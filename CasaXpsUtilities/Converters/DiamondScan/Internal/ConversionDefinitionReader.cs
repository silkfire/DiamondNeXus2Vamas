namespace CasaXpsUtilities.Converters.DiamondScan.Internal
{
    using Ultimately;
    using Ultimately.Async;
    using Ultimately.Reasons;

    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;


    public class ConversionDefinitionReader
    {
        public static async Task<Option<ConversionDefinition>> Read(string filepath)
        {
            return await Optional.SomeWhen(File.Exists(filepath), $"Conversion definition file '{filepath}' does not exist")
                                 .FlatMapAsync(async () =>
                                 {
                                     using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                                     {
                                         return await Read(filepath, fs);
                                     }
                                 });
        }

        public static async Task<Option<ConversionDefinition>> Read(string filepath, Stream stream)
        {
            string sourceFileDirectory = null;
            var sampleInformationStrings = new List<ConversionDefinition.SampleInformationString>();

            using (var sr  = new StreamReader(stream))
            {
                var isFirstLine = true;
            
                string line;
            
                while ((line = await sr.ReadLineAsync())!= null)
                {
                    line = line.Trim();
            
                    if (isFirstLine)
                    {
                        if (line == "")
                        {
                            return Optional.None<ConversionDefinition>(Error.Create("First line of the conversion definition file must contain the source file directory"));
                        }
            
                        sourceFileDirectory = line.Trim('"');
            
                        isFirstLine = false;
                    }
                    else
                    {
                        if (line == "")
                        {
                            // Skip empty lines
            
                            continue;
                        }
            
                        var sampleInformationStringResult = ConversionDefinition.SampleInformationString.Parse(line);
            
                        foreach (var (sampleInformationString, _) in sampleInformationStringResult)
                        {
                             sampleInformationStrings.Add(sampleInformationString);
                        }
            
                        if (!sampleInformationStringResult.HasValue)
                        {
                            return Optional.None<ConversionDefinition.SampleInformationString, ConversionDefinition>(sampleInformationStringResult);
                        }
                    }
                }
            }
            
            return ConversionDefinition.Create(filepath, sourceFileDirectory, sampleInformationStrings);
        }
    }
}
