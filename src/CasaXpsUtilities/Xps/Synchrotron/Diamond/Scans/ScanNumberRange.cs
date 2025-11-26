namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans;

using Ultimately;
using Ultimately.Collections;

using System.Collections.Generic;

/// <summary>
/// Represents a range of scan numbers.
/// </summary>
public class ScanNumberRange
{
    /// <summary>
    /// Starting scan number value in range.
    /// </summary>
    public uint StartingValue { get; }

    /// <summary>
    /// Final scan number value in range.
    /// </summary>
    public uint FinalValue { get; }

    private ScanNumberRange(uint startingValue, uint finalValue)
    {
        StartingValue = startingValue;
        FinalValue = finalValue;
    }

    /// <summary>
    /// Creates a new scan number range using the specified starting and final values.
    /// </summary>
    /// <param name="startingValue">The first scan number in the range.</param>
    /// <param name="finalValue">The last scan number in the range.</param>
    public static Option<ScanNumberRange> Create(uint startingValue, uint finalValue)
    {
        var validationRules = new List<LazyOption>
                              {
                                  Optional.Lazy(() => startingValue > 0, "Starting value of scan number range must be greater than zero"),
                                  Optional.Lazy(() => finalValue    > 0, "Final value of scan number range must be greater than zero"),
                                  Optional.Lazy(() => finalValue   >= startingValue, $"Starting value of scan number range must be smaller than the final one: {{ {startingValue} [too big] - {finalValue} }}"),
                              };

        return validationRules.Reduce().Map(() => new ScanNumberRange(startingValue, finalValue));
    }

    public override string ToString() => $"{StartingValue}{(StartingValue != FinalValue ? $"-{FinalValue}" : "")}";
}
