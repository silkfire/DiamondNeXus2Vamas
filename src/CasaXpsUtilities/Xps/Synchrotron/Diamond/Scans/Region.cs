namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans;

using Ultimately;
using Ultimately.Collections;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

/// <summary>
/// Represents a region within a Diamond synchrotron XPS scan.
/// </summary>
public class Region
{
    /// <summary>
    /// Gets the name associated with the current instance.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the creation time of the resource, expressed as a Unix timestamp in seconds.
    /// </summary>
    /// <remarks>The Unix timestamp represents the number of seconds that have elapsed since 00:00:00 UTC on 1
    /// January 1970, not counting leap seconds.</remarks>
    public ulong CreationTimeUnix { get; }

    /// <summary>
    /// Gets the starting energy value for the region.
    /// </summary>
    public double StartingEnergyValue { get; }

    /// <summary>
    /// Gets the collection of counts associated with the region.
    /// </summary>
    public ReadOnlyCollection<double> Counts { get; }

    /// <summary>
    /// Gets the excitation energy value associated with the current instance.
    /// </summary>
    public ushort ExcitationEnergy { get; }

    /// <summary>
    /// Gets the time interval, in seconds, between simulation steps.
    /// </summary>
    public double StepTime { get; }

    /// <summary>
    /// The energy step size for the region.
    /// </summary>
    public double EnergyStep { get; }

    private Region(string name, ulong creationTimeUnix, double startingEnergyValue, IEnumerable<double> counts, ushort excitationEnergy, double stepTime, double energyStep)
    {
        Name = name;
        CreationTimeUnix = creationTimeUnix;
        StartingEnergyValue = startingEnergyValue;
        Counts = counts.ToList().AsReadOnly();
        ExcitationEnergy = excitationEnergy;
        StepTime = stepTime;
        EnergyStep = energyStep;
    }

    public static Option<Region> Create(string name, ulong creationTimeUnix, double startingEnergyValue, IEnumerable<double>? counts, ushort excitationEnergy, double stepTime, double energyStep)
    {
        var validationRules = new List<LazyOption>
                              {
                                  Optional.Lazy(() => !string.IsNullOrWhiteSpace(name), "Name cannot be empty"),
                                  Optional.Lazy(() => creationTimeUnix > 0, "Creation time must be greater than zero"),
                                  Optional.Lazy(() => counts != null, "List of counts cannot be null"),
                                  Optional.Lazy(() => excitationEnergy > 0, "Excitation energy must be greater than zero"),
                                  Optional.Lazy(() => stepTime > 0, $"Step time must be greater than zero (was {stepTime})"),
                                  Optional.Lazy(() => energyStep > 0, $"Energy step must be greater than zero (was {energyStep})")
                              };

        return validationRules.Reduce()
                              .FlatMap(() => counts!.ToList().SomeWhen(cc => cc.Count > 0, "List of counts cannot be empty"))
                              .Map(cc => new Region(name, creationTimeUnix, startingEnergyValue, cc.AsReadOnly(), excitationEnergy, stepTime, energyStep), "Region validation failed");
    }

    public override string ToString() => $"REGION | {Name}";
}
