namespace Silkfire.CasaXpsUtilities.Test
{
    using Core.Models;
    using Core.Models.Properties;

    using FakeItEasy;
    using Xunit;


    public class VamasFileTests
    {
        public class NewCreationTime
        {
            [Fact]
            public void Should_return_the_exact_object()
            {
                const long unixTimeSeconds = 0L;

                var localTime = A.Fake<ILocalTime>();

                var localTimeFactory = A.Fake<ILocalTimeFactory<ILocalTime>>();
                A.CallTo(() => localTimeFactory.Create(unixTimeSeconds)).Returns(localTime);


                var localTimeOutcome = new VamasFile(localTimeFactory).NewCreationTime(unixTimeSeconds);

                Assert.Same(localTime, localTimeOutcome);
            }
        }
    }
}
