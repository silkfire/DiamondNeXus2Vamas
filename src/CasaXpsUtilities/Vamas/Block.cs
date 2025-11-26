namespace CasaXpsUtilities.Vamas;

using Internal.Time;

using Ultimately;
using Ultimately.Collections;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

/// <summary>
/// Represents a VAMAS data block.
/// </summary>
public class Block
{
    /// <summary>
    /// Gets the name of the block.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the sample identifier.
    /// </summary>
    public string SampleIdentifier { get; }

    /// <summary>
    /// Gets the local creation time.
    /// </summary>
    public ILocalTime CreationTimeLocal { get; }

    /// <summary>
    /// Gets the scan file path.
    /// </summary>
    public string ScanFilePath { get; }

    /// <summary>
    /// Gets the region name.
    /// </summary>
    public string RegionName { get; }

    /// <summary>
    /// Gets the species name associated with the object.
    /// </summary>
    public string Species { get; }

    /// <summary>
    /// Gets the starting energy value.
    /// </summary>
    public double StartingEnergyValue { get; }
    
    /// <summary>
    /// Gets the energy step (in <c>eV</c>).
    /// </summary>
    public double EnergyStep { get; }

    /// <summary>
    /// Gets a collection of count values associated with the current instance.
    /// </summary>
    public ReadOnlyCollection<double> Counts { get; }

    private Block(string name, string sampleIdentifier, ILocalTime creationTimeLocal, string scanFilePath, string regionName, string species, double startingEnergyValue, double energyStep, IEnumerable<double> counts)
    {
        Name = name;
        SampleIdentifier = sampleIdentifier;
        CreationTimeLocal = creationTimeLocal;
        ScanFilePath = scanFilePath;
        RegionName = regionName;
        Species = species;
        StartingEnergyValue = -1 * startingEnergyValue;
        EnergyStep = energyStep;
        Counts = counts.ToList().AsReadOnly();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Block"/> class after validating the provided parameters.
    /// </summary>
    /// <param name="name">The name of the block.</param>
    /// <param name="sampleIdentifier">The sample identifier.</param>
    /// <param name="creationTimeLocal">The local creation time.</param>
    /// <param name="scanFilePath">The scan file path.</param>
    /// <param name="regionName">The region name.</param>
    /// <param name="species">The species name.</param>
    /// <param name="startingEnergyValue">The starting energy value.</param>
    /// <param name="energyStep">The energy step.</param>
    /// <param name="counts">The count values.</param>
    public static Option<Block> Create(string name, string sampleIdentifier, ILocalTime? creationTimeLocal, string scanFilePath, string regionName, string species, double startingEnergyValue, double energyStep, IEnumerable<double>? counts)
    {
        var validationRules = new List<LazyOption>
                              {
                                  Optional.Lazy(() => !string.IsNullOrWhiteSpace(name), "Name cannot be empty"),
                                  Optional.Lazy(() => !string.IsNullOrWhiteSpace(sampleIdentifier), "Sample identifier cannot be empty"),
                                  Optional.Lazy(() => creationTimeLocal != null, "Local creation time cannot be null"),
                                  Optional.Lazy(() => !string.IsNullOrWhiteSpace(scanFilePath), "Scan file path cannot be empty"),
                                  Optional.Lazy(() => !string.IsNullOrWhiteSpace(regionName), "Region name cannot be empty"),
                                  Optional.Lazy(() => !string.IsNullOrWhiteSpace(species), "Species cannot be empty"),
                                  Optional.Lazy(() => energyStep > 0, $"Energy step must be greater than zero (was {energyStep})"),
                                  Optional.Lazy(() => counts != null, "List of counts cannot be null")
                              };

        return validationRules.Reduce()
                              .FlatMap(() => counts!.ToList().SomeWhen(cc => cc.Count > 0, "List of counts cannot be empty"))
                              .Map(cc => new Block(name, sampleIdentifier, creationTimeLocal!, scanFilePath, regionName, species, startingEnergyValue, energyStep, cc), "Block validation failed");
    }

    public override string ToString() => Name;
}
