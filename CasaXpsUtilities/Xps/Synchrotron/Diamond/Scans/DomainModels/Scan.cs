namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.DomainModels
{
    using Upshot;
    using Upshot.Collections;

    using System.Collections.Generic;
    using System.IO;


    /// <summary>
    /// Represents a Diamond beamline scan file.
    /// </summary>
    public class Scan
    {
        private readonly List<Region> _regions;

        /// <summary>
        /// The complete filepath of the scan file.
        /// </summary>
        public string Filepath { get; }

        /// <summary>
        /// The sequence number of the scan.
        /// </summary>
        public string Number { get; }

        public IReadOnlyList<Region> Regions => _regions;


        private Scan(string filepath, string number, List<Region> regions)
        {
            Filepath = filepath;
            Number = number;

            _regions = regions;
        }


        public static Option<Scan> Create(string filepath, string number, List<Region> regions)
        {
            var validationRules = new List<LazyOption>
            {
                Optional.Lazy(() => !string.IsNullOrWhiteSpace(filepath), "Filepath to the scan cannot be empty"),
                Optional.Lazy(() => !string.IsNullOrWhiteSpace(number), "The scan number cannot be empty"),
                Optional.Lazy(() => regions != null, "List of regions on the scan cannot be null"),
            };

            return validationRules.Reduce()
                                  .FlatMap(() => new Scan(filepath, number, regions));
        }


        public override string ToString() => Path.GetFileNameWithoutExtension(Filepath);
    }
}
