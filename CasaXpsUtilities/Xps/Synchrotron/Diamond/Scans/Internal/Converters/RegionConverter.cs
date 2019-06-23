namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.Internal.Converters
{
    using CasaXpsUtilities.Internal;
    using DomainModels;
    using Dtos;

    using Upshot;


    public class RegionConverter : IDtoDomainModelConverter<RegionDto, Region>
    {
        public Option<Region> Convert(RegionDto dto)
        {
            return Region.Create(dto.Name);
        }

        public RegionDto Convert(Region model)
        {
            return new RegionDto(model.Name);
        }
    }
}
