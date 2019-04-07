namespace Silkfire.CasaXpsUtilities.Core.Models
{
    using Properties;


    public class VamasFile
    {
        private readonly ILocalTimeFactory<ILocalTime> _localTimeFactory;

        public VamasFile(ILocalTimeFactory<ILocalTime> localTimeFactory)
        {
            _localTimeFactory = localTimeFactory;
        }

        public ILocalTime NewCreationTime(long unixTimeSeconds) => _localTimeFactory.Create(unixTimeSeconds);
    }
}
