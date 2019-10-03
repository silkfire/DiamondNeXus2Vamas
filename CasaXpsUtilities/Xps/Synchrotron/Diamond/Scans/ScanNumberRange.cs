namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans
{
    using Ultimately;
    using Ultimately.Collections;

    using System.Collections.Generic;


    public class ScanNumberRange
    {
        /// <summary>
        /// Starting scan number value in range.
        /// </summary>
        public uint StartingValue { get; }

        /// <summary>
        /// Final scan number value in range.
        /// </summary>
        public uint FinalValue { get; }


        private ScanNumberRange(uint startingValue, uint finalValue)
        {
            StartingValue = startingValue;
            FinalValue = finalValue;
        }

        public static Option<ScanNumberRange> Create(uint startingValue, uint finalValue)
        {
            var validationRules = new List<LazyOption>
            {
                Optional.Lazy(() => startingValue > 0, "Starting value of scan number range must be greater than zero"),
                Optional.Lazy(() => finalValue    > 0, "Final value of scan number range must be greater than zero"),
                Optional.Lazy(() => finalValue   >= startingValue, $"Starting value of scan number range must be smaller than the final one: {{ {startingValue} [too big] - {finalValue} }}"),
            };

            return validationRules.Reduce().Map(() => new ScanNumberRange(startingValue, finalValue));
        }


        public override string ToString() => $"{StartingValue}{(StartingValue != FinalValue ? $"-{FinalValue}" : "")}";
    }
}
