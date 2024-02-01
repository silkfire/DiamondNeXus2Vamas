namespace CasaXpsUtilities.Vamas.Internal.Time
{
    public class LocalTimeFactory : ILocalTimeFactory<LocalTime>
    {
        private readonly string _timeZoneId;

        public LocalTimeFactory(string timeZoneId)
        {
            _timeZoneId = timeZoneId;
        }

        public LocalTime Create(ulong unixTimeSeconds)
        {
            return LocalTime.Create(unixTimeSeconds, _timeZoneId);
        }
    }
}
