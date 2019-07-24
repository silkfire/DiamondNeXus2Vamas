﻿namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.DomainModels
{
    using Ultimately;
    using Ultimately.Collections;

    using System.Collections.Generic;


    /// <summary>
    /// Represents a Diamond beamline scan file.
    /// </summary>
    public class Scan
    {
        private readonly List<Region> _regions;

        /// <summary>
        /// The sequence number of the scan.
        /// </summary>
        public uint Number => File.Number;

        /// <summary>
        /// Holds information about the physical scan file on disk.
        /// </summary>
        public ScanFile File { get; set; }

        public IReadOnlyList<Region> Regions => _regions;


        private Scan(ScanFile file, List<Region> regions)
        {
            File = file;

            _regions = regions;
        }


        public static Option<Scan> Create(ScanFile file, List<Region> regions)
        {
            var validationRules = new List<LazyOption>
            {
                Optional.Lazy(() => file != null, "Scan file cannot be empty"),
                Optional.Lazy(() => regions != null, "List of regions on the scan cannot be null"),
            };

            return validationRules.Reduce()
                                  .Map(() => new Scan(file, regions));
        }


        public override string ToString() => File.Filename;
    }
}
