namespace CasaXpsUtilities.Vamas.Internal.Time;

using System;

/// <summary>
/// Represents a local time and exposes a property that represents its offset from UTC as a formatted string.
/// </summary>
public interface ILocalTime
{
    /// <summary>
    /// Gets the local date and time value.
    /// </summary>
    DateTime Value { get; }

    /// <summary>
    /// Gets the UTC offset as a formatted string.
    /// </summary>
    string UtcOffset { get; }
}
