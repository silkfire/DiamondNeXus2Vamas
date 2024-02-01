namespace CasaXpsUtilities.Vamas.IO
{
    using Shared;

    using Ultimately;
    using Ultimately.Async;
    using Ultimately.Collections;
    using Ultimately.Reasons;

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class VamasWriter
    {
        private readonly LazyAsync<IReadOnlyDictionary<string, string>> _templates;

        public VamasWriter(ITemplateProvider templateProvider)
        {
            _templates = templateProvider.GetTemplates();
        }

        public async Task<Option> Write(VamasDataSet dataSet, string destinationFilepath)
        {
            await using var fs = new FileStream(destinationFilepath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, FileOptions.Asynchronous);
                return await Write(dataSet, fs).FlatMapNoneAsync("Failed to save VAMAS file");
        }

        public async Task<Option> Write(VamasDataSet dataSet, Stream stream)
        {
            var validationRules = new List<LazyOption>
            {
                Optional.Lazy(() => dataSet != null, "VAMAS data set cannot be null"),
                Optional.Lazy(() => stream  != null, "Stream to write to cannot be null"),
                Optional.Lazy(() => stream.CanWrite, "Stream to write to must be writable"),
            };

            return await validationRules.Reduce()
                                        .FlatMapAsync(async () =>
                                        {
                                            try
                                            {
                                                await using var sw = new StreamWriter(stream);

                                                var blockIndex = 0;

                                                await sw.WriteAsync(await FormatFileHeader());

                                                foreach (var block in dataSet.Blocks)
                                                {
                                                    await sw.WriteAsync(await FormatBlock(block, blockIndex++));

                                                    foreach (var count in block.Counts)
                                                    {
                                                        await sw.WriteLineAsync(count.ToString("G17", CultureInfo.InvariantCulture));
                                                    }
                                                }

                                                await sw.WriteAsync(await FormatFileFooter());
                                            }
                                            catch (Exception e)
                                            {
                                                return Optional.None(e);
                                            }

                                            return Optional.Some(Success.Create("VAMAS data set written to stream successfully"));
                                        }, "Failed to write VAMAS file to stream");

            async Task<string> FormatFileHeader()
            {
                return string.Format((await _templates)["FILE_HEADER"],

                                      3 + dataSet.SampleIdentifiers.Count,
                                      string.Join("", dataSet.SampleIdentifiers.Select(c => $"{Environment.NewLine}CasaRowLabel:{c}")),
                                      dataSet.Blocks.Count);
            }

            async Task<string> FormatBlock(Block block, int blockIndex)
            {
                return string.Format((await _templates)["BLOCK"],

                                     block.Name,
                                     block.SampleIdentifier,
                                     block.CreationTimeLocal.Value.Year,
                                     block.CreationTimeLocal.Value.Month,
                                     block.CreationTimeLocal.Value.Day,
                                     block.CreationTimeLocal.Value.Hour,
                                     block.CreationTimeLocal.Value.Minute,
                                     block.CreationTimeLocal.Value.Second,
                                     block.CreationTimeLocal.UtcOffset,
                                     Path.GetFileNameWithoutExtension(block.ScanFilepath),
                                     block.RegionName,
                                     Path.GetDirectoryName(block.ScanFilepath),
                                     blockIndex,
                                     block.Species,
                                     block.StartingEnergyValue.ToString("0.#", CultureInfo.InvariantCulture),
                                     block.EnergyStep.ToString("0.0##", CultureInfo.InvariantCulture),
                                     block.Counts.Count,
                                     block.Counts.Min().ToString("G17", CultureInfo.InvariantCulture),
                                     block.Counts.Max().ToString("G17", CultureInfo.InvariantCulture));
            }

            async Task<string> FormatFileFooter()
            {
                return (await _templates)["FILE_FOOTER"];
            }
        }
    }
}
