namespace CasaXpsUtilities.Vamas.IO
{
    using Common;

    using System.Collections.Generic;


    public interface ITemplateProvider
    {
        LazyAsync<IReadOnlyDictionary<string, string>> GetTemplates();
    }
}
