namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO.Services
{
    using Dtos;

    using HdfLite;
    using HdfLite.Internal;
    using Upshot;

    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;


    public class ScanFileReader : IScanFileReader
    {
        public Option<ScanDto> Read(string filepath)
        {
            if (!File.Exists(filepath))
            {
                return Optional.None<ScanDto>($"Scan file '{filepath}' does not exist");
            }

            var filename = Path.GetFileName(filepath);

            var filenameScanNumberMatch = Regex.Match(filename, @"^i09-(\d+)");

            if (!filenameScanNumberMatch.Success)
            {
                return Optional.None<ScanDto>($"Failed to parse the scan number from file '{filename}'");
            }

            using (var scan = Hdf.Open(filepath))
            {
                if (scan.FileIdentifier < 0L)
                {
                    return Optional.None<ScanDto>($"The file at '{filepath}' is not a valid HDF document");
                }


                var outcome = Optional.Some(default);


                var regions = new List<RegionDto>();

                scan.IterateGroup("/entry1/instrument", (n, t, _) =>
                {
                    foreach (var __ in outcome)
                    {
                        if (t == ObjectType.Group)
                        {
                            var energies = scan.GetData<double>($"/entry1/instrument/{n}/energies");

                            if (energies != null)
                            {
                                var imageData = scan.GetData<double>($"/entry1/instrument/{n}/image_data");

                                if (imageData == null)
                                {
                                    outcome = Optional.None($"Region '{n}' in file '{filename}' does not contain dataset 'image_data'");

                                    return;
                                }

                                if (!imageData.ChangeTime.HasValue)
                                {
                                    outcome = Optional.None($"Dataset 'image_data' in region '{n}' of file '{filename}' does not specify a change time");

                                    return;
                                }



                                regions.Add(new RegionDto(n));
                            }
                        }
                    }
                });



                return outcome.FlatMap(() => new ScanDto(filepath, filenameScanNumberMatch.Groups[1].Value, regions));
            }
        }
    }
}
