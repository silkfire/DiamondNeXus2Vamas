namespace CasaXpsUtilities.Vamas.Internal.Time
{
    using NodaTime;
    using NodaTime.Text;

    using System;
    using System.Globalization;


    public sealed class LocalTime : ILocalTime
    {
        private static readonly OffsetPattern _offsetPatternHours = OffsetPattern.CreateWithInvariantCulture("-H");
        private static readonly OffsetPattern _offsetPatternMinutes = OffsetPattern.CreateWithInvariantCulture("%m");

        private readonly ZonedDateTime _value;


        public DateTime Value => _value.ToDateTimeUnspecified();

        public string UtcOffset => $"{_offsetPatternHours.Format(_value.Offset)}{(double.Parse(_offsetPatternMinutes.Format(_value.Offset)) / 60).ToString("#.0#", CultureInfo.InvariantCulture)}";



        private LocalTime(ulong unixTimeSeconds, string timeZoneId)
        {
            _value = Instant.FromUnixTimeSeconds((long)unixTimeSeconds).InZone(DateTimeZoneProviders.Tzdb[timeZoneId]);
        }

        public static LocalTime Create(ulong unixTimeSeconds, string timeZoneId)
        {
            return new LocalTime(unixTimeSeconds, timeZoneId);
        }



        public override string ToString() => _value.ToString();
    }
}
