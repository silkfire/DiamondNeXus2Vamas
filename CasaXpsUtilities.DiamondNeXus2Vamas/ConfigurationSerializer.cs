namespace CasaXpsUtilities.DiamondNeXus2Vamas
{
    using SpanJson;
    using SpanJson.Resolvers;
    using Ultimately;
    using Ultimately.Async;
    using Ultimately.Collections;
    using Ultimately.Reasons;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;


    public class ConfigurationSerializer
    {
        // TODO: Use SpanJson instead

        private readonly string _configurationLocation;


        public ConfigurationSerializer(string configurationLocation)
        {
            _configurationLocation = configurationLocation;
        }

        /// <summary>
        /// Reads the configuration file from disk.
        /// </summary>
        public async Task<Option<Configuration>> Read()
        {
            return await _configurationLocation.SomeWhen(File.Exists, "Configuration file does not exist").MapAsync(async cl =>
            {
                await using var fs = new FileStream(cl, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
                    return Convert(await JsonSerializer.Generic.Utf8.DeserializeAsync<ConfigurationEntity, IncludeNullsCamelCaseResolver<byte>>(fs));
            });
        }

        public async Task<Option> SaveAsync(Configuration configuration)
        {
            var validationRules = new List<LazyOption>
            {
                Optional.Lazy(() => configuration != null,                                                  "Configuration object cannot be null"),
                Optional.Lazy(() => !string.IsNullOrWhiteSpace(configuration.ConversionDefinitionFilepath), "Conversion definition filepath cannot be empty")
            };

            return await validationRules.Reduce()
                                        .Map(() => Convert(configuration))
                                        .FlatMapAsync(async ce =>
                                        {
                                            try
                                            {
                                                await using var fs = new FileStream(_configurationLocation, FileMode.Create, FileAccess.Write, FileShare.Write, 4096, FileOptions.Asynchronous);
                                                {
                                                    await JsonSerializer.Generic.Utf8.SerializeAsync<ConfigurationEntity, IncludeNullsCamelCaseResolver<byte>>(ce, fs);

                                                    return Optional.Some(Success.Create($"Configuration file successfully saved to '{_configurationLocation}'"));
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                return Optional.None(ExceptionalError.Create(e));
                                            }
                                        }, "Saving of configuration file failed");
        }


        private static Configuration Convert(ConfigurationEntity entity)
        {
            return new Configuration(entity.ConversionDefinitionFilepath);
        }

        private static ConfigurationEntity Convert(Configuration model)
        {
            return new ConfigurationEntity(model.ConversionDefinitionFilepath);
        }

        private class ConfigurationEntity
        {
            public string ConversionDefinitionFilepath { get; }


            [JsonConstructor]
            public ConfigurationEntity(string conversionDefinitionFilepath)
            {
                ConversionDefinitionFilepath = conversionDefinitionFilepath;
            }
        }
    }
}
