namespace CasaXpsUtilities.DiamondNeXus2Vamas
{
    using Grace.DependencyInjection;

    using Vamas.Internal.Time;
    using Xps.Synchrotron.IO;
    using Xps.Synchrotron.IO.NeXus;


    internal static class Startup
    {
        public static DependencyInjectionContainer Container { get; } = new DependencyInjectionContainer();

        static Startup()
        {
            Container.Configure(_ =>
            {
                _.ExportAs<DiamondSpectraReader, ISpectraReader>();
                _.Export<LocalTimeFactory>().WithCtorParam<TimeZoneId>(() => () => "Europe/London")
                 .As<ILocalTimeFactory<ILocalTime>>();
            });


            // Dummy pre-loading

            Container.Locate<ILocalTimeFactory<ILocalTime>>().Create(0);
        }
    }
}
