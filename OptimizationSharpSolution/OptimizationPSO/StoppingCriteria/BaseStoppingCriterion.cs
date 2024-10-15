using OptimizationPSO.Swarm;

namespace OptimizationPSO.StoppingCriteria
{
    public abstract class BaseStoppingCriterion
    {
        public string Description { get; }
        public abstract bool CanStop(ParticleSwarm particleSwarm);

        protected BaseStoppingCriterion(string description)
        {
            Description = description;
        }
    }
}