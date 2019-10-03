namespace CasaXpsUtilities.DiamondNeXus2Vamas
{
    using Pastel;
    using Ultimately;
    using Ultimately.Async;

    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;


    public class App
    {
        public static async Task Main()
        {
            Console.OutputEncoding = Encoding.UTF8;


            var bracketColour = Color.FromArgb(201, 64, 106);
            var defaultMessagePrefix = $"\r\n{"[".Pastel(bracketColour)}{"CasaXpsUtilities".Pastel(Color.FromArgb(62, 102, 208))}{"]".Pastel(bracketColour)} ";

            var errorMessageTemplate = $"{defaultMessagePrefix}{"ERROR".Pastel(Color.White).PastelBg(Color.FromArgb(222, 54, 26))} {{0}}";


            try
            {
                var configurationSerializer = Startup.Container.Locate<ConfigurationSerializer>();

                var configurationReadResult = await configurationSerializer.Read().FlatMapAsync(c => c.SomeWhen(_c => _c != null && !string.IsNullOrWhiteSpace(_c.ConversionDefinitionFilepath), "Definitions filepath is empty"));

                var cachedDefinitionsFileInfo = "";

                if (configurationReadResult.HasValue)
                {
                    cachedDefinitionsFileInfo = $" or press {"ENTER".Pastel(Color.FromArgb(255, 208, 0))} to reuse the previously used one";
                }


                // Resize window to fit the messages

                Console.Title = nameof(CasaXpsUtilities);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Console.WindowWidth = 215;
                }

                Console.WriteLine($"Please specify the path to the conversion definition file{cachedDefinitionsFileInfo}:");


                var readLine = Console.ReadLine();

                string conversionDefinitionFilepath = null;

                if (readLine == "")
                {
                    configurationReadResult.MatchSome(c => conversionDefinitionFilepath = c.ConversionDefinitionFilepath);


                    // Move caret up one step and write on the previous line (due to the user having pressed ENTER - looks more neat)
                    // http://www.lihaoyi.com/post/BuildyourownCommandLinewithANSIescapecodes.html

                    Console.Write("\u001b[1A");
                }
                else
                {
                    conversionDefinitionFilepath = readLine.Trim('"');
                }

                //var sw = new Stopwatch();
                //sw.Start();
                var conversionResult = await Startup.Container.Locate<ConversionService>().ConvertAndCreateOutputFile(conversionDefinitionFilepath);
                //sw.Stop();
                //Console.WriteLine($"Elapsed time: {(double)sw.ElapsedMilliseconds / 1000:#0.000}s");
                await conversionResult.Match(
                    some: async cr =>
                    {
                        Console.WriteLine($"{defaultMessagePrefix}Generated VAMAS file has been saved to {$"{cr.outputDirectoryPath}\\".Pastel(Color.MediumSeaGreen)}{cr.outputFilename.Pastel(Color.MediumSpringGreen)}.");

                        if (!configurationReadResult.Exists(c => c.ConversionDefinitionFilepath == conversionDefinitionFilepath))
                        {
                            await configurationSerializer.SaveAsync(new Configuration(conversionDefinitionFilepath));
                        }
                    },
                    none: e =>
                    {
                        Console.WriteLine(errorMessageTemplate, e.Print(" → ".Pastel(Color.WhiteSmoke), 0, s => s.Pastel(Color.FromArgb(230, 44, 34))));

                        return Task.CompletedTask;
                    }
                );
            }
            catch (Exception e)
            {
                Console.WriteLine(errorMessageTemplate, "An unexpected error occurred.");
            }

            Console.WriteLine("\nPress any key to continue.");
            Console.ReadKey(true);
        }
    }
}
