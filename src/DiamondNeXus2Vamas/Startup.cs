namespace DiamondNeXus2Vamas
{
    using CasaXpsUtilities.Vamas.Internal.Time;
    using CasaXpsUtilities.Vamas.IO;
    using CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO;

    using Grace.DependencyInjection;

    using System.IO;


    internal static class Startup
    {
        public static DependencyInjectionContainer Container { get; } = new DependencyInjectionContainer();


        static Startup()
        {
            Container.Configure(_ =>
            {
                _.ExportFactory(() => new ConfigurationSerializer(Path.Combine(Directory.GetCurrentDirectory(), "config.json"))).Lifestyle.Singleton();
                _.ExportFactory<NeXusReader, ConversionService>(nr => new ConversionService(nr, new LocalTimeFactory("Europe/London"), new VamasWriter(new TemplateProvider(typeof(Startup), "Templates")))).Lifestyle.Singleton();
            });
        }
    }
}
