namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO
{
    using LiteHDF;
    using LiteHDF.Internal;
    using Ultimately;
    using Ultimately.Reasons;
    using Ultimately.Collections;

    using System;
    using System.Collections.Generic;


    public class NeXusReader : IScanFileReader
    {
        public Option<Scan> Read(ScanFile scanFile)
        {
            {
                using var scan = Hdf.Open(scanFile.Filepath);

                if (scan.FileIdentifier < 0L)
                {
                    return Optional.None<Scan>($"The scan file at '{scanFile.Filepath}' does not exist or is not a valid HDF document");
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
                                // Starting energy value (energies), counts / creation time Unix (image_data)

                                energies.Value.FirstOrNone("The 'energies' array must contain at least one value")
                                              .FlatMap(sev => scan.GetData<double>($"/entry1/instrument/{n}/image_data").SomeNotNull("Region must contain dataset 'image_data'")
                                                                                                                        .Filter(id => id.ChangeTime.HasValue, "Dataset 'image_data' does not specify a change time")
                                                                                                                        .Map(id => new
                                                                                                                        {
                                                                                                                            StartingEnergyValue = sev,
                                                                                                                            CreationTimeUnix = id.ChangeTime.Value,
                                                                                                                            ImageData  = id.Value
                                                                                                                        }))

                                // Excitation energy (excitation_energy)

                                              .FlatMap(d => scan.GetData<double>($"/entry1/instrument/{n}/excitation_energy").Value.SingleOrNone("The 'excitation_energy' array must contain exactly one value")
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
                                                                                                                                       d.StartingEnergyValue,
                                                                                                                                       d.CreationTimeUnix,
                                                                                                                                       d.ImageData,
                                                                                                                                       ExcitationEnergy = ee
                                                                                                                                   })
                                              )


                                // Step time (step_time)

                                              .FlatMap(d => scan.GetData<double>($"/entry1/instrument/{n}/step_time").SomeNotNull("Region must contain dataset 'step_time'")
                                                                                                                     .FlatMap(st => st.Value.SingleOrNone("The 'step_time' array must contain exactly one value"))
                                                                                                                     .Map(st => new
                                                                                                                     {
                                                                                                                         d.StartingEnergyValue,
                                                                                                                         d.CreationTimeUnix,
                                                                                                                         d.ImageData,
                                                                                                                         d.ExcitationEnergy,
                                                                                                                         StepTime = st
                                                                                                                     }))
                                              
                                // Energy step (energy_step)

                                              .FlatMap(d => scan.GetData<double>($"/entry1/instrument/{n}/energy_step").SomeNotNull("Region must contain dataset 'energy_step'")
                                                                                                                       .FlatMap(es => es.Value.SingleOrNone("The 'energy_step' array must contain exactly one value"))
                                                                                                                       .Map(es => new
                                                                                                                       {
                                                                                                                           d.StartingEnergyValue,
                                                                                                                           d.CreationTimeUnix,
                                                                                                                           d.ImageData,
                                                                                                                           d.ExcitationEnergy,
                                                                                                                           d.StepTime,
                                                                                                                           EnergyStep = es
                                                                                                                       }))
                                 // Energy mode (energy_mode)

                                              .FlatMap(d => scan.GetString($"/entry1/instrument/{n}/energy_mode").SomeNotNull("Region must contain dataset 'energy_mode'")
                                                                                                                 .Map(em => new
                                                                                                                 {
                                                                                                                     d.StartingEnergyValue,
                                                                                                                     d.CreationTimeUnix,
                                                                                                                     d.ImageData,
                                                                                                                     d.ExcitationEnergy,
                                                                                                                     d.StepTime,
                                                                                                                     d.EnergyStep,
                                                                                                                     EnergyMode = em
                                                                                                                 }))
                                              
                                              .FlatMap(d => Region.Create(n, d.CreationTimeUnix, d.StartingEnergyValue, d.ImageData, d.ExcitationEnergy, d.StepTime, d.EnergyStep, d.EnergyMode))
                                              .Match(
                                                  some: r => regions.Add(r),
                                                  none: e => outcome = Optional.None(Error.Create($"Failed to read region '{n}' in file '{scanFile.Filename}'").CausedBy(e))
                                              );
                            }
                        }
                    }
                });

                return outcome.FlatMap(() => Scan.Create(scanFile, regions));
            }
        }

        public Option<Scan> Read(string filepath)
        {
            return ScanFile.Create(filepath).FlatMap(Read);
        }
    }
}
