namespace CasaXpsUtilities.Vamas.IO
{
    using Shared;

    using System.Collections.Generic;


    public interface ITemplateProvider
    {
        LazyAsync<IReadOnlyDictionary<string, string>> GetTemplates();
    }
}
