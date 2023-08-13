using System;
using OptimizationPSO.Particles;

namespace OptimizationPSO.Swarm
{
    public class ParticleSwarmMaximization : ParticleSwarm
    {
        public ParticleSwarmMaximization(Func<double[], double> evalFunc, PSOSolverConfig config,
            Action<Particle> updateParticlePositionFunc = null)
            : base(evalFunc, config, config.RandomEngine, updateParticlePositionFunc)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            BestFitness = -double.MaxValue;
            BestPosition = new double[NumDimensions];
            
            for (int i = 0; i < NumParticles; i++)
            {
                var p = new ParticleMaximization(NumDimensions);

                for (int j = 0; j < NumDimensions; j++)
                {
                    var diff = Config.UpperBound[j] - Config.LowerBound[j];
                    p.position[j] = NextDoubleInRange(Config.LowerBound[j], Config.UpperBound[j]);
                    p.velocity[j] = NextDoubleInRange(-diff, +diff);
                }

                Particles[i] = p;
            }
        }

        protected override void SortParticles()
        {
            
        }

        protected override void RunNMOptAndMoveParticles(int i)
        {
            
        }

        protected override void EvaluateParticle(Particle p)
        {
            p.fitness = FitnessFunc(p.position);

            if (p.fitness > p.bestFitness)
            {
                p.bestFitness = p.fitness;
                Array.Copy(p.position, p.bestPosition, p.position.Length);

                lock (_lock)
                {
                    if (p.bestFitness > BestFitness)
                    {
                        BestFitness = p.bestFitness;
                        Array.Copy(p.bestPosition, BestPosition, p.bestPosition.Length);
                    }
                }
            }
        }
    }
}