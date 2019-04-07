namespace Silkfire.CasaXpsUtilities.Core.Models.Properties
{
    public interface ILocalTimeFactory<out TLocalTime>
        where TLocalTime : ILocalTime
    {
        TLocalTime Create(long unixTimeSeconds);
    }
}
