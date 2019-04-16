namespace CasaXpsUtilities.Vamas.Internal.Time
{
    public class LocalTimeFactory : ILocalTimeFactory<LocalTime>
    {
        private readonly string _timeZoneId;


        public LocalTimeFactory(TimeZoneId timeZoneId)
        {
            _timeZoneId = timeZoneId();
        }


        public LocalTime Create(long unixTimeSeconds)
        {
            return LocalTime.Create(unixTimeSeconds, _timeZoneId);
        }
    }
}
