namespace CasaXpsUtilities.Vamas.Internal.Time;

using NodaTime;
using NodaTime.Text;

using System;
using System.Globalization;

/// <summary>
/// Represents a local time value with time zone information, based on NodaTime's <see cref="ZonedDateTime"/>, for precise date and
/// time handling in a specific time zone.
/// </summary>
public sealed class NodaTimeLocalTime : ILocalTime
{
    private static readonly OffsetPattern s_offsetPatternHours = OffsetPattern.CreateWithInvariantCulture("-H");
    private static readonly OffsetPattern s_offsetPatternMinutes = OffsetPattern.CreateWithInvariantCulture("%m");

    private readonly ZonedDateTime _value;

    public DateTime Value => _value.ToDateTimeUnspecified();

    public string UtcOffset => $"{s_offsetPatternHours.Format(_value.Offset)}{(double.Parse(s_offsetPatternMinutes.Format(_value.Offset)) / 60).ToString("#.0#", CultureInfo.InvariantCulture)}";

    private NodaTimeLocalTime(ulong unixTimeSeconds, string timeZoneId)
    {
        _value = Instant.FromUnixTimeSeconds((long)unixTimeSeconds).InZone(DateTimeZoneProviders.Tzdb[timeZoneId]);
    }

    /// <summary>
    /// Creates a new instance of <see cref="NodaTimeLocalTime"/> from the specified Unix time in seconds and time zone ID.
    /// </summary>
    /// <param name="unixTimeSeconds">The Unix time in seconds.</param>
    /// <param name="timeZoneId">The time zone ID.</param>
    public static NodaTimeLocalTime Create(ulong unixTimeSeconds, string timeZoneId)
    {
        return new NodaTimeLocalTime(unixTimeSeconds, timeZoneId);
    }

    public override string ToString() => _value.ToString();
}
