namespace CasaXpsUtilities.Vamas.Internal.Time
{
    public interface ILocalTimeFactory<out TLocalTime>
        where TLocalTime : ILocalTime
    {
        TLocalTime Create(ulong unixTimeSeconds);
    }
}
