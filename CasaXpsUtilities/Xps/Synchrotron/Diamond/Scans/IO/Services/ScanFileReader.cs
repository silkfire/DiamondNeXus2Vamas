namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO.Services
{
    using DomainModels;

    using HdfLite;
    using HdfLite.Internal;
    using Ultimately;
    using Ultimately.Reasons;

    using System.Collections.Generic;


    public class ScanFileReader : IScanFileReader
    {
        public Option<Scan> Read(string filepath)
        {
            var scanFileResult = ScanFile.Create(filepath);

            foreach (var (scanFile, _) in scanFileResult)
            {
                using (var scan = Hdf.Open(filepath))
                {
                    if (scan.FileIdentifier < 0L)
                    {
                        return Optional.None<Scan>($"The file at '{filepath}' does not exist or is not a valid HDF document");
                    }


                    var outcome = Optional.Some();


                    var regions = new List<Region>();

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
                                        outcome = Optional.None($"Region '{n}' in file '{scanFile.Filename}' does not contain dataset 'image_data'");

                                        return;
                                    }

                                    if (!imageData.ChangeTime.HasValue)
                                    {
                                        outcome = Optional.None($"Dataset 'image_data' in region '{n}' of file '{scanFile.Filename}' does not specify a change time");

                                        return;
                                    }


                                    var createRegionResult = Region.Create(n);

                                    foreach (var (region, _) in createRegionResult)
                                    {
                                        regions.Add(region);

                                        return;
                                    }

                                    createRegionResult.MatchNone(e => outcome = Optional.None(Error.Create($"Validation error when parsing region in file '{scanFile.Filename}'").CausedBy(e)));
                                }
                            }
                        }
                    });



                    return outcome.FlatMap(() => Scan.Create(scanFile, regions));
                }
            }

            return Optional.None<ScanFile, Scan>(scanFileResult);
            
        }
    }
}
