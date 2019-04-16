namespace CasaXpsUtilities.Xps.Synchrotron.IO.NeXus
{
    using IO;
    using Vamas;
    using Vamas.Internal.Time;

    using System.Collections.Generic;
    using System.IO;


    public class DiamondSpectraReader : ISpectraReader
    {
        private readonly ILocalTimeFactory<ILocalTime> _localTimeFactory;


        public DiamondSpectraReader(ILocalTimeFactory<ILocalTime> localTimeFactory)
        {
            _localTimeFactory = localTimeFactory;
        }


        public IReadOnlyList<Block> Read(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}
