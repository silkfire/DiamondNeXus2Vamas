namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans
{
    using Ultimately;
    using Ultimately.Collections;

    using System.Collections.Generic;
    using System.Linq;

    public class Region
    {
        public string Name { get; }

        public ulong CreationTimeUnix { get; }

        public double StartingEnergyValue { get; }

        public IReadOnlyCollection<double> Counts { get; }

        public ushort ExcitationEnergy { get; }

        public double StepTime { get; }

        public double EnergyStep { get; }

        private Region(string name, ulong creationTimeUnix, double startingEnergyValue, IReadOnlyCollection<double> counts, ushort excitationEnergy, double stepTime, double energyStep)
        {
            Name = name;
            CreationTimeUnix = creationTimeUnix;
            StartingEnergyValue = startingEnergyValue;
            Counts = counts;
            ExcitationEnergy = excitationEnergy;
            StepTime = stepTime;
            EnergyStep = energyStep;
        }

        public static Option<Region> Create(string name, ulong creationTimeUnix, double startingEnergyValue, IEnumerable<double> counts, ushort excitationEnergy, double stepTime, double energyStep)
        {
            var validationRules = new List<LazyOption>
            {
                Optional.Lazy(() => !string.IsNullOrWhiteSpace(name), "Name cannot be empty"),
                Optional.Lazy(() => creationTimeUnix > 0, "Creation time must be greater than zero"),
                Optional.Lazy(() => counts != null, "List of counts cannot be null"),
                Optional.Lazy(() => excitationEnergy > 0, "Excitation energy must be greater than zero"),
                Optional.Lazy(() => stepTime > 0, $"Step time must be greater than zero (was {stepTime})"),
                Optional.Lazy(() => energyStep > 0, $"Energy step must be greater than zero (was {energyStep})")
            };

            return validationRules.Reduce()
                                  .FlatMap(() => counts.ToList().SomeWhen(cc => cc.Count > 0, "List of counts cannot be empty"))
                                  .Map(cc => new Region(name, creationTimeUnix, startingEnergyValue, cc.AsReadOnly(), excitationEnergy, stepTime, energyStep), "Region validation failed");
        }

        public override string ToString() => $"REGION | {Name}";
    }
}
