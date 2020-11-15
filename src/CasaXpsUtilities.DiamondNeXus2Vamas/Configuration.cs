namespace CasaXpsUtilities.DiamondNeXus2Vamas
{
    public sealed class Configuration
    {
        public string ConversionDefinitionFilepath { get; }


        public Configuration(string conversionDefinitionFilepath)
        {
            ConversionDefinitionFilepath = conversionDefinitionFilepath;
        }
    }
}
