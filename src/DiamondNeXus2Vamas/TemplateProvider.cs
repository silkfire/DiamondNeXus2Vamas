namespace DiamondNeXus2Vamas;

using CasaXpsUtilities.Shared;
using CasaXpsUtilities.Vamas.IO;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

public class TemplateProvider(Assembly entryAssembly, string sourceDirectory) : ITemplateProvider
{
    private readonly Assembly _entryAssembly = entryAssembly;
    private readonly string _sourceDirectory = sourceDirectory;

    public LazyAsync<ReadOnlyDictionary<string, string>> GetTemplates()
    {
        return new LazyAsync<ReadOnlyDictionary<string, string>>(async () => new Dictionary<string, string>
                                                                             {
                                                                                 ["FILE_HEADER"] = await LoadTemplate("FileHeader"),
                                                                                 ["BLOCK"]       = await LoadTemplate("Block"),
                                                                                 ["FILE_FOOTER"] = await LoadTemplate("FileFooter")
                                                                             }.AsReadOnly());

        async Task<string> LoadTemplate(string templateName)
        {
            var resourceName = $"{_entryAssembly.GetName().Name}.{_sourceDirectory}.{templateName}.vms";
            _ = _entryAssembly.GetManifestResourceInfo(resourceName) ?? throw new InvalidOperationException($"Embedded template file not found under the name of '{resourceName}'");

            using var reader = new StreamReader(_entryAssembly.GetManifestResourceStream(resourceName)!);
            return await reader.ReadToEndAsync();
        }
    }
}
