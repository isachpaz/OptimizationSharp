using System;
using System.Linq;
using MathNet.Numerics.Statistics;
using OptimizationPSO.Particles;
using OptimizationPSO.Swarm;

namespace OptimizationPSO.StoppingCriteria
{
    public class StandardDeviationOfNBestSolutions : BaseStoppingCriterion
    {
        private readonly double _acceptanceError;

        public StandardDeviationOfNBestSolutions(double acceptanceError) : base("StandardDeviationOfNBestSolutions")
        {
            _acceptanceError = acceptanceError;
        }

        public override bool CanStop(ParticleSwarm particleSwarm)
        {
            var sd = CalculateStandardDeviation(particleSwarm);
            return sd < _acceptanceError;
        }

        private double CalculateStandardDeviation(ParticleSwarm particleSwarm)
        {
            var n = particleSwarm.NumDimensions;

            if (n < 3)
                n = 3;
            
            var bestOnes = particleSwarm.GetParticles().Take(n).ToList();

            var average = bestOnes.Select(x => x.bestFitness).Average();
            var sd1 = bestOnes.Select(x => Math.Pow(x.bestFitness - average, 2))
                .Average();
            sd1 = Math.Sqrt(sd1);

            var sd = bestOnes.Select(x => x.bestFitness).StandardDeviation();
            Console.WriteLine($"======================== SD={sd:E5}");

            var tolerance = 1E-6;
            var max = bestOnes.Max(x => x.bestFitness);
            var min = bestOnes.Min(x => x.bestFitness);
            var sIndex = 2.0*(max - min)/(min+max+tolerance);
            Console.WriteLine($"======================== sIndex={sIndex:E5}");
            return sd;
        }
    }
}