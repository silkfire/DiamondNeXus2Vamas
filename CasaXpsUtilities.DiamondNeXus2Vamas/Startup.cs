namespace CasaXpsUtilities.DiamondNeXus2Vamas
{
    using Internal;
    using IO;
    using Vamas.Internal.Time;
    using Xps.Synchrotron.Diamond.Scans.DomainModels;
    using Xps.Synchrotron.Diamond.Scans.Dtos;
    //using Xps.Synchrotron.Diamond.Scans.Internal.Converters;
    using Xps.Synchrotron.Diamond.Scans.IO;
    using Xps.Synchrotron.Diamond.Scans.IO.Services;

    using Grace.DependencyInjection;



    internal static class Startup
    {
        public static DependencyInjectionContainer Container { get; } = new DependencyInjectionContainer();


        static Startup()
        {
            Container.Configure(_ =>
            {
                _.ExportAs<ScanFileProvider, IFileProvider>();
                _.ExportAs<ScanFileReader, IScanFileReader>();

                //_.ExportAs<RegionConverter, IDtoDomainModelConverter<RegionDto, Region>>();
                //_.ExportAs<ScanConverter,   IDtoDomainModelConverter<ScanDto,   Scan>>();

                _.Export<LocalTimeFactory>().WithCtorParam<TimeZoneId>(() => () => "Europe/London")
                 .As<ILocalTimeFactory<ILocalTime>>();
            });


            // Dummy pre-loading

            Container.Locate<ILocalTimeFactory<ILocalTime>>().Create(0);
        }
    }
}
