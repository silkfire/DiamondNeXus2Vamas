namespace CasaXpsUtilities.DiamondNeXus2Vamas
{
    using System;
    using System.Collections.Generic;
    using Ultimately;

    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Omu.ValueInjecter;
    using Ultimately.Async;
    using Ultimately.Collections;
    using Ultimately.Reasons;


    public class ConfigurationSerializer
    {
        private readonly string _configurationLocation;


        public ConfigurationSerializer(string configurationLocation)
        {
            _configurationLocation = configurationLocation;
        }

        /// <summary>
        /// Reads the configuration file from disk <see langword="null"/>.
        /// </summary>
        public async Task<Option<Configuration>> Read()
        {
            return await _configurationLocation.SomeWhen(File.Exists, "Configuration file does not exist").MapAsync(async cl =>
            {
                await using var fs = new FileStream(cl, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
                    return Convert(await JsonSerializer.DeserializeAsync<ConfigurationEntity>(fs));
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
                                                await using var fs = new FileStream(_configurationLocation, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 4096, FileOptions.Asynchronous);
                                                {
                                                    await JsonSerializer.SerializeAsync(fs, ce);

                                                    return Optional.Some(Success.Create($"Configuration file successfully saved to '{_configurationLocation}'"));
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                return Optional.None(ExceptionalError.Create(e));
                                            }
                                        }, "Saving of configuration file failed");
        }


        private Configuration Convert(ConfigurationEntity entity)
        {
            return new Configuration(entity.ConversionDefinitionFilepath);
        }

        private ConfigurationEntity Convert(Configuration model)
        {
            return new ConfigurationEntity().InjectFrom(model) as ConfigurationEntity;
        }

        private class ConfigurationEntity
        {
            public string ConversionDefinitionFilepath { get; set; }
        }
    }
}
