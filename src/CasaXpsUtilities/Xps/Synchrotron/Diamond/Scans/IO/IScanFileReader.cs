namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO
{
    using Ultimately;


    public interface IScanFileReader
    {
        Option<Scan> Read(ScanFile scanFile);

        Option<Scan> Read(string filepath);
    }
}
