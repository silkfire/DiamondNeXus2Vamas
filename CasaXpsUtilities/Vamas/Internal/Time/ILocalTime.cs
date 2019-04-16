namespace CasaXpsUtilities.Vamas.Internal.Time
{
    using System;


    /// <summary>
    /// Specifies a local time and its offset to UTC as a formatted string.
    /// </summary>
    public interface ILocalTime
    {
        DateTime Value { get; }

        string UtcOffset { get; }
    }
}
