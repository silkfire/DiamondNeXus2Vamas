namespace CasaXpsUtilities.Vamas
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;


    public class DataSet
    {
        public ReadOnlyCollection<Block> Blocks { get; }


        private DataSet(IEnumerable<Block> blocks)
        {
            Blocks = blocks.ToList().AsReadOnly();
        }

        public static DataSet Create(IEnumerable<Block> blocks)
        {
            return new DataSet(blocks);
        }
    }
}
