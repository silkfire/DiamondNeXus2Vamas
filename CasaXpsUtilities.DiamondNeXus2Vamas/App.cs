namespace Silkfire.CasaXpsUtilities.DiamondNeXus2Vamas
{
    using Core.Models;

    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading.Tasks;


    public class App
    {
        public static void Main(string[] args)
        {
            VamasFile vamasFile = null;

            Task.Run(() => vamasFile = Startup.Container.Locate<VamasFile>());

            Console.WriteLine("Please provide the location of the definitions file:");

            var definitionsFilepath = Console.ReadLine();

            var sw = new Stopwatch();
            sw.Start();

            var ukLocalTime = vamasFile.NewCreationTime(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            var ukLocalTime2 = vamasFile.NewCreationTime(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            sw.Stop();

            Console.WriteLine(sw.Elapsed.TotalMilliseconds.ToString("0.00000ms", CultureInfo.InvariantCulture));

            Console.WriteLine(ukLocalTime);
            Console.WriteLine(ukLocalTime2);


            Console.WriteLine("\nPress any key to continue.");
            Console.ReadKey(true);
        }
    }
}
