using System.Collections.Generic;

namespace OptimizationPSO.Particles
{
    public class ParticleComparer : IComparer<Particle>
    {
        public int Compare(Particle x, Particle y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            var fitnessComparison = x.bestFitness.CompareTo(y.bestFitness);
            if (fitnessComparison != 0) return fitnessComparison;
            return x.bestFitness.CompareTo(y.bestFitness);
        }
    }
}