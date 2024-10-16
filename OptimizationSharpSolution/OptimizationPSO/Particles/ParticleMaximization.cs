﻿namespace OptimizationPSO.Particles
{
    public class ParticleMaximization : Particle
    {
        public ParticleMaximization(int numDimensions) : base(numDimensions)
        {
            bestFitness = fitness = -double.MaxValue;
        }
    }
}