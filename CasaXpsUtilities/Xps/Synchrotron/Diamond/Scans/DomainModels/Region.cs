namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.DomainModels
{
    using Upshot;
    using Upshot.Collections;

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
                                  .FlatMap(() => new Region(name));
        }


        public override string ToString() => Name;
    }
}
