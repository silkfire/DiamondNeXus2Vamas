﻿namespace CasaXpsUtilities.DiamondNeXus2Vamas
{
    using Vamas.Internal.Time;
    using Vamas.IO;
    using Xps.Synchrotron.Diamond.Scans.IO;

    using Grace.DependencyInjection;

    using System;
    using System.IO;


    internal static class Startup
    {
        public static DependencyInjectionContainer Container { get; } = new DependencyInjectionContainer();


        static Startup()
        {
            Container.Configure(_ =>
            {
                _.ExportFactory(() => new ConfigurationSerializer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"))).Lifestyle.Singleton();
                _.ExportFactory<NeXusReader, ConversionService>(nr => new ConversionService(nr, new LocalTimeFactory("Europe/London"), new VamasWriter(new TemplateProvider(typeof(Startup), "Templates")))).Lifestyle.Singleton();
            });
        }
    }
}
