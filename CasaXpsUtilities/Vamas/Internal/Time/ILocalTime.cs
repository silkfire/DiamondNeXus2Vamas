namespace CasaXpsUtilities.Vamas.Internal.Time
{
    using System;


    /// <summary>
    /// Represents a local time and exposes a property that represents its offset from UTC as a formatted string.
    /// </summary>
    public interface ILocalTime
    {
        DateTime Value { get; }

        string UtcOffset { get; }
    }
}
