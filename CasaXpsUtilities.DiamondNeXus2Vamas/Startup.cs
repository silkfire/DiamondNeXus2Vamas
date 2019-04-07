namespace Silkfire.CasaXpsUtilities.DiamondNeXus2Vamas
{
    using Grace.DependencyInjection;
    using Core.Models.Properties;

    internal static class Startup
    {
        public static DependencyInjectionContainer Container { get; } = new DependencyInjectionContainer();

        static Startup()
        {
            Container.Configure(_ =>
            {
                _.Export<LocalTimeFactory>().WithCtorParam<TimeZoneId>(() => () => "Europe/London")
                 .As<ILocalTimeFactory<ILocalTime>>();
            });


            // Dummy pre-loading

            Container.Locate<ILocalTimeFactory<ILocalTime>>().Create(0);
        }
    }
}