namespace CasaXpsUtilities.IO
{
    using System.Collections.Generic;


    public interface IFileProvider
    {
        /// <summary>
        /// Defines a method to retrieve a list of filenames (including their paths).
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<string> GetFiles();
    }
}

