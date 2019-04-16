namespace CasaXpsUtilities.Xps.Synchrotron.IO
{
    using Vamas;

    using System.Collections.Generic;
    using System.IO;

    public interface ISpectraReader
    {
        IReadOnlyList<Block> Read(Stream stream);
    }
}
