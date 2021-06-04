namespace OptimizationPSO
{
    public class ParticleMinimization : Particle
    {
        public ParticleMinimization(int numDimensions) 
            : base(numDimensions)
        {
            bestFitness = fitness = double.MaxValue;
        }
    }
}