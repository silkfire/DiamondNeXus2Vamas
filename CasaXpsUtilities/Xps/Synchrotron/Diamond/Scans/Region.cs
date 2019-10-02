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

        public IReadOnlyList<double> Counts { get; }

        public ushort ExcitationEnergy { get; }

        public double StepTime { get; }

        public double EnergyStep { get; }

        public string EnergyMode { get; }



        private Region(string name, ulong creationTimeUnix, double startingEnergyValue, IEnumerable<double> counts, ushort excitationEnergy, double stepTime, double energyStep, string energyMode)
        {
            Name = name;
            CreationTimeUnix = creationTimeUnix;
            StartingEnergyValue = startingEnergyValue;
            Counts = counts.ToList().AsReadOnly();
            ExcitationEnergy = excitationEnergy;
            StepTime = stepTime;
            EnergyStep = energyStep;
            EnergyMode = energyMode;
        }


        public static Option<Region> Create(string name, ulong creationTimeUnix, double startingEnergyValue, IEnumerable<double> counts, ushort excitationEnergy, double stepTime, double energyStep, string energyMode)
        {
            var validationRules = new List<LazyOption>
            {
                Optional.Lazy(() => !string.IsNullOrWhiteSpace(name), "Name cannot be empty"),
                Optional.Lazy(() => creationTimeUnix > 0, "Creation time must be greater than zero"),
                Optional.Lazy(() => startingEnergyValue > 0, $"Starting energy value must be greater than zero (was {startingEnergyValue})"),
                Optional.Lazy(() => counts != null, "List of counts cannot be null"),
                Optional.Lazy(() => excitationEnergy > 0, "Excitation energy must be greater than zero"),
                Optional.Lazy(() => stepTime > 0, $"Step time must be greater than zero (was {stepTime})"),
                Optional.Lazy(() => energyStep > 0, $"Energy step must be greater than zero (was {energyStep})"),
                Optional.Lazy(() => !string.IsNullOrWhiteSpace(energyMode), "Energy mode cannot be empty")
            };

            return validationRules.Reduce()
                                  .FlatMap(() => counts.ToList().SomeWhen(cc => cc.Count > 0, "List of counts cannot be empty"))
                                  .Map(cc => new Region(name, creationTimeUnix, startingEnergyValue, cc, excitationEnergy, stepTime, energyStep, energyMode), "Region validation failed");
        }


        public override string ToString() => $"REGION | {Name}";
    }
}
