namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans
{
    using Ultimately;
    using Ultimately.Collections;

    using System.Collections.Generic;


    public class ScanNumberRange
    {
        /// <summary>
        /// First scan number in range.
        /// </summary>
        public uint First { get; }

        /// <summary>
        /// Last scan number in range.
        /// </summary>
        public uint Last { get; }


        private ScanNumberRange(uint first, uint last)
        {
            First = first;
            Last = last;
        }

        public static Option<ScanNumberRange> Create(uint first, uint last)
        {
            var validationRules = new List<LazyOption>
            {
                Optional.Lazy(() => first > 0, "First scan number in range must be greater than zero"),
                Optional.Lazy(() => last  > 0, "Last scan number in range must be greater than zero"),
                Optional.Lazy(() => last  >= first, $"Last scan number in range must be greater than the first value: [{first}-{last}]"),
            };

            return validationRules.Reduce().Map(() => new ScanNumberRange(first, last));
        }


        public override string ToString() => $"{First}{(First != Last ? $"-{Last}" : "")}";
    }
}
