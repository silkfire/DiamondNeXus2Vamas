namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO
{
    using DomainModels;

    using Ultimately;


    public interface IScanFileReader
    {
        Option<Scan> Read(string filepath);
    }
}
