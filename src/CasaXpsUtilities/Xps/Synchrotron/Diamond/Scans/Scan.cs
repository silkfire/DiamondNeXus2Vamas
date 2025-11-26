namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans;

using IO;

using Ultimately;
using Ultimately.Collections;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

/// <summary>
/// Represents a Diamond beamline scan.
/// </summary>
public class Scan
{
    /// <summary>
    /// The sequence number of the scan.
    /// </summary>
    public uint Number => File.Number;

    /// <summary>
    /// Holds information about the physical scan file on disk.
    /// </summary>
    public ScanFile File { get; }

    public ReadOnlyCollection<Region> Regions { get; }


    private Scan(ScanFile file, IEnumerable<Region> regions)
    {
        File = file;
        Regions = regions.ToList().AsReadOnly();
    }

    /// <summary>
    /// Creates a new instance of <see cref="Scan"/> after validating the provided parameters.
    /// </summary>
    /// <param name="file">The scan file associated with the scan.</param>
    /// <param name="regions">The regions of interest within the scan.</param>
    public static Option<Scan> Create(ScanFile? file, IEnumerable<Region>? regions)
    {
        var validationRules = new List<LazyOption>
                              {
                                  Optional.Lazy(() => file != null, "Scan file cannot be empty"),
                                  Optional.Lazy(() => regions != null, "List of regions on the scan cannot be null"),
                              };

        return validationRules.Reduce()
                              .Map(() => new Scan(file!, regions!));
    }

    public override string ToString() => File.FileName;
}
