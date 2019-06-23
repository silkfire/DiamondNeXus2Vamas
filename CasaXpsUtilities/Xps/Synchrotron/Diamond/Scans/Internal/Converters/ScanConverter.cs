namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.Internal.Converters
{
    using CasaXpsUtilities.Internal;
    using DomainModels;
    using Dtos;

    using Upshot;
    using Upshot.Collections;

    using System.Linq;


    public class ScanConverter : IDtoDomainModelConverter<ScanDto, Scan>
    {
        private readonly IDtoDomainModelConverter<RegionDto, Region> _regionConverter;

        public ScanConverter(IDtoDomainModelConverter<RegionDto, Region> regionConverter)
        {
            _regionConverter = regionConverter;
        }


        public Option<Scan> Convert(ScanDto dto)
        {
            var regionsConversion = dto.Regions.Transform(r => _regionConverter.Convert(r));

            return regionsConversion.FlatMap(rs => Scan.Create(dto.Filepath, dto.Number, rs.ToList()));
        }

        public ScanDto Convert(Scan model)
        {
            return new ScanDto(model.Filepath, model.Number, model.Regions.Select(r => _regionConverter.Convert(r)).ToList());
        }
    }
}
