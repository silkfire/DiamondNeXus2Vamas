namespace CasaXpsUtilities.DiamondNeXus2Vamas
{
    using Common;
    using Vamas.IO;

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Threading.Tasks;


    public class TemplateProvider : ITemplateProvider
    {
        private readonly Type _scope;
        private readonly string _sourceDirectory;

        public TemplateProvider(Type scope, string sourceDirectory)
        {
            _scope = scope;
            _sourceDirectory = sourceDirectory;
        }

        public LazyAsync<IReadOnlyDictionary<string, string>> GetTemplates()
        {
            return new LazyAsync<IReadOnlyDictionary<string, string>>(async () => new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
            {
                ["FILE_HEADER"] = await LoadTemplate("FileHeader"),
                ["BLOCK"]       = await LoadTemplate("Block"),
                ["FILE_FOOTER"] = await LoadTemplate("FileFooter")
            }));

            async Task<string> LoadTemplate(string templateName)
            {
                using (var reader = new StreamReader(_scope.Assembly.GetManifestResourceStream(_scope, $"{_sourceDirectory}.{templateName}.vms")))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}
