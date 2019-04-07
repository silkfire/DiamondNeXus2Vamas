namespace Silkfire.CasaXpsUtilities.Core.Models.Properties
{
    public class LocalTimeFactory : ILocalTimeFactory<LocalTime>
    {
        private readonly TimeZoneId _getTimeZoneId;


        public LocalTimeFactory(TimeZoneId timeZoneId)
        {
            _getTimeZoneId = timeZoneId;
        }


        public LocalTime Create(long unixTimeSeconds)
        {
            return LocalTime.Create(unixTimeSeconds, _getTimeZoneId());
        }
    }
}