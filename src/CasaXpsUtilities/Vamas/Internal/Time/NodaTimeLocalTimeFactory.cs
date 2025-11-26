namespace CasaXpsUtilities.Vamas.Internal.Time;

/// <summary>
/// Provides a factory for creating <see cref="NodaTimeLocalTime"/> instances based on a specified time zone.
/// </summary>
public class NodaTimeLocalTimeFactory(string timeZoneId) : ILocalTimeFactory<NodaTimeLocalTime>
{
    public NodaTimeLocalTime Create(ulong unixTimeSeconds)
    {
        return NodaTimeLocalTime.Create(unixTimeSeconds, timeZoneId);
    }
}
