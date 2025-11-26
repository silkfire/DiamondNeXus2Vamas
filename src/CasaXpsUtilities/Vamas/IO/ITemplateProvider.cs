using System.Collections.ObjectModel;

namespace CasaXpsUtilities.Vamas.IO;

using Shared;

/// <summary>
/// Defines a means to retrieve VAMAS templates.
/// </summary>
public interface ITemplateProvider
{
    /// <summary>
    /// Gets the VAMAS templates.
    /// </summary>
    LazyAsync<ReadOnlyDictionary<string, string>> GetTemplates();
}
