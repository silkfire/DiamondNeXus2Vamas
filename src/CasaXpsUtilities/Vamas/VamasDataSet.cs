namespace CasaXpsUtilities.Vamas
{
    using Ultimately;
    using Ultimately.Collections;

    using System.Collections.Generic;
    using System.Linq;

    public class VamasDataSet
    {
        public string Name { get; }

        public IReadOnlyCollection<string> SampleIdentifiers { get; }

        public IReadOnlyCollection<Block> Blocks { get; }

        private VamasDataSet(string name, IReadOnlySet<string> sampleIdentifiers, IEnumerable<Block> blocks)
        {
            Name = name;
            SampleIdentifiers = sampleIdentifiers.ToList().AsReadOnly();
            Blocks = blocks.ToList().AsReadOnly();
        }

        public static Option<VamasDataSet> Create(string name, IReadOnlySet<string> sampleIdentifiers, IEnumerable<Block> blocks)
        {
            var validations = new List<LazyOption>
            {
                Optional.Lazy(() => !string.IsNullOrWhiteSpace(name), "Name cannot be empty"),
                Optional.Lazy(() => sampleIdentifiers != null, "List of sample identifiers cannot be null"),
                Optional.Lazy(() => blocks != null, "List of blocks cannot be null")
            };

            return validations.Reduce()
                              .Map(() => new VamasDataSet(name, sampleIdentifiers, blocks), "VAMAS data set validation failed");
        }
    }
}
