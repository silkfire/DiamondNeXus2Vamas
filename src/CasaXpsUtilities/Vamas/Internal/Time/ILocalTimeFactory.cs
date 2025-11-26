namespace CasaXpsUtilities.Vamas.Internal.Time;

/// <summary>
/// Defines a factory for creating instances of a local time type from a Unix timestamp.
/// </summary>
/// <remarks>Implementations of this interface provide a way to convert Unix time values to strongly typed local
/// time representations. This is useful for abstracting time zone or calendar logic from consumers.</remarks>
/// <typeparam name="TLocalTime">The type of local time object to create. Must implement <see cref="ILocalTime"/>.</typeparam>
public interface ILocalTimeFactory<out TLocalTime>
    where TLocalTime : ILocalTime
{
    /// <summary>
    /// Creates a new instance of the local time type that represents the specified Unix time, expressed as the number
    /// of seconds that have elapsed since 00:00:00 UTC on 1 January 1970.
    /// </summary>
    /// <param name="unixTimeSeconds">The number of seconds since 00:00:00 UTC on 1 January 1970. Must be a non-negative value.</param>
    /// <returns>An instance of the local time type corresponding to the specified Unix time in seconds.</returns>
    TLocalTime Create(ulong unixTimeSeconds);
}
