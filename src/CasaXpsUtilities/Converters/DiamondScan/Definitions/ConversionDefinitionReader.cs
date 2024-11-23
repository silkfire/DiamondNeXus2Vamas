namespace CasaXpsUtilities.Converters.DiamondScan.Definitions
{
    using Ultimately;
    using Ultimately.Async;
    using Ultimately.Collections;

    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class ConversionDefinitionReader
    {
        public static async Task<Option<ConversionDefinition>> Read(string filepath)
        {
            var validationRules = new List<LazyOption>
            {
                Optional.Lazy(() => !string.IsNullOrWhiteSpace(filepath), "Provided filepath cannot be empty"),
                Optional.Lazy(() => File.Exists(filepath), $"File '{filepath}' does not exist")
            };

            return await validationRules.Reduce()
                                        .FlatMapNone("Failed to read conversion definition file")
                                        .FlatMapAsync(async () =>
                                        {
                                            await using var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);

                                            return await Read(filepath, fs);
                                        });
        }

        public static async Task<Option<ConversionDefinition>> Read(string filepath, Stream stream)
        {
            string sourceFileDirectory = null!;
            var sampleInformationStrings = new List<ConversionDefinition.SampleInformationString>();

            using (var sr = new StreamReader(stream))
            {
                var isFirstLine = true;

                while (await sr.ReadLineAsync() is { } line)
                {
                    line = line.Trim();

                    if (isFirstLine)
                    {
                        if (line == "")
                        {
                            return Optional.None<ConversionDefinition>("First line of the conversion definition file must contain the source file directory");
                        }

                        sourceFileDirectory = $"{line.Trim('"').TrimEnd('\\')}\\";

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
