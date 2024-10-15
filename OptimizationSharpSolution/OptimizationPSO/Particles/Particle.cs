namespace OptimizationPSO.Particles
{
    public abstract class Particle
    {
        public double[] position;
        public double[] velocity;
        public double[] bestPosition;
        public double fitness;
        public double bestFitness;

        public Particle(int numDimensions)
        {
            position = new double[numDimensions];
            velocity = new double[numDimensions];
            bestPosition = new double[numDimensions];
        }
    }
}
