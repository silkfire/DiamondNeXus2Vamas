namespace CasaXpsUtilities.Vamas;

using Ultimately;
using Ultimately.Collections;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

/// <summary>
/// Represents a VAMAS data set.
/// </summary>
public class VamasDataSet
{
    /// <summary>
    /// Gets the name of the data set.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets a collection of sample identifiers associated with the current instance.
    /// </summary>
    public ReadOnlyCollection<string> SampleIdentifiers { get; }

    /// <summary>
    /// Gets a collection of blocks in the data set.
    /// </summary>
    public ReadOnlyCollection<Block> Blocks { get; }

    private VamasDataSet(string name, IEnumerable<string> sampleIdentifiers, IEnumerable<Block> blocks)
    {
        Name = name;
        SampleIdentifiers = sampleIdentifiers.ToList().AsReadOnly();
        Blocks = blocks.ToList().AsReadOnly();
    }

    public static Option<VamasDataSet> Create(string name, IEnumerable<string>? sampleIdentifiers, IEnumerable<Block>? blocks)
    {
        var validations = new List<LazyOption>
                          {
                              Optional.Lazy(() => !string.IsNullOrWhiteSpace(name), "Name cannot be empty"),
                              Optional.Lazy(() => sampleIdentifiers != null, "Collection of sample identifiers cannot be null"),
                              Optional.Lazy(() => blocks != null, "Collection of blocks cannot be null")
                          };

        return validations.Reduce()
                          .Map(() => new VamasDataSet(name, sampleIdentifiers!, blocks!), "VAMAS data set validation failed");
    }
}
