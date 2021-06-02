using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationPSO
{
    public class Particle
    {
        public double[] Position { get; internal set; }
        public double[] Velocity { get; }
        public double[] BestPosition { get; }
        public double Fitness { get; }
        public double BestFitness { get; }

        public Particle(double[] position, double[] velocity, double[] bestPosition, double fitness, double bestFitness)
        {
            Position = position;
            Velocity = velocity;
            BestPosition = bestPosition;
            Fitness = fitness;
            BestFitness = bestFitness;
        }

    }
}
