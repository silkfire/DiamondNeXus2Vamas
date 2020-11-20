namespace DiamondNeXus2Vamas
{
    using CasaXpsUtilities.Shared;
    using CasaXpsUtilities.Vamas.IO;

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
                var resourceName = $"{_scope.Assembly.GetName().Name}.{_sourceDirectory}.{templateName}.vms";
                var resourceInfo = _scope.Assembly.GetManifestResourceInfo(resourceName);

                if (resourceInfo == null) throw new Exception($"Embedded template file not found under the name of '{resourceName}'");

                using var reader = new StreamReader(_scope.Assembly.GetManifestResourceStream(resourceName));
                    return await reader.ReadToEndAsync();
            }
        }
    }
}
