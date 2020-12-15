namespace CasaXpsUtilities.Vamas
{
    using Internal.Time;

    using Ultimately;
    using Ultimately.Collections;

    using System.Collections.Generic;
    using System.Linq;


    public class Block
    {
        public string Name { get; }

        public string SampleIdentifier { get; }

        public ILocalTime CreationTimeLocal { get; }

        public string ScanFilepath { get; }

        public string RegionName { get; }

        public string Species { get; }

        public double StartingEnergyValue { get; }

        public double EnergyStep { get; }

        public IReadOnlyList<double> Counts { get; }



        private Block(string name, string sampleIdentifier, ILocalTime creationTimeLocal, string scanFilepath, string regionName, string species, double startingEnergyValue, double energyStep, IEnumerable<double> counts)
        {
            Name = name;
            SampleIdentifier = sampleIdentifier;
            CreationTimeLocal = creationTimeLocal;
            ScanFilepath = scanFilepath;
            RegionName = regionName;
            Species = species;
            StartingEnergyValue = -1 * startingEnergyValue;
            EnergyStep = energyStep;
            Counts = counts.ToList().AsReadOnly();
        }

        public static Option<Block> Create(string name, string sampleIdentifier, ILocalTime creationTimeLocal, string scanFilepath, string regionName, string species, double startingEnergyValue, double energyStep, IEnumerable<double> counts)
        {
            var validationRules = new List<LazyOption>
            {
                Optional.Lazy(() => !string.IsNullOrWhiteSpace(name), "Name cannot be empty"),
                Optional.Lazy(() => !string.IsNullOrWhiteSpace(sampleIdentifier), "Sample identifier cannot be empty"),
                Optional.Lazy(() => creationTimeLocal != null, "Local creation time cannot be null"),
                Optional.Lazy(() => !string.IsNullOrWhiteSpace(scanFilepath), "Scan filepath cannot be empty"),
                Optional.Lazy(() => !string.IsNullOrWhiteSpace(regionName), "Region name cannot be empty"),
                Optional.Lazy(() => !string.IsNullOrWhiteSpace(species), "Species cannot be empty"),
                Optional.Lazy(() => energyStep > 0, $"Energy step must be greater than zero (was {energyStep})"),
                Optional.Lazy(() => counts != null, "List of counts cannot be null")
            };

            return validationRules.Reduce()
                                  .FlatMap(() => counts.ToList().SomeWhen(cc => cc.Count > 0, "List of counts cannot be empty"))
                                  .Map(cc => new Block(name, sampleIdentifier, creationTimeLocal, scanFilepath, regionName, species, startingEnergyValue, energyStep, cc), "Block validation failed");
        }

        public override string ToString() => Name;
    }
}
