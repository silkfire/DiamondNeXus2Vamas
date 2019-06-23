namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.Dtos
{
    using System.Collections.Generic;


    public class ScanDto
    {
        public string Filepath { get; }

        public string Number { get; }

        public List<RegionDto> Regions { get; }


        public ScanDto(string filepath, string number, List<RegionDto> regions)
        {
            Filepath = filepath;
            Number = number;
            Regions = regions;
        }
    }
}
