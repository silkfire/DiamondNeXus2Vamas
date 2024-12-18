﻿using DiamondNeXus2Vamas;

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
                .AddSingleton(new ConversionService(new NeXusReader(), new LocalTimeFactory("Europe/London"), new VamasWriter(new TemplateProvider(Assembly.GetEntryAssembly()!, "Templates"))));

var host = builder.Build();



try
{
    var configurationSerializer = host.Services.GetRequiredService<ConfigurationSerializer>();

    var configurationReadResult = configurationSerializer.Read().FlatMap(c => c.SomeWhen(cc => cc != null && !string.IsNullOrWhiteSpace(cc.ConversionDefinitionFilepath), "Definitions filepath is empty"));

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

    string conversionDefinitionFilepath = null!;

    if (readLine == "")
    {
        foreach (var (configuration, _) in configurationReadResult)
        {
            conversionDefinitionFilepath = configuration.ConversionDefinitionFilepath;
        }

        // Move caret up one step and write on the previous line (due to the user having pressed ENTER - looks more neat)
        // http://www.lihaoyi.com/post/BuildyourownCommandLinewithANSIescapecodes.html

        Console.Write("\e[1A");
    }
    else
    {
        conversionDefinitionFilepath = readLine!.Trim('"');
    }

    var conversionResult = await host.Services.GetRequiredService<ConversionService>().ConvertAndCreateOutputFile(conversionDefinitionFilepath);

    await conversionResult.Match(
        some: async cr =>
        {
            Console.WriteLine($"{defaultMessagePrefix}Generated VAMAS file has been saved to {$"{cr.OutputDirectoryPath}".Pastel(Color.MediumSeaGreen)}{cr.OutputFilename.Pastel(Color.MediumSpringGreen)}.");

            if (!configurationReadResult.Exists(c => c.ConversionDefinitionFilepath == conversionDefinitionFilepath))
            {
                await configurationSerializer.SaveAsync(new Configuration(conversionDefinitionFilepath));
            }
        },
        none: e =>
        {
            Console.WriteLine(errorMessageTemplate, e.Print(s => s.Pastel(Color.FromArgb(230, 44, 34)), " → ".Pastel(Color.WhiteSmoke)));

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
