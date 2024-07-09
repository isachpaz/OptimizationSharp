using OptimizationPSO.Swarm;

namespace OptimizationPSO.StoppingCriteria
{
    public class MaxIterationsStoppingCriterion : BaseStoppingCriterion
    {
        private readonly long _maxIterations;
        public static long Iterations = 0;

        public MaxIterationsStoppingCriterion(long maxIterations)
            : base($"MaxIterations={maxIterations}")
        {
            _maxIterations = maxIterations;
        }

        public override bool CanStop(ParticleSwarm particleSwarm)
        {
            Iterations++;
            if (Iterations >= _maxIterations)
            {
                return true;
            }

            return false;
        }
    }
}