namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO
{
    using LiteHDF;
    using Ultimately;
    using Ultimately.Reasons;
    using Ultimately.Collections;

    using System;
    using System.Collections.Generic;

    public class NeXusReader : IScanFileReader
    {
        private const string BaseEntryName = "entry";

        public Option<Scan> Read(ScanFile scanFile)
        {
            using var scan = Hdf.Open(scanFile.Filepath);

            if (scan.FileIdentifier < 0L)
            {
                return Optional.None<Scan>($"The scan file at '{scanFile.Filepath}' does not exist or is not a valid HDF document");
            }

            var outcome = Optional.Some();
            var regions = new List<Region>();

            var entryName = $"/{BaseEntryName}1";
            if (scan.GetGroupObjectData(entryName).Length == 0)
            {
                entryName = BaseEntryName;
            }

            var regionGroups = scan.GetGroupObjectData($"/{entryName}/instrument");
            if (regionGroups.Length == 0)
            {
                return Optional.None<Scan>("Failed to find any regions in scan file");
            }

            foreach (var regionGroup in regionGroups)
            {
                if (regionGroup.Type == ObjectType.Group)
                {
                    var energies = regionGroup.File.GetData<double>($"/{entryName}/instrument/{regionGroup.Name}/energies");
                    if (energies != null)
                    {
                            // Starting energy value (energies), counts / creation time Unix (image_data)

                            new
                            {
                                RegionGroup = regionGroup,
                                Energies = energies
                            }.Some()
                             .FlatMap(d => d.Energies.Value.FirstOrNone("The 'energies' array must contain at least one value")
                                                           .Map(sev => new
                                                                       {
                                                                           d.RegionGroup,
                                                                           StartingEnergyValue = sev
                                                                       }))
                             
                             .FlatMap(d => d.RegionGroup.File.GetData<double>($"/{entryName}/instrument/{d.RegionGroup.Name}/image_data").SomeNotNull("Region must contain dataset 'image_data'")
                                                                                                                                         .Filter(id => id.ChangeTime.HasValue, "Dataset 'image_data' does not specify a change time")
                                                                                                                                         .Map(id => new
                                                                                                                                                    {
                                                                                                                                                        d.RegionGroup,
                                                                                                                                                        d.StartingEnergyValue,
                                                                                                                                                        CreationTimeUnix = id.ChangeTime!.Value,
                                                                                                                                                        ImageData = id.Value
                                                                                                                                                    }))

                             // Excitation energy (excitation_energy)

                             .FlatMap(d => d.RegionGroup.File.GetData<double>($"/{entryName}/instrument/{d.RegionGroup.Name}/excitation_energy").Value.SingleOrNone("The 'excitation_energy' array must contain exactly one value")
                                                                                                                                                      .FlatMap(ee =>
                                                                                                                                                      {
                                                                                                                                                          try
                                                                                                                                                          {
                                                                                                                                                              return Optional.Some(Convert.ToUInt16(ee));
                                                                                                                                                          }
                                                                                                                                                          catch
                                                                                                                                                          {
                                                                                                                                                              return Optional.None<ushort>($"Excitation energy value must be convertible to a 16-bit unsigned integer (was {ee})");
                                                                                                                                                          }
                                                                                                                                                      })
                                                                                                                                                      .Map(ee => new
                                                                                                                                                                 {
                                                                                                                                                                     d.RegionGroup,
                                                                                                                                                                     d.StartingEnergyValue,
                                                                                                                                                                     d.CreationTimeUnix,
                                                                                                                                                                     d.ImageData,
                                                                                                                                                                     ExcitationEnergy = ee
                                                                                                                                                                 })
                                     )


                             // Step time (step_time)

                             .FlatMap(d => d.RegionGroup.File.GetData<double>($"/{entryName}/instrument/{d.RegionGroup.Name}/step_time").SomeNotNull("Region must contain dataset 'step_time'")
                                                                                                                                        .FlatMap(st => st.Value.SingleOrNone("The 'step_time' array must contain exactly one value"))
                                                                                                                                        .Map(st => new
                                                                                                                                                   {
                                                                                                                                                       d.RegionGroup,
                                                                                                                                                       d.StartingEnergyValue,
                                                                                                                                                       d.CreationTimeUnix,
                                                                                                                                                       d.ImageData,
                                                                                                                                                       d.ExcitationEnergy,
                                                                                                                                                       StepTime = st
                                                                                                                                                   }))
                                      
                             // Energy step (energy_step)

                             .FlatMap(d => d.RegionGroup.File.GetData<double>($"/{entryName}/instrument/{d.RegionGroup.Name}/energy_step").SomeNotNull("Region must contain dataset 'energy_step'")
                                                                                                                                          .FlatMap(es => es.Value.SingleOrNone("The 'energy_step' array must contain exactly one value"))
                                                                                                                                          .Map(es => new
                                                                                                                                                     {
                                                                                                                                                         d.RegionGroup,
                                                                                                                                                         d.StartingEnergyValue,
                                                                                                                                                         d.CreationTimeUnix,
                                                                                                                                                         d.ImageData,
                                                                                                                                                         d.ExcitationEnergy,
                                                                                                                                                         d.StepTime,
                                                                                                                                                         EnergyStep = es
                                                                                                                                                     }))
                            
                             .FlatMap(d => Region.Create(d.RegionGroup.Name, d.CreationTimeUnix, d.StartingEnergyValue, d.ImageData, d.ExcitationEnergy, d.StepTime, d.EnergyStep))
                             .Match(
                                    some: regions.Add,
                                    none: e => outcome = Optional.None(Error.Create($"Failed to read region '{regionGroup.Name}' in file '{regionGroup.File.Filename}'").CausedBy(e))
                                   );
                    }
                }
            };

            return outcome.FlatMap(() => Scan.Create(scanFile, regions));
        }

        public Option<Scan> Read(string filepath)
        {
            return ScanFile.Create(filepath).FlatMap(Read);
        }
    }
}
