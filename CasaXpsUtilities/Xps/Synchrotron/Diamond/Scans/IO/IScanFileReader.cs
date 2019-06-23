namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO
{
    using Dtos;

    using Upshot;


    public interface IScanFileReader
    {
        Option<ScanDto> Read(string filepath);
    }
}
