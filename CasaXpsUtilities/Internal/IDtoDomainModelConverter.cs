namespace CasaXpsUtilities.Internal
{
    using Upshot;


    public interface IDtoDomainModelConverter<TDto, TDomainModel>
    {
        Option<TDomainModel> Convert(TDto dto);

        TDto Convert(TDomainModel model);
    }
}
