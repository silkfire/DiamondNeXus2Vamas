namespace CasaXpsUtilities.Test.Vamas.Internal.Time;

using NodaTimeLocalTime = CasaXpsUtilities.Vamas.Internal.Time.NodaTimeLocalTime;

using NodaTime;
using NodaTime.Text;
using Xunit;

using System;
using System.Collections.Generic;
using System.Globalization;


public static class LocalTimeTests
{
    private const string UnixTimeSecondsKeyEpoch = "epoch";
    private const string UnixTimeSecondsKeyNow   = "now";

    private static readonly Dictionary<string, long> s_unixTimeSecondsUtc = new()
                                                                            {
                                                                                [UnixTimeSecondsKeyEpoch] = 0L,
                                                                                [UnixTimeSecondsKeyNow]   = DateTimeOffset.Now.ToUnixTimeSeconds()
                                                                            };


    public class Create
    {
        [Theory]
        [InlineData("Europe/London",    UnixTimeSecondsKeyEpoch)]
        [InlineData("Europe/London",    UnixTimeSecondsKeyNow)]
        [InlineData("Europe/Stockholm", UnixTimeSecondsKeyEpoch)]
        [InlineData("Europe/Stockholm", UnixTimeSecondsKeyNow)]
        public void Should_return_local_time_with_specified_value_and_offset_utc_offset_string(string timeZoneId, string unixTimeSecondsKey)
        {
            var localTimeExpected = Instant.FromUnixTimeSeconds(s_unixTimeSecondsUtc[unixTimeSecondsKey]).InZone(DateTimeZoneProviders.Tzdb[timeZoneId]);

            var localTimeOutcome = NodaTimeLocalTime.Create((ulong)s_unixTimeSecondsUtc[unixTimeSecondsKey], timeZoneId);

            Assert.Equal(localTimeExpected.ToDateTimeUnspecified(), localTimeOutcome.Value);
            Assert.Equal($"{OffsetPattern.CreateWithInvariantCulture("-H").Format(localTimeExpected.Offset)}{(double.Parse(OffsetPattern.CreateWithInvariantCulture("%m").Format(localTimeExpected.Offset)) / 60).ToString("#.0#", CultureInfo.InvariantCulture)}", localTimeOutcome.UtcOffset);
        }
    }
}