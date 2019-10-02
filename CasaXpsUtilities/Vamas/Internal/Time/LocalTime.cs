namespace CasaXpsUtilities.Vamas.Internal.Time
{
    using NodaTime;
    using NodaTime.Text;

    using System;


    public sealed class LocalTime : ILocalTime
    {
        private static readonly OffsetPattern _offsetPattern = OffsetPattern.CreateWithInvariantCulture("-H");

        private readonly ZonedDateTime _value;


        public DateTime Value => _value.ToDateTimeUnspecified();

        public string UtcOffset => _offsetPattern.Format(_value.Offset);



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
