using System;
using System.Linq;
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
            var averagef = particleSwarm.GetParticles().Take(n + 1).Select(x => x.bestFitness).Average();
            var sd = particleSwarm.GetParticles().Take(n + 1).Select(x => Math.Pow(x.bestFitness - averagef, 2))
                .Average();
            return Math.Sqrt(sd);
        }
    }
}