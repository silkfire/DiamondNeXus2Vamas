namespace DiamondNeXus2Vamas;

using Ultimately;
using Ultimately.Async;
using Ultimately.Collections;
using Ultimately.Reasons;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public class ConfigurationSerializer(string configurationLocation)
{
    private static readonly JsonSerializerOptions s_serializerOptions;

    private readonly string _configurationLocation = configurationLocation;


    static ConfigurationSerializer()
    {
        s_serializerOptions = new JsonSerializerOptions
                              {
                                  PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                              };
        s_serializerOptions.MakeReadOnly(true);
    }

    /// <summary>
    /// Reads the configuration file from disk.
    /// </summary>
    public Option<Configuration> Read()
    {
        return _configurationLocation.SomeWhen(File.Exists, "Configuration file does not exist").Map(cl => Convert(JsonSerializer.Deserialize<ConfigurationEntity>(ReadAllBytes(cl), s_serializerOptions)!));
    }

    public async Task<Option> SaveAsync(Configuration? configuration)
    {
        var validationRules = new List<LazyOption>
                              {
                                  Optional.Lazy(() => configuration != null,                                                  "Configuration object cannot be null"),
                                  Optional.Lazy(() => !string.IsNullOrWhiteSpace(configuration!.ConversionDefinitionFilePath), "Conversion definition file path cannot be empty")
                              };

        return await validationRules.Reduce()
                                    .Map(() => Convert(configuration!))
                                    .FlatMapAsync(async ce =>
                                    {
                                        try
                                        {
                                            await using var fs = new FileStream(_configurationLocation, FileMode.Create, FileAccess.Write, FileShare.Write, 4096, FileOptions.Asynchronous);
                                            
                                            {
                                                await JsonSerializer.SerializeAsync(fs, ce, s_serializerOptions);

                                                return Optional.Some(Success.Create($"Configuration file successfully saved to '{_configurationLocation}'"));
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            return Optional.None(e);
                                        }
                                    }, "Saving of configuration file failed");
    }


    private static Configuration Convert(ConfigurationEntity entity)
    {
        return new Configuration(entity.ConversionDefinitionFilePath);
    }

    private static ConfigurationEntity Convert(Configuration model)
    {
        return new ConfigurationEntity(model.ConversionDefinitionFilePath);
    }

    private record ConfigurationEntity(string ConversionDefinitionFilePath);


    private static byte[] ReadAllBytes(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize: 1, FileOptions.SequentialScan);
        var bytes = new byte[fs.Length];
        int index = 0, remaining = bytes.Length;
        while (remaining > 0)
        {
            int n = fs.Read(bytes, index, remaining);
            if (n == 0) throw new EndOfStreamException();
            index += n;
            remaining -= n;
        }
        return bytes;
    }
}
