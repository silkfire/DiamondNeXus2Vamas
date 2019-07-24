namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.DomainModels
{
    using Ultimately;
    using Ultimately.Collections;

    using System.Collections.Generic;


    public class Region
    {
        public string Name { get; }


        private Region(string name)
        {
            Name = name;
        }


        public static Option<Region> Create(string name)
        {
            var validationRules = new List<LazyOption>
            {
                Optional.Lazy(() => !string.IsNullOrWhiteSpace(name), "Name of region cannot be empty")
            };

            return validationRules.Reduce()
                                  .Map(() => new Region(name));
        }


        public override string ToString() => Name;
    }
}
