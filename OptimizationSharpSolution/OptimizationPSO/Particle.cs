using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationPSO
{
    internal class Particle
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
            bestFitness = fitness = -double.MaxValue;
        }
    }
}
