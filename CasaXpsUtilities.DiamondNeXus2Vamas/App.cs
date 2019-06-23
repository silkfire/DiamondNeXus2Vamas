namespace CasaXpsUtilities.DiamondNeXus2Vamas
{
    using System;
    using System.Drawing;
    using Internal;
    using Pastel;
    using Xps.Synchrotron.Diamond.Scans.DomainModels;
    using Xps.Synchrotron.Diamond.Scans.Dtos;
    using Xps.Synchrotron.Diamond.Scans.IO;


    public class App
    {
        public static void Main()
        {
            //var scan = Scan.Load(@"C:\Users\Gabriel\Documents\Visual Studio 2017\Projects\2018\CasaXpsUtilities\test-Oct\i09-144646.nxs");



            var reader = Startup.Container.Locate<IScanFileReader>();

            var scanResult = reader.Read(@"C:\Users\Gabriel\Documents\Visual Studio 2017\Projects\2018\CasaXpsUtilities\test-Oct\i09-144646.nxs");

            var scanConverter = Startup.Container.Locate<IDtoDomainModelConverter<ScanDto, Scan>>();

            var scan = scanResult.FlatMap(s => scanConverter.Convert(s));


            foreach (var (value, _) in scan)
            {
                foreach (var region in value.Regions)
                {
                    Console.WriteLine(region.Name.Pastel(Color.Aquamarine));
                }
            }


            //ISpectraReader spectraReader = null;

            //Task.Run(() => spectraReader = Startup.Container.Locate<ISpectraReader>());

            //Console.WriteLine("Please provide the location of the definitions file:");

            //var definitionsFilepath = Console.ReadLine();

            //var sw = new Stopwatch();
            //sw.Start();

            //var localTimeFactory = Startup.Container.Locate<ILocalTimeFactory<ILocalTime>>();

            //var ukLocalTime = localTimeFactory.Create(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            //var ukLocalTime2 = localTimeFactory.Create(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            //sw.Stop();

            //Console.WriteLine($"{(sw.Elapsed.TotalMilliseconds / 1000).ToString("0.00000000", CultureInfo.InvariantCulture).Pastel("FFCA00")}s");

            //Console.WriteLine(ukLocalTime);
            //Console.WriteLine(ukLocalTime2);


            Console.WriteLine("\nPress any key to continue.");
            Console.ReadKey(true);
        }
    }
}
