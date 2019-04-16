namespace CasaXpsUtilities.DiamondNeXus2Vamas
{
    using FluentResults;
    using Optional;
    using Pastel;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Vamas.Internal.Time;
    using Xps.Synchrotron.IO;

    public class App
    {
        public static void Main(string[] args)
        {
            const int number = 18;

            var validations = new List<Option<int, Error>>
            {
                number.SomeWhen(n =>  n > 20,      new Error("Value must be above 20, but was equal to or less.")),
                number.SomeWhen(n =>  n < 77,      new Error("Value must be below 77, but was equal to or greater.")),
                number.SomeWhen(n => (n & 1) == 1, new Error("Value must be odd.")),
            };


            var a = validations.Aggregate((v1, v2) => v1.FlatMap(_ => v2));

            a.Match(n => Console.WriteLine($"Validation passed for number {n}"), e => Console.WriteLine($"{$"Validation failed for {{{nameof(number)}: {number}}} => ".Pastel("FF3D00")} {e.Message}\n"));





            ISpectraReader spectraReader = null;

            Task.Run(() => spectraReader = Startup.Container.Locate<ISpectraReader>());

            Console.WriteLine("Please provide the location of the definitions file:");

            var definitionsFilepath = Console.ReadLine();

            var sw = new Stopwatch();
            sw.Start();

            var localTimeFactory = Startup.Container.Locate<ILocalTimeFactory<ILocalTime>>();

            var ukLocalTime = localTimeFactory.Create(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            var ukLocalTime2 = localTimeFactory.Create(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            sw.Stop();

            Console.WriteLine($"{(sw.Elapsed.TotalMilliseconds / 1000).ToString("0.00000000", CultureInfo.InvariantCulture).Pastel("FFCA00")}s");

            Console.WriteLine(ukLocalTime);
            Console.WriteLine(ukLocalTime2);


            Console.WriteLine("\nPress any key to continue.");
            Console.ReadKey(true);
        }
    }
}
