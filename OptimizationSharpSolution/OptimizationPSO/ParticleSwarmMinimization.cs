using System;

namespace OptimizationPSO
{
    public class ParticleSwarmMinimization : ParticleSwarm
    {
        public ParticleSwarmMinimization(Func<double[], double> evalFunc, PSOSolverConfig config,
            Action<Particle> updateParticlePositionFunc = null)
            : base(evalFunc, config, config.RandomEngine, updateParticlePositionFunc)
        {
        }

        protected override void Initialize()
        {
            var numDimensions = Config.LowerBound.Length;
            var numParticles = Config.NumParticles;

            BestFitness = double.MaxValue;

            BestPosition = new double[numDimensions];
            Particles = new Particle[numParticles];

            for (int i = 0; i < numParticles; i++)
            {
                var p = new ParticleMinimization(numDimensions);

                for (int j = 0; j < numDimensions; j++)
                {
                    var diff = Config.UpperBound[j] - Config.LowerBound[j];
                    p.position[j] = NextDoubleInRange(Config.LowerBound[j], Config.UpperBound[j]);
                    p.velocity[j] = NextDoubleInRange(-diff, +diff);
                }

                UpdateParticlePositionFunc?.Invoke(p);
                Particles[i] = p;
            }
        }

        protected override void EvaluateParticle(Particle p)
        {
            p.fitness = FitnessFunc(p.position);

            if (p.fitness < p.bestFitness)
            {
                p.bestFitness = p.fitness;
                Array.Copy(p.position, p.bestPosition, p.position.Length);

                lock (_lock)
                {
                    if (p.bestFitness < BestFitness)
                    {
                        BestFitness = p.bestFitness;
                        Array.Copy(p.bestPosition, BestPosition, p.bestPosition.Length);
                    }
                }
            }
        }
    }
}