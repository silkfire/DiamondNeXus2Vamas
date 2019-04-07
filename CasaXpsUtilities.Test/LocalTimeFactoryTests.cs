namespace Silkfire.CasaXpsUtilities.Test
{
    using Core.Models.Properties;

    using NodaTime;
    using NodaTime.Text;
    using Xunit;


    public class LocalTimeFactoryTests
    {
        public class Create
        {
            [Theory]
            [InlineData("Europe/London")]
            [InlineData("Europe/Stockholm")]
            public void Should_return_object_with_specified_DateTime_value_and_offset_utc_offset_string(string timeZoneId)
            {
                const long unixTimeSeconds = 0L;

                var localTimeExpected = Instant.FromUnixTimeSeconds(unixTimeSeconds).InZone(DateTimeZoneProviders.Tzdb[timeZoneId]);


                var localTimeOutcome = new LocalTimeFactory(() => timeZoneId).Create(unixTimeSeconds);


                Assert.Equal(localTimeExpected.ToDateTimeUnspecified(), localTimeOutcome.Value);
                Assert.Equal(OffsetPattern.CreateWithInvariantCulture("-H").Format(localTimeExpected.Offset), localTimeOutcome.UtcOffset);
            }
        }
    }
}
