using DiamondNeXus2Vamas;

using CasaXpsUtilities.Vamas.Internal.Time;
using CasaXpsUtilities.Vamas.IO;
using CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pastel;
using Ultimately;
using Ultimately.Collections;

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;


Console.OutputEncoding = Encoding.UTF8;

var bracketColour = Color.FromArgb(201, 64, 106);
var defaultMessagePrefix = $"\r\n{"[".Pastel(bracketColour)}{"CasaXpsUtilities".Pastel(Color.FromArgb(62, 102, 208))}{"]".Pastel(bracketColour)} ";

var errorMessageTemplate = $"{defaultMessagePrefix}{"ERROR".Pastel(Color.White).PastelBg(Color.FromArgb(222, 54, 26))} {{0}}";

var builder = Host.CreateApplicationBuilder();
builder.Services.AddSingleton(new ConfigurationSerializer(Path.Combine(Directory.GetCurrentDirectory(), "config.json")))
                .AddSingleton(new ConversionService(new NeXusReader(), new NodaTimeLocalTimeFactory("Europe/London"), new VamasWriter(new TemplateProvider(Assembly.GetEntryAssembly()!, "Templates"))));

var host = builder.Build();



try
{
    var configurationSerializer = host.Services.GetRequiredService<ConfigurationSerializer>();

    var configurationReadResult = configurationSerializer.Read().FlatMap(c => c.SomeWhen(cc => cc != null && !string.IsNullOrWhiteSpace(cc.ConversionDefinitionFilePath), "Definitions file path is empty"));

    var cachedDefinitionsFileInfo = "";

    if (configurationReadResult.HasValue)
    {
        cachedDefinitionsFileInfo = $" or press {"ENTER".Pastel(Color.FromArgb(255, 208, 0))} to reuse the previously used one";
    }


    // Resize window to fit the messages

    Console.Title = nameof(CasaXpsUtilities);

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        Console.WindowWidth = Math.Min(Console.LargestWindowWidth, 215);
    }

    Console.WriteLine($"Please specify the path to the conversion definition file{cachedDefinitionsFileInfo}:");


    var readLine = Console.ReadLine();

    string conversionDefinitionFilePath = null!;

    if (readLine == "")
    {
        foreach (var (configuration, _) in configurationReadResult)
        {
            conversionDefinitionFilePath = configuration.ConversionDefinitionFilePath;
        }

        // Move caret up one step and write on the previous line (due to the user having pressed ENTER - looks more neat)
        // https://www.lihaoyi.com/post/BuildyourownCommandLinewithANSIescapecodes.html#cursor-navigation

        Console.Write("\e[1A");
    }
    else
    {
        conversionDefinitionFilePath = readLine!.Trim('"');
    }

    var conversionResult = await host.Services.GetRequiredService<ConversionService>().ConvertAndCreateOutputFile(conversionDefinitionFilePath);

    await conversionResult.Match(
        some: async cr =>
        {
            Console.WriteLine($"{defaultMessagePrefix}Generated VAMAS file has been saved to {$"{cr.OutputDirectoryPath}".Pastel(Color.MediumSeaGreen)}{cr.OutputFilename.Pastel(Color.MediumSpringGreen)}.");

            if (!configurationReadResult.Exists(c => c.ConversionDefinitionFilePath == conversionDefinitionFilePath))
            {
                await configurationSerializer.SaveAsync(new Configuration(conversionDefinitionFilePath));
            }
        },
        none: e =>
        {
            Console.WriteLine(errorMessageTemplate, e.Print(s => s.Pastel(Color.FromArgb(230, 44, 34)), 0, " → ".Pastel(Color.WhiteSmoke)));

            return Task.CompletedTask;
        }
    );
}
catch (Exception e)
{
    Console.WriteLine(errorMessageTemplate, "An unexpected error occurred");

    if (args.SingleOrNone().Contains("debug")) Console.WriteLine(e);
}

Console.WriteLine("\nPress any key to continue.");
Console.ReadKey(true);
