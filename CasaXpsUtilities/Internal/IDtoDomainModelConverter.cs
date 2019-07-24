namespace CasaXpsUtilities.Internal
{
    using Ultimately;


    public interface IDtoDomainModelConverter<TDto, TDomainModel>
    {
        Option<TDomainModel> Convert(TDto dto);

        TDto Convert(TDomainModel model);
    }
}
