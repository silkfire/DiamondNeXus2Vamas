namespace CasaXpsUtilities.Vamas.Internal.Time
{
    using NodaTime;
    using NodaTime.Text;

    using System;


    public class LocalTime : ILocalTime
    {
        protected static readonly OffsetPattern _offsetPattern = OffsetPattern.CreateWithInvariantCulture("-H");

        private readonly ZonedDateTime _zonedLocalTime;


        public DateTime Value => _zonedLocalTime.ToDateTimeUnspecified();

        public string UtcOffset => _offsetPattern.Format(_zonedLocalTime.Offset);



        private LocalTime(long unixTimeSeconds, string timeZoneId)
        {
            _zonedLocalTime = Instant.FromUnixTimeSeconds(unixTimeSeconds).InZone(DateTimeZoneProviders.Tzdb[timeZoneId]);
        }

        public static LocalTime Create(long unixTimeSeconds, string timeZoneId)
        {
            return new LocalTime(unixTimeSeconds, timeZoneId);
        }



        public override string ToString() => _zonedLocalTime.ToString();
    }
}
